using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Zeiss.PublicationManager.Data.DataSet;

namespace Zeiss.PublicationManager.Data.Excel.IO.Write
{  
    public class WriteExcel : ExcelIOBase
    {       
       
        #region CreateSpreadSheetEntries       
        #region CreateSharedString
        private static SharedStringTablePart GetSharedStringTablePart(ref SpreadsheetDocument spreadsheetDocument)
        {
            SharedStringTablePart sharedStringTablePart;
            if (spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Any())
            {
                sharedStringTablePart = spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
            }
            else
            {
                sharedStringTablePart = spreadsheetDocument.WorkbookPart.AddNewPart<SharedStringTablePart>();
            }

            return sharedStringTablePart;
        }

        protected static void AddSharedString(ref SpreadsheetDocument spreadsheetDocument, ref Cell newCell, string text)
        {
            //If no SharedStringTable is created, we create new one if no exist.
            //We shouldn't create a SharedStringTable if it's not used, because it can corrupt the file
            SharedStringTablePart sharedStringTablePart = GetSharedStringTablePart(ref spreadsheetDocument);

            int index = InsertSharedStringItem(text, ref sharedStringTablePart);

            newCell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
            newCell.CellValue = new CellValue(index.ToString());
        }

        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        protected static int InsertSharedStringItem(string text, ref SharedStringTablePart sharedStringTablePart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (sharedStringTablePart.SharedStringTable is null)
            {
                sharedStringTablePart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;
            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in sharedStringTablePart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            sharedStringTablePart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            sharedStringTablePart.SharedStringTable.Save();

            return i;
        }
        #endregion

        #region Cell    
        //letterIDAndValue: <cellReference, value>
        protected static void CreateCell(ref SpreadsheetDocument spreadsheetDocument, ref Row row, KeyValuePair<string, object> cellReferenceIDAndValue)
        {
            //Get reference cell
            Cell referenceCell = GetReferenceCell(row, cellReferenceIDAndValue.Key);
            if (referenceCell is null)
                throw new ArgumentException("The Cell-Reference: " + referenceCell + " is invalid.");
            // Add the cell to the cell table at cellReference.
            Cell newCell = new() { CellReference = cellReferenceIDAndValue.Key };
            row.InsertBefore(newCell, referenceCell);

            UpdateCell(ref spreadsheetDocument, ref newCell, cellReferenceIDAndValue.Value);
        }

        protected static void UpdateCell(ref SpreadsheetDocument spreadsheetDocument, ref Cell cell, object newValue)
        {
            switch (newValue)
            {
                case string objstr:
                    AddSharedString(ref spreadsheetDocument, ref cell, objstr);
                    break;

                case DateTime objdate:
                    //Normal way. Does NOT work for xlsx (!)
                    //string strdate = objdate.ToOADate().ToString();
                    //cell.DataType = CellValues.Date;
                    //cell.CellValue = new CellValue(strdate);

                    //"StyleIndex" is "1", because we added a new stylesheet (index 0 would be default) with "NumberFormatId=14"
                    //is in the 2nd item of 'CellFormats' array.
                    cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                    cell.StyleIndex = 1;
                    cell.CellValue = new CellValue(objdate.ToOADate().ToString(CultureInfo.InvariantCulture));
                    break;

                case bool objbool:
                    AddSharedString(ref spreadsheetDocument, ref cell, objbool.ToString());
                    break;

                default:
                    if (newValue is not null && Decimal.TryParse(newValue.ToString(), out decimal objdec))
                    {
                        cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                        cell.CellValue = new CellValue(objdec.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        //Enter an empty cell to make IO easier
                        AddSharedString(ref spreadsheetDocument, ref cell, " ");
                    }
                    break;
            }
        }
        #endregion

        protected static void RemoveRow(List<Row> deletingRows)
        {
            for (int i = 0; i < deletingRows.Count; i++)
                deletingRows[i].Remove();
        }

        protected static void RemoveRowAdvanced(SheetData sheetData, List<Row> deletingRows)
        {
            while (deletingRows.Any())
            {
                RemoveRowAdvanced(sheetData, deletingRows.FirstOrDefault());
                deletingRows.RemoveAt(0);
            }
        }

        protected static void RemoveRowAdvanced(SheetData sheetData, Row deletingRow)
        {
            List<Row> allRows = sheetData.Elements<Row>().ToList();
            uint deletedRowIndex = deletingRow.RowIndex.Value;
            deletingRow.Remove();
        
            for (int i = 0; i < allRows.Count; i++)
            {
                //Only change the indexes of the rows and cells after the deleted row
                if (allRows[i].RowIndex.Value > deletedRowIndex)
                {
                    List<Cell> cells = allRows[i].Elements<Cell>().ToList();
                    if (cells is not null)
                    {
                        for (int j = 0; j < cells.Count; j++)
                        {
                            string oldCellReference = cells[i].CellReference.Value;

                            //Decrement Row index
                            int rowIndex = Convert.ToInt32(Regex.Replace(oldCellReference, @"[^\d]+", "")) - 1;
                            string letterIndex = Regex.Replace(oldCellReference, @"[\d-]", "");

                            cells[i].CellReference.Value = $"{letterIndex}{rowIndex}";
                        }
                    }
                }            
            }
        }
        #endregion
    }
}
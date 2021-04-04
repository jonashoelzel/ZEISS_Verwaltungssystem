using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Zeiss.PublicationManager.Data.DataSet;

namespace Zeiss.PublicationManager.Data.IO.Excel
{  
    public class WriteExcel : ExcelIOBase
    {       
        #region Insert
        #region Public_Insert
        public static void Insert(string filepath, string worksheetName, List<object> columnValues)
        {
            List<string> columnLetterIDs = new();
            for (int i = 1; i <= columnValues.Count; i++)
                columnLetterIDs.Add(ConvertNumberToCellLetters(i));

            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData);
            InsertRow(ref spreadsheetDocument, sheetData, columnLetterIDs, columnValues);
            SaveSpreadsheetDocument(ref spreadsheetDocument);
        }

        //
        //public static void Insert(string filepath, string worksheetName, string startColumnID, List<List<object>> columnValues)
        //{
        //
        //}

        //public static void Insert(string filepath, string worksheetName, List<string> columnNames, List<List<object>> columnValues)
        //{

        //}

        //public static void Insert(string filepath, string worksheetName, string startColumnID, List<string> columnNames, List<List<object>> columnValues)
        //{
        //
        //}
        //

        #endregion

        #region Private_Insert
        private static void InsertRow(ref SpreadsheetDocument spreadsheetDocument, SheetData sheetData, List<string> columnLetterIDs, List<object> columnValues)
        {
            //Create new row
            int rowCount = sheetData.Elements<Row>().Count();
            Row row = new() { RowIndex = UInt32Value.FromUInt32((uint)(++rowCount)) };
            sheetData.Append(row);

            for (int i = 0; i < columnValues.Count; i++)
            {
                //Format XX00
                string cellReference = columnLetterIDs[i] + rowCount;
                CreateCell(ref spreadsheetDocument, columnValues[i], row, cellReference);              
            }
        }
        #endregion
        #endregion
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

        private static void AddSharedString(ref SpreadsheetDocument spreadsheetDocument, ref Cell newCell, string text)
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
        private static int InsertSharedStringItem(string text, ref SharedStringTablePart sharedStringTablePart)
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

        #region CreateCell
        private static void CreateCell(ref SpreadsheetDocument spreadsheetDocument, object cellEntry, Row row, string cellReference)
        {
            //Get reference cell
            Cell referenceCell = GetReferenceCell(row, cellReference);
            // Add the cell to the cell table at cellReference.
            Cell newCell = new() { CellReference = cellReference };
            row.InsertBefore(newCell, referenceCell);

            switch (cellEntry)
            {
                case string objstr:
                    AddSharedString(ref spreadsheetDocument, ref newCell, objstr);
                    break;

                case DateTime objdate:
                    //Normal way. Does NOT work for xlsx (!)
                    //string strdate = objdate.ToOADate().ToString();
                    //newCell.DataType = CellValues.Date;
                    //newCell.CellValue = new CellValue(strdate);

                    //"StyleIndex" is "1", because we added a new stylesheet (index 0 would be default) with "NumberFormatId=14"
                    //is in the 2nd item of 'CellFormats' array.
                    newCell.DataType = new EnumValue<CellValues>(CellValues.Number);
                    newCell.StyleIndex = 1;
                    newCell.CellValue = new CellValue(objdate.ToOADate().ToString(CultureInfo.InvariantCulture));
                    break;

                case bool objbool:
                    AddSharedString(ref spreadsheetDocument, ref newCell, objbool.ToString());
                    break;

                default:
                    if (cellEntry is not null)
                    {
                        if (Decimal.TryParse(cellEntry.ToString(), out decimal objdec))
                        {
                            newCell.DataType = new EnumValue<CellValues>(CellValues.Number);
                            newCell.CellValue = new CellValue(objdec.ToString(CultureInfo.InvariantCulture));
                        }
                    }
                    else
                    {
                        //Enter an empty cell to make IO easier
                        AddSharedString(ref spreadsheetDocument, ref newCell, " ");
                    }
                    break;
            }
        }
        #endregion
        #endregion
    }
}
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
    public abstract class WriteExcel : ExcelIOBase
    {

        #region CreateSpreadSheetEntries       
        #region CreateSharedString
        private static void AddSharedString(ref SpreadsheetDocument spreadsheetDocument, ref Cell newCell, string text)
        {
            //If no SharedStringTable is created, we create new one if no exist.
            //We shouldn't create a SharedStringTable if it's not used, because it can corrupt the file
            SharedStringTablePart sharedStringTablePart = GetSharedStringTablePart(ref spreadsheetDocument);

            int index = InsertSharedStringItem(text, ref sharedStringTablePart);

            newCell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
            newCell.CellValue = new CellValue(index.ToString());
        }


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

        #region Cell    
        //cellReferenceIDAndValue: <cellReference, value>
        protected static void CreateCell(ref SpreadsheetDocument spreadsheetDocument, ref Row row, KeyValuePair<string, object> cellReferenceIDAndValue)
        {
            //Get reference cell
            //The referenceCell is the Cell in the previous row with the same letterIndex
            Cell referenceCell = GetReferenceCell(row, cellReferenceIDAndValue.Key);
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

        private static Cell GetReferenceCell(Row row, string cellName)
        {
            if (String.IsNullOrEmpty(cellName))
                return null;

            foreach (Cell cell in row.Elements<Cell>())
            {
                if (string.Compare(cell.CellReference, cellName, true) > 0)
                {
                    return cell;
                }
            }

            return null;
        }
        #endregion
        #endregion

        protected enum LetterEnum
        {
            A = 1,
            B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y,
            Z = 26
        }


        protected static List<string> GetCellReferenceLetters(int count)
        {
            List<string> columnLetterIDs = new();
            for (int i = 1; i <= count; i++)
                columnLetterIDs.Add(ConvertNumberToCellLetters(i));

            return columnLetterIDs;
        }


        //Excel column names are from A-Z over AA-AZ and ZA-ZZ up to AAA-ZZZ, [...]
        private static string ConvertNumberToCellLetters(int number)
        {
            //If the number is invalid
            if (number <= 0)
                throw new IndexOutOfRangeException("Value 'number' must be a value greater or equal 1. Current 'number was " + number);

            string columnname = "";
            int letterEnumCounter = 0;
            int letterValue = number;

            //For columnnames with multiple letters
            while (letterValue > 26)
            {
                letterValue -= 26;
                letterEnumCounter++;

                //Appends a Z for columnnames with 3 or more letters
                if (letterEnumCounter > 26)
                {
                    letterEnumCounter -= 26;
                    columnname += "Z";
                }
            }

            //Converts the lettervalues into the letter
            LetterEnum letter;
            if (letterEnumCounter > 0)
            {
                letter = (LetterEnum)letterEnumCounter;
                columnname += letter.ToString();
            }

            letter = (LetterEnum)letterValue;
            columnname += letter.ToString();


            return columnname;
        }

    }
}
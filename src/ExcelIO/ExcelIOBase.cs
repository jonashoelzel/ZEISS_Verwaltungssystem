using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


namespace Zeiss.PublicationManager.Data.Excel.IO
{
    public class ExcelIOBase
    {
        #region GetCellInformation
        protected enum LetterEnum
        {
            A = 1,
            B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y,
            Z = 26
        }

        //Excel column names are from A-Z over AA-AZ and ZA-ZZ up to AAA-ZZZ, [...]
        protected static string ConvertNumberToCellLetters(int number)
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


        protected static object ReadCell(Cell cell, SharedStringTable sharedStringTable)
        {
            //Make sure that the Excel has a SharedStringTable, the Cell has a DataType and is a String
            if (cell.DataType is not null && sharedStringTable is not null && cell.DataType == CellValues.SharedString)
            {
                var cellValue = cell.InnerText;
                //Return String
                return (sharedStringTable.ElementAt(Int32.Parse(cellValue)).InnerText);
            }
            //DataType is null
            else
            {
                //Check if StyleIndex is a Date Format
                if (Int32.TryParse(cell.StyleIndex?.InnerText, out int styleIndex))
                {
                    //Standard date format
                    if (styleIndex >= 12 && styleIndex <= 22
                        //Formatted date format
                        || styleIndex >= 165 && styleIndex <= 180
                        //Number formats that can be interpreted as a number
                        || styleIndex >= 1 && styleIndex <= 5)
                    {
                        if (double.TryParse(cell.CellValue.Text, out double dateTimeDouble))
                        {
                            //Return Date
                            return (DateTime.FromOADate(dateTimeDouble));
                        }
                    }
                }

                //Default is number (if StyleIndex is null or any other StyleIndex)
                return Convert.ToDecimal(cell.CellValue.Text);
            }
        }



        protected static Cell GetReferenceCell(Row row, string cellName)
        {
            if (String.IsNullOrEmpty(cellName))
                return null;

            foreach (Cell cell in row.Elements<Cell>())
            {
                if (string.Compare(cell.CellReference.Value, cellName, true) > 0)
                {
                    return cell;
                }
            }

            return null;
        }

        protected static List<string> GetCellReferenceLetters(int count)
        {
            List<string> columnLetterIDs = new();
            for (int i = 1; i <= count; i++)
                columnLetterIDs.Add(ConvertNumberToCellLetters(i));

            return columnLetterIDs;
        }


        protected static List<string> GetColumnLetterIDsOfColumnNames(ref SpreadsheetDocument spreadsheetDocument, SheetData sheetData, List<string> columnNames)
        {
            //Try to read SharedStringTable if it exists. If not, make sure to do NOT try to read from it
            SharedStringTable sharedStringTable = spreadsheetDocument?.WorkbookPart?.SharedStringTablePart?.SharedStringTable;

            List<string> letterIDs = new();

            List<object> objectList = new();
            foreach (string strobj in columnNames)
            {
                objectList.Add(strobj);
            }

            Row columns = SearchRow(ref spreadsheetDocument, sheetData, objectList);
            foreach (string name in columnNames)
            {
                foreach (Cell cell in columns.Elements<Cell>())
                {
                    object entry = ReadCell(cell, sharedStringTable);
                    if (CompareObjects(name, entry))
                    {
                        letterIDs.Add(GetLetterIDOfCellReference(cell.CellReference));
                        break;
                    }
                }
            }

            return letterIDs;
        }

        protected static string GetLetterIDOfCellReference(string cellReference)
        {
            for (int i = 0; i < cellReference.Length; i++)
            {
                char c = cellReference[i];
                if (Int32.TryParse(c.ToString(), out _))
                    return cellReference[0..i];
            }

            //Already letters only
            return cellReference;
        }

        #endregion

        #region CheckExists
        public static bool WorksheetExists(ref SpreadsheetDocument spreadsheetDocument, string worksheetName, out IEnumerable<Sheet> sheetsIEnum)
        {
            //Search for specific sheet
            sheetsIEnum = spreadsheetDocument?.WorkbookPart?.Workbook?.Descendants<Sheet>()?.Where(s => s.Name == worksheetName);

            return sheetsIEnum.Any();
        }

        public static bool WorksheetExists(ref string filepath, string worksheetName)
        {
            if (!CheckPathExist(ref filepath))
                return false;

            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filepath, false);

            //Search for specific sheet
            IEnumerable<Sheet> sheetsIEnum = spreadsheetDocument?.WorkbookPart?.Workbook?.Descendants<Sheet>()?.Where(s => s.Name == worksheetName);
            //If specified sheet does not exists => return false
            bool isExists = sheetsIEnum.Any();

            spreadsheetDocument.Close();

            return isExists;
        }
        #endregion

        #region CheckPaths
        //Does check, if the filepath does exist
        public static bool CheckPathExist(ref string filepath)
        {
            CheckAndConvertLongFilePath(ref filepath);

            //If the path exists, it returns true and other functions can work further
            return (File.Exists(filepath));
        }

        public static void CheckAndConvertLongFilePath(ref string filepath)
        {
            //Checks for longer filepaths (MAX_PATH is regularly 260)
            if (filepath.Length >= 256)
            {
                //Checks if file does not exists or/and if system cannot access it
                if (!File.Exists(filepath))
                {
                    //Adds the prefix to exceed MAX_PATH
                    filepath = @"\\?\" + filepath;

                    //Either file does not exist or prefix is unsupported if true
                    if (!File.Exists(filepath))
                        throw new PathTooLongException("The entered filepath:\n" + filepath +
                            "\nis too long (and current IO API does not support \"" + @"\\?\" + "\") or does not exist");
                }
            }
        }
        #endregion

        #region ExcelIO
        #region Write
        protected static SpreadsheetDocument OpenSpreadsheetDocument(string filepath, string worksheetName, out SheetData sheetData, bool isCreateable = true, bool isEditable = true)
        {
            SpreadsheetDocument spreadsheetDocument;

            if (CheckPathExist(ref filepath))
            {
                spreadsheetDocument = SpreadsheetDocument.Open(filepath, isEditable);
                //Open worksheet and if it doesn't exist - create it
                if (!OpenWorksheet(ref spreadsheetDocument, worksheetName, out sheetData))
                {
                    if (isEditable && isCreateable)
                    {
                        sheetData = CreateNewWorkbookPartAndGetSheetData(ref spreadsheetDocument, worksheetName);
                    }
                    else
                        throw new UnauthorizedAccessException("Worksheet: " + worksheetName + "; at file: " + filepath + " not found and worksheet cannot be created.");
                }
            }
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            else if (isCreateable)
            {
                spreadsheetDocument = CreateSpreadsheetDocument(filepath, worksheetName, out sheetData);               
            }
            else
                throw new FileNotFoundException("Unable to open (or create) file at path: " + filepath);

            return spreadsheetDocument;
        }

        protected static SpreadsheetDocument CreateSpreadsheetDocument(string filepath, string worksheetName, out SheetData sheetData)
        {
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);
            sheetData = CreateNewWorkbookPartAndGetSheetData(ref spreadsheetDocument, worksheetName, false);

            return spreadsheetDocument;
        }

        protected static void SaveSpreadsheetDocument(ref SpreadsheetDocument spreadsheetDocument)
        {
            // Save Close the document.
            spreadsheetDocument.Close();
        }
        #region CreateWorkbook
        protected static SheetData CreateNewWorkbookPartAndGetSheetData(ref SpreadsheetDocument spreadsheetDocument, string worksheetName, bool isAppendable = true)
        {
            if (isAppendable)
            {
                // Add a blank WorksheetPart.
                WorksheetPart worksheetPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string relationshipId = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart);
                uint sheetId = GetUniqueSheetID(ref sheets);

                // Append the new worksheet and associate it with the workbook.
                Sheet sheet = new() { Id = relationshipId, SheetId = sheetId, Name = worksheetName };

                sheets.Append(sheet);

                // Get the sheetData cell table.
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                return sheetData;
            }
            else
            {
                // Add a WorkbookPart to the document.
                //(Only possible, if no WorkbookPart exists)
                WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                AddAndAppendStyleSheet(ref spreadsheetDocument);

                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                // Add Sheets to the Workbook.
                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                uint sheetID = GetUniqueSheetID(ref sheets);

                // Append a new worksheet and associate it with the workbook.
                Sheet sheet = new() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = UInt32Value.FromUInt32(sheetID), Name = worksheetName };

                sheets.Append(sheet);

                // Get the sheetData cell table.
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                return sheetData;
            }
        }

        private static void AddAndAppendStyleSheet(ref SpreadsheetDocument spreadsheetDocument)
        {
            // Add minimal Stylesheet
            var stylesPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorkbookStylesPart>();
            stylesPart.Stylesheet = new Stylesheet
            {
                Fonts = new Fonts(new Font()),
                Fills = new Fills(new Fill()),
                Borders = new Borders(new Border()),
                CellStyleFormats = new CellStyleFormats(new CellFormat()),
                CellFormats =
                    new CellFormats(
                        new CellFormat(),
                        //This Style is for dates in xlsx (Excel) files
                        //To use it call StyleIndex=1
                        new CellFormat
                        {
                            NumberFormatId = 14,
                            ApplyNumberFormat = true
                        })
            };
        }
        #endregion
        #endregion

        #region Read
        private static uint GetUniqueSheetID(ref Sheets sheets)
        {
            // Get a unique ID for the new worksheet.
            uint sheetId = 1;
            if (sheets?.Elements<Sheet>()?.Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            return sheetId;
        }

        protected static bool OpenWorksheet(ref SpreadsheetDocument spreadsheetDocument, string worksheetName, out SheetData sheetData)
        {        
            if (WorksheetExists(ref spreadsheetDocument, worksheetName, out IEnumerable<Sheet> sheetsIEnum))
            {
                //Open worksheet
                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(sheetsIEnum.First().Id);
                sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                return true;
            }

            sheetData = null;
            return false;
        }
        #endregion
        #endregion

        #region GetRowInformation
        protected static Row SearchRow(ref SpreadsheetDocument spreadsheetDocument, SheetData sheetData, List<object> columnConditions)
        {
            //Try to read SharedStringTable if it exists. If not, make sure to do NOT try to read from it
            SharedStringTable sharedStringTable = spreadsheetDocument?.WorkbookPart?.SharedStringTablePart?.SharedStringTable;

            foreach (Row row in sheetData.Elements<Row>())
            {
                if (CompareRows(row, sharedStringTable, columnConditions))
                    return row;
            }

            //Row not found
            return null;
        }


        //protected static Row SearchRow(ref SpreadsheetDocument spreadsheetDocument, List<string> columnIDs, List<object> columnConditions)
        //{

        //}

        protected static bool CompareRows(Row row, SharedStringTable sharedStringTable, List<object> columnConditions)
        {
            //Create 'copy'
            List<object> leftConditions = new(columnConditions);
            foreach (Cell cell in row.Elements<Cell>())
            {
                object entry = ReadCell(cell, sharedStringTable);

                foreach (var condition in leftConditions)
                {
                    if (CompareObjects(entry, condition))
                    {
                        leftConditions.Remove(condition);
                        break;
                    }
                }
            }

            //If an condition is left that means that not all conditions were matched
            return !(leftConditions.Any());
        }
        #endregion

        #region Protected_Helper_Methods

        protected static bool CompareObjects(object a, object b)
        {
            //Compare datatypes
            if (a.GetType() == b.GetType())
            {
                switch (a)
                {
                    case string objstr:
                        return objstr.Equals(b);

                    case DateTime objdate:
                        return objdate.Equals(b);

                    case bool objbool:
                        return objbool.Equals(b);

                    default:
                        if (a is not null)
                        {
                            return (Convert.ToDecimal(a) == Convert.ToDecimal(b));
                        }
                        //Both objects are null
                        else
                            return true;
                }
            }

            return false;
        }

        #endregion
    }
}

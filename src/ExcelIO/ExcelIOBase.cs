using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using System.Text.RegularExpressions;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Zeiss.PublicationManager.Data.Excel.IO
{
    public abstract class ExcelIOBase
    {
        #region GetCellInformation      
        protected static object ReadCell(Cell cell, SharedStringTable sharedStringTable)
        {
            //Make sure that the Excel has a SharedStringTable, the Cell has a DataType and is a String
            if (cell.DataType is not null && sharedStringTable is not null && cell.DataType == CellValues.SharedString)
            {
                var cellValue = cell.InnerText;
                //Return String
                return (sharedStringTable.ElementAt(Int32.Parse(cellValue)).InnerText);
            }
            //DataType is null, but cell contains text
            else if (!String.IsNullOrEmpty(cell?.CellValue?.Text))
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
                        //Make sure that the double is converted into the correct format (with '.' instead of ',')
                        if (double.TryParse(cell.CellValue.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double dateTimeDouble))
                        {
                            return DateTime.FromOADate(dateTimeDouble);
                        }
                    }
                }

                //Default is number (if StyleIndex is null or any other StyleIndex)
                return Convert.ToDecimal(cell.CellValue.Text);
            }

            //If the Cell has no cell text
            return new string(" ");
        }

        protected static string GetLetterIDOfCellReference(string cellReference)
        {
            string letterID = Regex.Match(cellReference, @"[^\d]+").Value;

            return letterID;
        }

        protected static string GetColumnLetterIDsOfColumnNames(ref SpreadsheetDocument spreadsheetDocument, SheetData sheetData, string columnNames, out int rowIndex)
        {
            Row row = SearchRow(ref spreadsheetDocument, sheetData, columnNames);
            if (row is not null)
                return GetColumnLetterIDsOfColumnNames(ref spreadsheetDocument, row, columnNames, out rowIndex);

            rowIndex = -1;
            return null;
        }

        protected static string GetColumnLetterIDsOfColumnNames(ref SpreadsheetDocument spreadsheetDocument, Row row, string columnNames, out int rowIndex)
        {
            //Try to read SharedStringTable if it exists. If not, make sure to do NOT try to read from it
            SharedStringTable sharedStringTable = spreadsheetDocument?.WorkbookPart?.SharedStringTablePart?.SharedStringTable;
            rowIndex = -1;

            if (row is not null)
            {
                rowIndex = Convert.ToInt32(row.RowIndex.Value);
                foreach (Cell cell in row.Elements<Cell>())
                {
                    object entry = ReadCell(cell, sharedStringTable);
                    if (CompareObjects(columnNames, entry))
                        return GetLetterIDOfCellReference(cell.CellReference.Value);
                }
            }

            return null;
        }

        //return: <letterID, value>
        //columnNamesAndValues: <columName, value>
        protected static Dictionary<string, object> ConvertColumnNamesAndValuesToLetterIDsAndValues
            (ref SpreadsheetDocument spreadsheetDocument, SheetData sheetData, Dictionary<string, object> columnNamesAndValues)
        {
            //<letterID, value>
            Dictionary<string, object> idsAndValues = new();

            foreach (var columnAndValue in columnNamesAndValues)
            {
                string letterID = GetColumnLetterIDsOfColumnNames(ref spreadsheetDocument, sheetData, columnAndValue.Key, out _);
                if (letterID is not null)
                    idsAndValues.Add(letterID, columnAndValue.Value);
            }
                
            return idsAndValues;
        }
        #endregion

        #region CheckExists
        /// <summary>
        /// Check if a worksheet does exist in a spreadsheet and optional it returns the sheet(s) in (parameter) 'sheetsIEnum'.
        /// </summary>
        /// <param name="spreadsheetDocument">
        /// Spreadsheet where to search for the worksheet.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet that should be searched.
        /// </param>
        /// <param name="sheetsIEnum">
        /// This returns the sheet(s) that do have the name of (parameter) 'worksheetName'.
        /// </param>
        /// <returns>
        /// True, if worksheet with (parameter) 'worksheetName' does exist, otherwise False.
        /// </returns>
        protected static bool WorksheetExists(ref SpreadsheetDocument spreadsheetDocument, string worksheetName, out IEnumerable<Sheet> sheetsIEnum)
        {
            //Search for specific sheet
            sheetsIEnum = spreadsheetDocument?.WorkbookPart?.Workbook?.Descendants<Sheet>()?.Where(s => s.Name == worksheetName);

            return sheetsIEnum.Any();
        }
        #endregion

        #region CheckPaths
        /// <summary>
        /// Check if a path at the specified (parameter) 'filepath' does exist. 
        /// If the filepath is too long it'll try to access directly to the OS-File-System.
        /// </summary>
        /// <param name="filepath">
        /// The path to the file that should be searched.
        /// If the filepath is too long it'll try to access directly to the OS-File-System to search for the file.
        /// </param>
        /// <returns>
        /// True, if the file exists, otherwise false.
        /// </returns>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        protected static bool CheckPathExist(ref string filepath)
        {
            CheckAndConvertLongFilePath(ref filepath);

            //If the path exists, it returns true and other functions can work further
            return File.Exists(filepath);
        }

        private static void CheckAndConvertLongFilePath(ref string filepath)
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
                            "\nis too long (and current OS-IO-API does not support \"" + @"\\?\" + "\") or does not exist");
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
        private static SheetData CreateNewWorkbookPartAndGetSheetData(ref SpreadsheetDocument spreadsheetDocument, string worksheetName, bool isAppendable = true)
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
        private static bool OpenWorksheet(ref SpreadsheetDocument spreadsheetDocument, string worksheetName, out SheetData sheetData)
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

        private static uint GetUniqueSheetID(ref Sheets sheets)
        {
            // Get a unique ID for the new worksheet.
            uint sheetId = 1;
            if ((sheets?.Elements<Sheet>()?.Any()) ?? false)
            {
                sheetId = sheets.Elements<Sheet>().Max(s => s.SheetId.Value) + 1;
            }

            return sheetId;
        }
        #endregion
        #endregion

        #region GetRowInformation
        //columnConditions can be type of 'List<object>', 'string', 'Dictionary<string, object>' or 'KeyValuePair<string, object>'
        //objects (values) in columnConditions are the conditions and strings (keys) are columnLetterIDs
        protected static Row SearchRow(ref SpreadsheetDocument spreadsheetDocument, SheetData sheetData, object columnConditions)
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

        //columnConditions can be type of 'List<object>', 'string', 'Dictionary<string, object>' or 'KeyValuePair<string, object>'
        //objects (values) in columnConditions are the conditions and strings (keys) are columnLetterIDs
        protected static List<Row> SearchRows(ref SpreadsheetDocument spreadsheetDocument, SheetData sheetData, object columnConditions)
        {
            //Try to read SharedStringTable if it exists. If not, make sure to do NOT try to read from it
            SharedStringTable sharedStringTable = spreadsheetDocument?.WorkbookPart?.SharedStringTablePart?.SharedStringTable;

            return SearchRows(sharedStringTable, sheetData, columnConditions);
        }

        //columnConditions can be type of 'List<object>', 'string', 'Dictionary<string, object>' or 'KeyValuePair<string, object>'
        //objects (values) in columnConditions are the conditions and strings (keys) are columnLetterIDs
        protected static List<Row> SearchRows(SharedStringTable sharedStringTable, SheetData sheetData, object columnConditions)
        {
            List<Row> rows = new();

            foreach (Row row in sheetData.Elements<Row>())
            {
                if (CompareRows(row, sharedStringTable, columnConditions))
                    rows.Add(row);
            }

            return rows;
        }


        //columnConditions can be type of 'List<object>', 'string', 'Dictionary<string, object>' or 'KeyValuePair<string, object>'
        //objects (values) in columnConditions are the conditions and strings (keys) are columnLetterIDs
        private static bool CompareRows(Row row, SharedStringTable sharedStringTable, object columnConditions)
        {
            return columnConditions switch
            {            
                //<letterID, value>
                Dictionary<string, object> dicCon => CompareRows(row, sharedStringTable, dicCon),
                KeyValuePair<string, object> kvpCon => CompareRows(row, sharedStringTable, new Dictionary<string, object> { { kvpCon.Key, kvpCon.Value } }),

                List<object> lstCon => CompareRows(row, sharedStringTable, lstCon),
                object strCon => CompareRows(row, sharedStringTable, new List<object> { strCon }),

                _ => throw new InvalidCastException("Cannot convert 'columnConditions', because type of 'columnCondition' was invalid.\n" +
                    "Only 'List<string>', 'Dictionary<string, object>' and 'KeyValuePair<string, object>' are accepted"),
            };
        }


        private static bool CompareRows(Row row, SharedStringTable sharedStringTable, List<object> columnConditions)
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

        //columnConditions: <letterID, value>
        private static bool CompareRows(Row row, SharedStringTable sharedStringTable, Dictionary<string, object> columnConditions)
        {
            //Create 'copy'
            //<letterID, condition>
            Dictionary<string, object> leftConditions = new(columnConditions);
            foreach (Cell cell in row.Elements<Cell>())
            {
                object entry = ReadCell(cell, sharedStringTable);

                foreach (var condition in leftConditions)
                {
                    if (condition.Key == GetLetterIDOfCellReference(cell.CellReference.Value) && CompareObjects(entry, condition.Value))
                    {
                        leftConditions.Remove(condition.Key);
                        break;
                    }
                }
            }

            //If an condition is left that means that not all conditions were matched
            return !(leftConditions.Any());
        }
        #endregion

        #region Helper_Methods
        private static bool CompareObjects(object a, object b)
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;

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
            //Normal way in OpenXML. Does NOT work for xlsx (!)
            /*
            else if (cell.DataType is not null && cell.DataType == CellValues.Date)
            {
                if (!String.IsNullOrEmpty(cell?.CellValue?.Text))
                {
                    //Make sure that the double is converted into the correct format (with '.' instead of ',')
                    if (double.TryParse(cell.CellValue.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double dateTimeDouble))
                    {
                        return DateTime.FromOADate(dateTimeDouble);
                    }
                }
            }
            */
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


        protected static Cell GetReferenceCell(Row row, string cellName)
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

        protected static List<string> GetCellReferenceLetters(int count)
        {
            List<string> columnLetterIDs = new();
            for (int i = 1; i <= count; i++)
                columnLetterIDs.Add(ConvertNumberToCellLetters(i));

            return columnLetterIDs;
        }

        protected static string GetLetterIDOfCellReference(string cellReference)
        {
            string letterID = Regex.Match(cellReference, @"[^\d]+").Value;

            return letterID;

            /*
            for (int i = 0; i < cellReference.Length; i++)
            {
                char c = cellReference[i];
                if (Int32.TryParse(c.ToString(), out _))
                    return cellReference[0..i];
            }

            //Already letters only
            return cellReference;
            */
        }

        //return: <letterID, columnName>
        protected static Dictionary<string, string> GetColumnLetterIDsOfColumnNames(ref SpreadsheetDocument spreadsheetDocument, SheetData sheetData, List<string> columnNames, out int rowIndex)
        {
            rowIndex = -1;
            //<letterID, columnName>
            Dictionary<string, string> letterIDsAndColumnNames = new();
            List<object> objectList = new();
            foreach (string strobj in columnNames)
            {
                objectList.Add(strobj);
            }

            Row row = SearchRow(ref spreadsheetDocument, sheetData, objectList);
            if (row is not null)
            {
                foreach (string name in columnNames)
                {
                    string letterID = GetColumnLetterIDsOfColumnNames(ref spreadsheetDocument, row, name, out rowIndex);
                    if (letterID is not null)
                        letterIDsAndColumnNames.Add(letterID, name);
                }
            }

            return letterIDsAndColumnNames;
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
        public static bool WorksheetExists(ref SpreadsheetDocument spreadsheetDocument, string worksheetName, out IEnumerable<Sheet> sheetsIEnum)
        {
            //Search for specific sheet
            sheetsIEnum = spreadsheetDocument?.WorkbookPart?.Workbook?.Descendants<Sheet>()?.Where(s => s.Name == worksheetName);

            return sheetsIEnum.Any();
        }

        /// <summary>
        /// Check if a worksheet does exist in a spreadsheet.
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file that should be opened.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet that should be searched.
        /// </param>
        /// <returns>
        /// True, if worksheet with (parameter) 'worksheetName' does exist, otherwise False.
        /// </returns>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
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
        public static bool CheckPathExist(ref string filepath)
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

        /// <summary>
        /// Checks if a specified ID does exist in a worksheet of the spreadsheet.
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file that should be opened.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet that should be opened.
        /// </param>
        /// <param name="id">
        /// The key is the (so called) 'header-column' 
        /// and the value is the condition a cell should match (the cell should match data-type and value) and that is below the (so called) 'header-column' in the key.
        /// </param>
        /// <returns>
        /// True, if the value in (parameter) 'id' was found below the (so called) 'header-column' below the key of (parameter) 'id'.
        /// </returns>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
        public static bool IsIDOfWorksheet(string filepath, string worksheetName, KeyValuePair<string, object> id)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData, false, false);
            
            string letterID = GetColumnLetterIDsOfColumnNames(ref spreadsheetDocument, sheetData, id.Key, out _);
            if (letterID is null)
                throw new ArgumentException("The Header-Column: " + id.Key + " does not exist.");

            //For easier usage, we take KeyValuePair<columnHeaderName, guid>, but we need the format KeyValuePair<columnLetterID, guid>
            KeyValuePair<string, object> letterIDAndSearchID = new(letterID, id.Value);
            bool found = (SearchRow(ref spreadsheetDocument, sheetData, letterIDAndSearchID) is not null);

            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return found;
        }
        #endregion
        #endregion

        #region GetRowInformation
        protected static int GetRowIDOfCellReference(string cellReference)
        {
            string rowID = Regex.Match(cellReference, @"[\d-]").Value;

            return Convert.ToInt32(rowID);
            
            /*
            for (int i = 0; i < cellReference.Length; i++)
            {
                char c = cellReference[i];
                if (Int32.TryParse(c.ToString(), out _))
                    return Convert.ToInt32(cellReference[i..]);
            }

            //Already letters only
            return Convert.ToInt32(cellReference);
            */
        }


        /// <summary>
        /// Check if a row with all of the entered (so called) 'header-columns' do exist in the worksheet.
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file that should be opened.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet that should be opened.
        /// </param>
        /// <param name="headerColumns">
        /// Every entry represents one (so called) 'header-column' that should be searched.
        /// </param>
        /// <returns>
        /// True, if all (so called) 'header-columns' where found in the same row, otherwise false.
        /// </returns>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
        public static bool CheckHeaderColumnsExist(string filepath, string worksheetName, List<object> headerColumns)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData, false, false);

            bool found = (SearchRow(ref spreadsheetDocument, sheetData, headerColumns) is not null);

            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return found;
        }


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


        //protected static Row SearchRow(ref SpreadsheetDocument spreadsheetDocument, List<string> columnIDs, List<object> columnConditions)
        //{

        //}

        //columnConditions can be type of 'List<object>', 'string', 'Dictionary<string, object>' or 'KeyValuePair<string, object>'
        //objects (values) in columnConditions are the conditions and strings (keys) are columnLetterIDs
        protected static bool CompareRows(Row row, SharedStringTable sharedStringTable, object columnConditions)
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

        //columnConditions: <letterID, value>
        protected static bool CompareRows(Row row, SharedStringTable sharedStringTable, Dictionary<string, object> columnConditions)
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

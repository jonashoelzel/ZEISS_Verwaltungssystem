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
    
    public class WriteExcel
    {
        #region GetCellInformation
        private enum LetterEnum
        {
            A = 1,
            B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y,
            Z = 26
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

        private static Cell GetReferenceCell(Row row, string cellName)
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

        #endregion

        #region Insert
        #region Public_Insert
        public static void Insert(string filepath, string worksheetName, List<object> columnValues)
        {
            List<string> columnLetterIDs = new List<string>();
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

        public static void Insert(string filepath, string worksheetName, List<string> columnNames, List<List<object>> columnValues)
        {

        }

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
            Row row = new Row { RowIndex = UInt32Value.FromUInt32((uint)(++rowCount)) };
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

        #region Public_HelperMethods
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
            bool exists = sheetsIEnum.Any();

            spreadsheetDocument.Close();

            return exists;
        }
        #endregion

        #region CreateSpreadSheetParts
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

        #region CreateSpreadsheet
        private static SpreadsheetDocument OpenSpreadsheetDocument(string filepath, string worksheetName, out SheetData sheetData)
        {
            SpreadsheetDocument spreadsheetDocument;

            if (CheckPathExist(ref filepath))
            {
                spreadsheetDocument = SpreadsheetDocument.Open(filepath, true);

                if (!WorksheetExists(ref spreadsheetDocument, worksheetName, out IEnumerable<Sheet> sheetsIEnum))
                {
                    sheetData = CreateNewWorkbookPartAndGetSheetData(ref spreadsheetDocument, worksheetName);
                }
                else
                {
                    //Open worksheet
                    WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(sheetsIEnum.First().Id);
                    sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                }
            }
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            else
            {
                spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);
                sheetData = CreateNewWorkbookPartAndGetSheetData(ref spreadsheetDocument, worksheetName, false);
            }

            return spreadsheetDocument;
        }

        private static void SaveSpreadsheetDocument(ref SpreadsheetDocument spreadsheetDocument)
        {
            // Save Close the document.
            spreadsheetDocument.Close();
        }
        #endregion

        #region CreateWorkbook
        private static SheetData CreateNewWorkbookPartAndGetSheetData(ref SpreadsheetDocument spreadsheetDocument, string worksheetName, bool append = true)
        {
            if (append)
            {
                // Add a blank WorksheetPart.
                WorksheetPart worksheetPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string relationshipId = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart);
                uint sheetId = GetUniqueSheetID(ref sheets);

                // Append the new worksheet and associate it with the workbook.
                Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = worksheetName };

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
                Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = UInt32Value.FromUInt32(sheetID), Name = worksheetName };

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
        #endregion

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
            if (sharedStringTablePart.SharedStringTable == null)
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
            Cell newCell = new Cell() { CellReference = cellReference };
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
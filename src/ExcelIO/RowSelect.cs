using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

//Make sure the NuGet Package DocumentFormat.OpenXml is installed !!!
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

//Make sure to set under Debuggen->Debugeigenschaften->Anwendung-Zielframework to .NET 5 !!!

namespace Zeiss.PublicationManager.Data.Excel.IO.Read
{
    public class RowSelect : ExcelIOBase
    {       
        //return: <columnName, columnEntries>
        public static Dictionary<string, List<object>> Select(string filepath, string worksheetName, List<string> columnNames)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData, false, false);

            //Try to read SharedStringTable if it exists. If not, make sure to do NOT try to read from it
            SharedStringTable sharedStringTable = spreadsheetDocument?.WorkbookPart?.SharedStringTablePart?.SharedStringTable;

            Dictionary<string, string> letterIDsAndColumnNames = GetColumnLetterIDsOfColumnNames(ref spreadsheetDocument, sheetData, columnNames, out int rowIndex);
            if (!letterIDsAndColumnNames.Any())
                throw new ArgumentException("Unable to find row that matches all ColumnNames in columnNames\n" +
                    "Some of the entered columnNames (Keys) in columnNamesAndValues might not exist or are misspelled");
          
            Dictionary<string, List<object>> result = SelectRow(sheetData, sharedStringTable, letterIDsAndColumnNames, ++rowIndex);
            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return result;
        }

        //return: <columnName, columnEntries>
        //whereColumnNamesAndConditions: <columnName, condition>
        public static Dictionary<string, List<object>> Select(string filepath, string worksheetName, List<string> columnNames, Dictionary<string, object> whereColumnNamesAndConditions)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData, false, false);
            
            //Try to read SharedStringTable if it exists. If not, make sure to do NOT try to read from it
            SharedStringTable sharedStringTable = spreadsheetDocument?.WorkbookPart?.SharedStringTablePart?.SharedStringTable;

            //<letterID, condition>
            Dictionary<string, object> letterIDsAndConditions = ConvertColumnNamesAndValuesToLetterIDsAndValues(ref spreadsheetDocument, sheetData, whereColumnNamesAndConditions);
            if (!letterIDsAndConditions.Any())
                throw new ArgumentException("Unable to find row that matches all names in whereColumnNamesAndConditions.\n" +
                    "Some of the entered columnNames (Keys) in whereColumnNamesAndConditions might not exist or are misspelled");

            Dictionary<string, string> letterIDsAndColumnNames = GetColumnLetterIDsOfColumnNames(ref spreadsheetDocument, sheetData, columnNames, out _);
            if (!letterIDsAndColumnNames.Any())
                throw new ArgumentException("Unable to find row that matches all ColumnNames in columnNames\n" +
                    "Some of the entered columnNames (Keys) in columnNamesAndValues might not exist or are misspelled");

            Dictionary<string, List<object>> result = SelectRow(sheetData, sharedStringTable, letterIDsAndColumnNames, letterIDsAndConditions);
            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return result;
        }

        //return: <columnName, columnEntries>
        //columnLetterIDsAndColumnNames: <columnLetterID, columnName>
        protected static Dictionary<string, List<object>> SelectRow(SheetData sheetData, SharedStringTable sharedStringTable, 
            Dictionary<string, string> columnLetterIDsAndColumnNames, int startRowIndex = 1)
        {
            List<Row> rows = sheetData.Elements<Row>().ToList();
            return SelectRow(sharedStringTable, rows, columnLetterIDsAndColumnNames, startRowIndex);
        }

        //return: <columnName, columnEntries>
        //columnLetterIDsAndColumnNames: <columnLetterID, columnName>
        //letterIDsAndConditions: <letterID, condition>
        protected static Dictionary<string, List<object>> SelectRow(SheetData sheetData, SharedStringTable sharedStringTable,
            Dictionary<string, string> columnLetterIDsAndColumnNames, Dictionary<string, object> letterIDsAndConditions)
        {
            List<Row> rows = SearchRows(sharedStringTable, sheetData, letterIDsAndConditions);
            return SelectRow(sharedStringTable, rows, columnLetterIDsAndColumnNames);
        }

        //return: <columnName, columnEntries>
        //columnLetterIDsAndColumnNames: <columnLetterID, columnName>
        protected static Dictionary<string, List<object>> SelectRow(SharedStringTable sharedStringTable, List<Row> rows, Dictionary<string, string> columnLetterIDsAndColumnNames, int startRowIndex = 1)
        {
            //<columnName, columnEntries>
            Dictionary<string, List<object>> rowsList = new(columnLetterIDsAndColumnNames.Select(x => new KeyValuePair<string, List<object>>(x.Value, new())));

            foreach (Row row in rows)
            {
                //Only read rows after index => (could be used) to prevent reading header columns
                if (row?.RowIndex?.Value >= startRowIndex)
                {
                    foreach (Cell cell in row.Elements<Cell>())
                    {
                        string letterIDOfReference = GetLetterIDOfCellReference(cell.CellReference.Value);
                        if (columnLetterIDsAndColumnNames.ContainsKey(letterIDOfReference))
                        {
                            string columnName = columnLetterIDsAndColumnNames[letterIDOfReference];
                            if (rowsList.ContainsKey(columnName))
                                rowsList[columnName].Add(ReadCell(cell, sharedStringTable));
                        }
                    }
                }
            }

            return rowsList;
        }
    }
}

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
        #region SelectAsColumns
        /// <summary>
        /// Reads and returns all values below of entered (so-called) 'header-columns' in (parameter) 'columnNames'.
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file that should be opened.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet in the *.xlsx file that should be read.
        /// </param>
        /// <param name="columnNames">
        /// Names of all columns that should be used to identify the header, so that it'll only read values that are below those headers.
        /// </param>
        /// <returns>
        /// Returns a Dictionary, where the keys are the entered (parameter) 'columnNames' and the values of those keys are all read values that are below of those (so called) 'header-column'.
        /// </returns>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
        public static Dictionary<string, List<object>> SelectAsColumns(string filepath, string worksheetName, List<string> columnNames)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData, false, false);

            //Try to read SharedStringTable if it exists. If not, make sure to do NOT try to read from it
            SharedStringTable sharedStringTable = spreadsheetDocument?.WorkbookPart?.SharedStringTablePart?.SharedStringTable;

            Dictionary<string, string> letterIDsAndColumnNames = GetColumnLetterIDsOfColumnNames(ref spreadsheetDocument, sheetData, columnNames, out int rowIndex);
            if (!letterIDsAndColumnNames.Any())
                throw new ArgumentException("Unable to find row that matches all ColumnNames in columnNames\n" +
                    "Some of the entered columnNames (Keys) in columnNamesAndValues might not exist or are misspelled");

            Dictionary<string, List<object>> result = SelectRowAsColumns(sheetData, sharedStringTable, letterIDsAndColumnNames, ++rowIndex);
            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return result;
        }

        /// <summary>
        /// Reads and returns all values of rows that do match all of the entered conditions in (parameter) 'whereColumnNamesAndConditions' 
        /// below of entered (so-called) 'header-columns' in (parameter) 'columnNames'.
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file that should be opened.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet in the *.xlsx file that should be read.
        /// </param>
        /// <param name="columnNames">
        /// Names of all columns that should be used to identify the header, so that it'll only read values that are below those headers.
        /// </param>
        /// <param name="whereColumnNamesAndConditions">
        /// Every KeyValuePair represents one condition, where the key is the (so called) 'header-column' 
        /// and the value is the condition a cell should match (the cell should match data-type and value) and that is below the (so called) 'header-column' in the key.
        /// </param>
        /// <returns>
        /// Returns a Dictionary, where the keys are the entered (parameter) 'columnNames' and the values of those keys are all read values that are below of those (so called) 'header-column'.
        /// </returns>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
        public static Dictionary<string, List<object>> SelectWhereAsColumns(string filepath, string worksheetName, List<string> columnNames,
            Dictionary<string, object> whereColumnNamesAndConditions)
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

            Dictionary<string, List<object>> result = SelectRowAsColumns(sheetData, sharedStringTable, letterIDsAndColumnNames, letterIDsAndConditions);
            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return result;
        }

        //return: <columnName, columnEntries>
        //columnLetterIDsAndColumnNames: <columnLetterID, columnName>
        protected static Dictionary<string, List<object>> SelectRowAsColumns(SheetData sheetData, SharedStringTable sharedStringTable,
            Dictionary<string, string> columnLetterIDsAndColumnNames, int startRowIndex = 1)
        {
            List<Row> rows = sheetData.Elements<Row>().ToList();
            return SelectRowAsColumns(sharedStringTable, rows, columnLetterIDsAndColumnNames, startRowIndex);
        }

        //return: <columnName, columnEntries>
        //columnLetterIDsAndColumnNames: <columnLetterID, columnName>
        //letterIDsAndConditions: <letterID, condition>
        protected static Dictionary<string, List<object>> SelectRowAsColumns(SheetData sheetData, SharedStringTable sharedStringTable,
            Dictionary<string, string> columnLetterIDsAndColumnNames, Dictionary<string, object> letterIDsAndConditions)
        {
            List<Row> rows = SearchRows(sharedStringTable, sheetData, letterIDsAndConditions);
            return SelectRowAsColumns(sharedStringTable, rows, columnLetterIDsAndColumnNames);
        }

        //return: <columnName, columnEntries>
        //columnLetterIDsAndColumnNames: <columnLetterID, columnName>
        protected static Dictionary<string, List<object>> SelectRowAsColumns(SharedStringTable sharedStringTable, List<Row> rows,
            Dictionary<string, string> columnLetterIDsAndColumnNames, int startRowIndex = 1)
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
        #endregion

        #region SelectAsRows
        /// <summary>
        /// Reads and returns all values below of entered (so-called) 'header-columns' in (parameter) 'columnNames'.
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file that should be opened.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet in the *.xlsx file that should be read.
        /// </param>
        /// <param name="columnNames">
        /// Names of all columns that should be used to identify the header, so that it'll only read values that are below those headers.
        /// </param>
        /// <returns>
        /// Returns a Dictionary, where the keys are the entered (parameter) 'columnNames' and the values of those keys are all read values that are below of those (so called) 'header-column'.
        /// </returns>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
        public static List<Dictionary<string, object>> SelectAsRows(string filepath, string worksheetName, List<string> columnNames)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData, false, false);

            //Try to read SharedStringTable if it exists. If not, make sure to do NOT try to read from it
            SharedStringTable sharedStringTable = spreadsheetDocument?.WorkbookPart?.SharedStringTablePart?.SharedStringTable;

            Dictionary<string, string> letterIDsAndColumnNames = GetColumnLetterIDsOfColumnNames(ref spreadsheetDocument, sheetData, columnNames, out int rowIndex);
            if (!letterIDsAndColumnNames.Any())
                throw new ArgumentException("Unable to find row that matches all ColumnNames in columnNames\n" +
                    "Some of the entered columnNames (Keys) in columnNamesAndValues might not exist or are misspelled");

            List<Dictionary<string, object>> result = SelectRowAsRows(sheetData, sharedStringTable, letterIDsAndColumnNames, ++rowIndex);
            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return result;
        }

        /// <summary>
        /// Reads and returns all values of rows that do match all of the entered conditions in (parameter) 'whereColumnNamesAndConditions' 
        /// below of entered (so-called) 'header-columns' in (parameter) 'columnNames'.
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file that should be opened.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet in the *.xlsx file that should be read.
        /// </param>
        /// <param name="columnNames">
        /// Names of all columns that should be used to identify the header, so that it'll only read values that are below those headers.
        /// </param>
        /// <param name="whereColumnNamesAndConditions">
        /// Every KeyValuePair represents one condition, where the key is the (so called) 'header-column' 
        /// and the value is the condition a cell should match (the cell should match data-type and value) and that is below the (so called) 'header-column' in the key.
        /// </param>
        /// <returns>
        /// Returns a Dictionary, where the keys are the entered (parameter) 'columnNames' and the values of those keys are all read values that are below of those (so called) 'header-column'.
        /// </returns>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
        public static List<Dictionary<string, object>> SelectWhereAsRows(string filepath, string worksheetName, List<string> columnNames,
            Dictionary<string, object> whereColumnNamesAndConditions)
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

            List<Dictionary<string, object>> result = SelectRowAsRows(sheetData, sharedStringTable, letterIDsAndColumnNames, letterIDsAndConditions);
            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return result;
        }

        //return: <columnName, columnEntry>
        //columnLetterIDsAndColumnNames: <columnLetterID, columnName>
        protected static List<Dictionary<string, object>> SelectRowAsRows(SheetData sheetData, SharedStringTable sharedStringTable,
            Dictionary<string, string> columnLetterIDsAndColumnNames, int startRowIndex = 1)
        {
            List<Row> rows = sheetData.Elements<Row>().ToList();
            return SelectRowAsRows(sharedStringTable, rows, columnLetterIDsAndColumnNames, startRowIndex);
        }

        //return: <columnName, columnEntry>
        //columnLetterIDsAndColumnNames: <columnLetterID, columnName>
        //letterIDsAndConditions: <letterID, condition>
        protected static List<Dictionary<string, object>> SelectRowAsRows(SheetData sheetData, SharedStringTable sharedStringTable,
            Dictionary<string, string> columnLetterIDsAndColumnNames, Dictionary<string, object> letterIDsAndConditions)
        {
            List<Row> rows = SearchRows(sharedStringTable, sheetData, letterIDsAndConditions);
            return SelectRowAsRows(sharedStringTable, rows, columnLetterIDsAndColumnNames);
        }

        //return: <columnName, columnEntry>
        //columnLetterIDsAndColumnNames: <columnLetterID, columnName>
        protected static List<Dictionary<string, object>> SelectRowAsRows(SharedStringTable sharedStringTable, List<Row> rows,
            Dictionary<string, string> columnLetterIDsAndColumnNames, int startRowIndex = 1)
        {
            //<columnName, columnEntry>
            List<Dictionary<string, object>> rowsList = new();

            foreach (Row row in rows)
            {
                //Only read rows after index => (could be used) to prevent reading header columns
                if (row?.RowIndex?.Value >= startRowIndex)
                {
                    Dictionary<string, object> columnsNameAndValues = new();

                    foreach (Cell cell in row.Elements<Cell>())
                    {
                        string letterIDOfReference = GetLetterIDOfCellReference(cell.CellReference.Value);
                        if (columnLetterIDsAndColumnNames.ContainsKey(letterIDOfReference))
                        {
                            string columnName = columnLetterIDsAndColumnNames[letterIDOfReference];

                            columnsNameAndValues.Add(columnName, ReadCell(cell, sharedStringTable));
                        }
                    }

                    rowsList.Add(columnsNameAndValues);
                }
            }

            return rowsList;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Zeiss.PublicationManager.Data.DataSet;
using Zeiss.PublicationManager.Data.Excel;

namespace Zeiss.PublicationManager.Data.Excel.IO.Write
{
    public class RowInsert : WriteExcel
    {
        #region Insert
        #region Public_Insert
        //Can be used to create Headers

        /// <summary>
        /// Inserts all values of (parameter) 'columnValues' into a new row.
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file where the new row should be inserted.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet in the *.xlsx file where the new row should be inserted.
        /// </param>
        /// <param name="columnValues">
        /// Every value of (parameter) 'columnValues' is the value of a new cell in the new row.
        /// </param>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
        public static void Insert(string filepath, string worksheetName, List<object> columnValues)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData);
            
            List<string> columnLetterIDs = GetCellReferenceLetters(columnValues.Count);
            if (!columnLetterIDs.Any())
                throw new ArgumentException("Unable to create row with values.");

            Dictionary<string, object> letterIDsAndValues = new();
            for (int i = 0; i < columnLetterIDs.Count; i++)
                letterIDsAndValues.Add(columnLetterIDs[i], columnValues[i]);

            InsertRow(ref spreadsheetDocument, ref sheetData, letterIDsAndValues);
            SaveSpreadsheetDocument(ref spreadsheetDocument);
        }

        /// <summary>
        /// Inserts all values of (parameter) 'columnNamesAndValues' into a new row.
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file where the new row should be inserted.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet in the *.xlsx file where the new row should be inserted.
        /// </param>
        /// <param name="columnNamesAndValues">
        /// Every KeyValuePair represents one cell with value, where the key is the (so called) 'header-column' where the cell should be inserted below this (so called) 'header-column'
        /// and the value is the value of the cell.
        /// </param>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
        public static void Insert(string filepath, string worksheetName, Dictionary<string, object> columnNamesAndValues)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData);
            //<letterID, value>
            Dictionary<string, object> letterIDsAndValues = ConvertColumnNamesAndValuesToLetterIDsAndValues(ref spreadsheetDocument, sheetData, columnNamesAndValues);
            if (!letterIDsAndValues.Any())
                throw new ArgumentException("Unable to find row that matches all columnNames in columnNamesAndValues.\n" +
                    "Some of the entered columnNames (Keys) in columnNamesAndValues might not exist or are misspelled");

            InsertRow(ref spreadsheetDocument, ref sheetData, letterIDsAndValues);
            SaveSpreadsheetDocument(ref spreadsheetDocument);
        }
        #endregion

        #region Private_Insert
        
        //letterIDsAndValues: <letterID, value>
        private static void InsertRow(ref SpreadsheetDocument spreadsheetDocument, ref SheetData sheetData, Dictionary<string, object> letterIDsAndValues)
        {
            //Create new row after the last low
            //We use this instead of .Count() in case of rows where deleted
            uint rowIndex = 1;
            if (sheetData.Elements<Row>()?.Any() ?? false)
                rowIndex = sheetData.Elements<Row>().Max(x => x.RowIndex.Value) + 1;

            Row row = new() { RowIndex = UInt32Value.FromUInt32((rowIndex)) };
            sheetData.Append(row);

            foreach (var letterIDAndValue in letterIDsAndValues)
            {
                //Format XX00
                string cellReference = letterIDAndValue.Key + rowIndex;
                //<cellReference, value>
                CreateCell(ref spreadsheetDocument, ref row, new KeyValuePair<string, object>(cellReference, letterIDAndValue.Value));
            }
        }
        #endregion
        #endregion
    }
}

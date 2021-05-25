using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Zeiss.PublicationManager.Data.Excel.IO.Write
{
    public class RowDelete : WriteExcel
    {
        /// <summary>
        /// Deletes all rows that do match all the conditions in (parameter) 'whereColumnNamesAndValues'.
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file where the rows should be deleted.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet in the *.xlsx file where the rows should be deleted.
        /// </param>
        /// <param name="whereColumnNamesAndConditions">
        /// Every KeyValuePair represents one condition, where the key is the (so called) 'header-column' 
        /// and the value is the condition a cell should match (the cell should match data-type and value) and that is below the (so called) 'header-column' in the key.
        /// </param>
        /// <returns>
        /// Number of deleted rows.
        /// </returns>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
        public static int Delete(string filepath, string worksheetName, Dictionary<string, object> whereColumnNamesAndConditions)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData);
            //<letterID, condition>
            Dictionary<string, object> letterIDsAndConditions = ConvertColumnNamesAndValuesToLetterIDsAndValues(ref spreadsheetDocument, sheetData, whereColumnNamesAndConditions);
            if (!letterIDsAndConditions.Any())
                throw new ArgumentException("Unable to find row that matches all names in whereColumnNamesAndConditions.\n" +
                    "Some of the entered columnNames (Keys) in whereColumnNamesAndConditions might not exist or are misspelled");

            int rowsChanged = DeleteRow(ref spreadsheetDocument, ref sheetData, letterIDsAndConditions);
            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return rowsChanged;
        }

        //letterIDsAndValues: <letterID, condition>
        private static int DeleteRow
            (ref SpreadsheetDocument spreadsheetDocument, ref SheetData sheetData, Dictionary<string, object> letterIDsAndConditions)
        {
            //Search for a row that matches the conditions in letterIDsAndConditions.
            List<Row> rows = SearchRows(ref spreadsheetDocument, sheetData, letterIDsAndConditions);
            int countRows = rows.Count;

            RemoveRow(rows);

            return countRows;
        }



        /// <summary>
        /// Deletes all rows that do match all the conditions in (parameter) 'whereColumnNamesAndValues'.
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file where the rows should be deleted.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet in the *.xlsx file where the rows should be deleted.
        /// </param>
        /// <param name="whereColumnNamesAndConditions">
        /// Every KeyValuePair represents one condition, where the key is the (so called) 'header-column' 
        /// and the value is the condition a cell should match (the cell should match data-type and value) and that is below the (so called) 'header-column' in the key.
        /// </param>
        /// <returns>
        /// Number of deleted rows.
        /// </returns>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
        public static int DeleteAdvanced(string filepath, string worksheetName, Dictionary<string, object> whereColumnNamesAndConditions)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData);
            //<letterID, condition>
            Dictionary<string, object> letterIDsAndConditions = ConvertColumnNamesAndValuesToLetterIDsAndValues(ref spreadsheetDocument, sheetData, whereColumnNamesAndConditions);
            if (!letterIDsAndConditions.Any())
                throw new ArgumentException("Unable to find row that matches all names in whereColumnNamesAndConditions.\n" +
                    "Some of the entered columnNames (Keys) in whereColumnNamesAndConditions might not exist or are misspelled");

            int rowsChanged = DeleteRowAdvanced(ref spreadsheetDocument, ref sheetData, letterIDsAndConditions);
            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return rowsChanged;
        }


        //letterIDsAndValues: <letterID, condition>
        private static int DeleteRowAdvanced
            (ref SpreadsheetDocument spreadsheetDocument, ref SheetData sheetData, Dictionary<string, object> letterIDsAndConditions)
        {
            //Search for a row that matches the conditions in letterIDsAndConditions.
            List<Row> rows = SearchRows(ref spreadsheetDocument, sheetData, letterIDsAndConditions);
            int countRows = rows.Count;

            RemoveRowAdvanced(sheetData, rows);

            return countRows;
        }
    }
}

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
    public class RowUpdate : WriteExcel
    {
        /// <summary>
        /// Updates all rows with the entered values in (parameter) 'updateColumnsAndNewValues' that do match all the conditions in (parameter) 'whereColumnNamesAndConditions).
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file where the rows should be updated.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet in the *.xlsx file where the rows should be updated.
        /// </param>
        /// <param name="whereColumnNamesAndConditions">
        /// Every KeyValuePair represents one condition, where the key is the (so called) 'header-column' 
        /// and the value is the condition a cell should match (the cell should match data-type and value) and that is below the (so called) 'header-column' in the key.
        /// </param>
        /// <param name="updateColumnsAndNewValues">
        /// Every KeyValuePair represents one cell with value, where the key is the (so called) 'header-column' where the cells that should be updated are below those (so called) 'header-columns'
        /// and the value is the new value of the cell.
        /// </param>
        /// <returns>
        /// Number of updated rows.
        /// </returns>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
        public static int Update(string filepath, string worksheetName, Dictionary<string, object> whereColumnNamesAndConditions, Dictionary<string, object> updateColumnsAndNewValues)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData);
            //<letterID, condition>
            Dictionary<string, object> letterIDsAndConditions = ConvertColumnNamesAndValuesToLetterIDsAndValues(ref spreadsheetDocument, sheetData, whereColumnNamesAndConditions);
            if (!letterIDsAndConditions.Any())
                throw new ArgumentException("Unable to find row that matches all names in whereColumnNamesAndConditions.\n" +
                    "Some of the entered columnNames (Keys) in whereColumnNamesAndConditions might not exist or are misspelled");

            //<letterID, newValue>
            Dictionary<string, object> letterIDsAndNewValue = ConvertColumnNamesAndValuesToLetterIDsAndValues(ref spreadsheetDocument, sheetData, updateColumnsAndNewValues);
            if (!letterIDsAndNewValue.Any())
                throw new ArgumentException("Unable to find row that matches all names in updateColumnAndNewValues.\n" +
                    "Some of the entered columnNames (Keys) in updateColumnAndNewValues might not exist or are misspelled");

            int rowsChanged = UpdateRow(ref spreadsheetDocument, ref sheetData, letterIDsAndConditions, letterIDsAndNewValue);
            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return rowsChanged;
        }

        //letterIDsAndConditions: <letterID, condition>
        //updateColumnAndNewValue: <columnName, newValue>
        private static int UpdateRow
            (ref SpreadsheetDocument spreadsheetDocument, ref SheetData sheetData, Dictionary<string, object> letterIDsAndConditions, Dictionary<string, object> letterIDsAndNewValues)
        {
            //Search for a row that matches the conditions in letterIDsAndConditions.
            List<Row> rows = SearchRows(ref spreadsheetDocument, sheetData, letterIDsAndConditions);
            int countRows = rows.Count;
            //!!! DO NOT USE (!!!) 'foreach', because we need the original references (and 'foreach' creates copy) !!!
            for (int i = 0; i < countRows; i++)
            {
                List<Cell> cells = rows[i].Elements<Cell>().ToList();
                for (int j = 0; j < cells.Count; j++)
                {
                    Cell cell = cells[j];
                    string letterID = GetLetterIDOfCellReference(cell.CellReference.Value);
                    //If this cell at letterID should be updated
                    if (letterIDsAndNewValues.ContainsKey(letterID))
                        UpdateCell(ref spreadsheetDocument, ref cell, letterIDsAndNewValues[letterID]);
                }
            }

            return countRows;
        }
    }
}

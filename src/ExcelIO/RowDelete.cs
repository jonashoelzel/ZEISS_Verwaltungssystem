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
        //whereColumnNamesAndConditions: <columnName, condition>
        //updateColumnAndNewValue: <columnName, newValue>
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



        //whereColumnNamesAndConditions: <columnName, condition>
        //updateColumnAndNewValue: <columnName, newValue>
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

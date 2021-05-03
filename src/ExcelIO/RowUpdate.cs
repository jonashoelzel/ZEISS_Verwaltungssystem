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
        //whereColumnNamesAndConditions: <columnName, condition>
        //updateColumnAndNewValue: <columnName, newValue>
        public static int Update(string filepath, string worksheetName, Dictionary<string, object> whereColumnNamesAndConditions, Dictionary<string, object> updateColumnAndNewValues)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData);
            //<letterID, value>
            Dictionary<string, object> letterIDsAndConditions = ConvertColumnNamesAndValuesToLetterIDsAndValues(ref spreadsheetDocument, sheetData, whereColumnNamesAndConditions);
            Dictionary<string, object> letterIDsAndNewValue = ConvertColumnNamesAndValuesToLetterIDsAndValues(ref spreadsheetDocument, sheetData, updateColumnAndNewValues);
            int rowsChanged = UpdateRow(ref spreadsheetDocument, sheetData, letterIDsAndConditions, letterIDsAndNewValue);
            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return rowsChanged;
        }

        //letterIDsAndValues: <letterID, condition>
        //updateColumnAndNewValue: <columnName, newValue>
        private static int UpdateRow
            (ref SpreadsheetDocument spreadsheetDocument, SheetData sheetData, Dictionary<string, object> letterIDsAndConditions, Dictionary<string, object> letterIDsAndNewValues)
        {
            List<Row> rows = SearchRows(ref spreadsheetDocument, sheetData, letterIDsAndConditions);
            int countRows = rows.Count;
            //!!! DO NOT USE (!!!) 'foreach', because we need the original references (and 'foreach' creates copy) !!!
            for (int i = 0; i < countRows; i++)
            {
                List<Cell> cells = rows[i].Elements<Cell>().ToList();
                for (int j = 0; j < cells.Count; j++)
                {
                    Cell cell = cells[i];
                    string letterID = GetLetterIDOfCellReference(cell.CellReference);
                    //If this cell at letterID should be updated
                    if (letterIDsAndNewValues.ContainsKey(letterID))
                        UpdateCell(ref spreadsheetDocument, ref cell, letterIDsAndNewValues[letterID]);
                }
            }

            return countRows;
        }
    }
}

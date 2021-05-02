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
        public static void Insert(string filepath, string worksheetName, Dictionary<string, object> columnNamesAndValues)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData);
            Dictionary<string, object> letterIDsAndValues = ConvertColumnNamesAndValuesToLetterIDsAndValues(ref spreadsheetDocument, sheetData, columnNamesAndValues);
            InsertRow(ref spreadsheetDocument, sheetData, letterIDsAndValues);
            SaveSpreadsheetDocument(ref spreadsheetDocument);
        }
        #endregion

        #region Private_Insert
        private static void InsertRow(ref SpreadsheetDocument spreadsheetDocument, SheetData sheetData, Dictionary<string, object> letterIDsAndValues)
        {
            //Create new row
            int rowCount = sheetData.Elements<Row>().Count();
            Row row = new() { RowIndex = UInt32Value.FromUInt32((uint)(++rowCount)) };
            sheetData.Append(row);

            foreach (var letterIDAndValue in letterIDsAndValues)
            {
                //Format XX00
                string cellReference = letterIDAndValue.Key + rowCount;
                CreateCell(ref spreadsheetDocument, row, new KeyValuePair<string, object>(cellReference, letterIDAndValue.Value));
            }
        }
        #endregion
        #endregion
    }
}

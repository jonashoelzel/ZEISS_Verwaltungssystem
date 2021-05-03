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

namespace Zeiss.PublicationManager.Data.Excel.IO.Write.Legacy
{
    public class LegacyRowInsert : RowInsert
    {
        #region Insert
        #region Public_Insert
        public static void Insert(string filepath, string worksheetName, List<string> columnNames, List<object> columnValues)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData);
            List<string> columnLetterIDs = IO.Legacy.LegacyExcelIOBase.GetColumnLetterIDsOfColumnNames(ref spreadsheetDocument, sheetData, columnNames, out _);
            InsertRow(ref spreadsheetDocument, sheetData, columnLetterIDs, columnValues);
            SaveSpreadsheetDocument(ref spreadsheetDocument);
        }

        //
        //public static void Insert(string filepath, string worksheetName, string startColumnID, List<List<object>> columnValues)
        //{
        //
        //}


        //public static void Insert(string filepath, string worksheetName, string startColumnID, List<string> columnNames, List<List<object>> columnValues)
        //{

        //}

        public static void InsertRow(ref SpreadsheetDocument spreadsheetDocument, SheetData sheetData, List<string> columnLetterIDs, List<object> columnValues)
        {
            //Create new row
            int rowCount = sheetData.Elements<Row>().Count();
            Row row = new() { RowIndex = UInt32Value.FromUInt32((uint)(++rowCount)) };
            sheetData.Append(row);

            for (int i = 0; i < columnValues.Count; i++)
            {
                //Format XX00
                string cellReference = columnLetterIDs[i] + rowCount;
                IO.Write.Legacy.LegacyWriteExcel.CreateCell(ref spreadsheetDocument, row, cellReference, columnValues[i]);
            }
        }

        #endregion
        #endregion
    }
}

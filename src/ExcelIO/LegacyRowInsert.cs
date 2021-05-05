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
            if (!columnLetterIDs.Any())
                throw new ArgumentException("Unable to find row that matches all columnNames in columnNames.\n" +
                    "Some of the entered columnNames (Keys) in columnNames might not exist or are misspelled");

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
            //Create new row after the last low
            //We use this instead of .Count() in case of rows where deleted
            uint rowIndex = (sheetData.Elements<Row>().Max(x => x.RowIndex.Value)) + 1;
            Row row = new() { RowIndex = UInt32Value.FromUInt32((uint)(++rowIndex)) };
            sheetData.Append(row);

            for (int i = 0; i < columnValues.Count; i++)
            {
                //Format XX00
                string cellReference = columnLetterIDs[i] + rowIndex;
                IO.Write.Legacy.LegacyWriteExcel.CreateCell(ref spreadsheetDocument, row, cellReference, columnValues[i]);
            }
        }

        #endregion
        #endregion
    }
}

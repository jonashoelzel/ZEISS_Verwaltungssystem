﻿using System;
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
        //columnNamesAndValues <columnName, value>
        public static void Insert(string filepath, string worksheetName, Dictionary<string, object> columnNamesAndValues)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData);
            //<letterID, value>
            Dictionary<string, object> letterIDsAndValues = ConvertColumnNamesAndValuesToLetterIDsAndValues(ref spreadsheetDocument, sheetData, columnNamesAndValues);
            InsertRow(ref spreadsheetDocument, sheetData, letterIDsAndValues);
            SaveSpreadsheetDocument(ref spreadsheetDocument);
        }

        public static void Insert(string filepath, string worksheetName, List<string> columnNames, List<object> columnValues)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData);
            List<string> columnLetterIDs = GetColumnLetterIDsOfColumnNames(ref spreadsheetDocument, sheetData, columnNames);
            InsertRow(ref spreadsheetDocument, sheetData, columnLetterIDs, columnValues);
            SaveSpreadsheetDocument(ref spreadsheetDocument);
        }

        public static void Insert(string filepath, string worksheetName, Dictionary<string, object> attributesDict)
        {
            var columnNames = new List<string>();
            var columnValues = new List<object>();
            foreach (var attribute in attributesDict)
            {
                columnNames.Add(attribute.Key);
                columnValues.Add(attribute.Value);
            }
            var spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData);

            var columnLetterIDs = GetColumnLetterIDsOfColumnNames(ref spreadsheetDocument, sheetData, columnNames);

            
            InsertRow(ref spreadsheetDocument, sheetData, columnLetterIDs, columnValues);
            SaveSpreadsheetDocument(ref spreadsheetDocument);
        }

        #endregion

        #region Private_Insert
        //letterIDsAndValues: <letterID< value>
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
                //<cellReference, value>
                CreateCell(ref spreadsheetDocument, row, new KeyValuePair<string, object>(cellReference, letterIDAndValue.Value));
            }
        }
        #endregion
        #endregion
    }
}

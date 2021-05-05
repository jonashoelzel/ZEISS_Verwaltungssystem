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
    //NOTE: When we use with Console, all members have to be static !!!
    public class RowSelect : ExcelIOBase
    {       
        //return: <columnName, columnEntries>
        public static Dictionary<string, List<object>> Select(string filepath, string worksheetName, List<string> columnNames)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData, false, false);

            //Try to read SharedStringTable if it exists. If not, make sure to do NOT try to read from it
            SharedStringTable sharedStringTable = spreadsheetDocument?.WorkbookPart?.SharedStringTablePart?.SharedStringTable;

            Dictionary<string, string> letterIDsAndColumnNames = GetColumnLetterIDsOfColumnNames(ref spreadsheetDocument, sheetData, columnNames, out int rowIndex);
            if (!letterIDsAndColumnNames.Any())
                throw new ArgumentException("Unable to find row that matches all ColumnNames in columnNames\n" +
                    "Some of the entered columnNames (Keys) in columnNamesAndValues might not exist or are misspelled");

            return Reader(sheetData, sharedStringTable, letterIDsAndColumnNames, ++rowIndex);
        }

        //return: <columnName, columnEntries>
        //columnLetterIDsAndColumnNames: <columnLetterID, columnName>
        protected static Dictionary<string, List<object>> Reader(SheetData sheetData, SharedStringTable sharedStringTable, 
            Dictionary<string, string> columnLetterIDsAndColumnNames, int startRowIndex = 1)
        {
            //<columnName, columnEntries>
            Dictionary<string, List<object>> rowsList = new(columnLetterIDsAndColumnNames.Select(x => new KeyValuePair<string, List<object>>(x.Value, new())));

            foreach (Row row in sheetData.Elements<Row>())
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
    }
}

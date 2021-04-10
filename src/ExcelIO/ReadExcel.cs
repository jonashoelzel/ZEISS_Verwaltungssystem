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
    public class ReadExcel : ExcelIOBase
    {
        //We using DOM instead of SAX for easier reading management
        //public static List<List<string>> OpenExcelDOM(string filepath)
        //{
        //    // Open the document for editing.
        //    using (SpreadsheetDocument spreadsheetDocument =
        //        SpreadsheetDocument.Open(filepath, true))
        //    {
        //        //Open Excel
        //        WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
        //        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
        //        SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

        //        //Try to read SharedStringTable if it exists. If not, make sure to do NOT try to read from it
        //        SharedStringTable sharedStringTable = spreadsheetDocument?.WorkbookPart?.SharedStringTablePart?.SharedStringTable;

        //        List<List<string>> entries = new List<List<string>>();

        //        foreach (Row row in sheetData.Elements<Row>())
        //        {
        //            List<string> entry = new List<string>();

        //            foreach (Cell cell in row.Elements<Cell>())
        //            {
        //                //Make sure that the Excel has a SharedStringTable, the Cell has a DataType and is a String
        //                if (cell.DataType is not null && sharedStringTable is not null && cell.DataType == CellValues.SharedString)
        //                {
        //                    var cellValue = cell.InnerText;
        //                    entry.Add(sharedStringTable.ElementAt(Int32.Parse(cellValue)).InnerText);
        //                }
        //                //DataType is null
        //                else
        //                {
        //                    //Check if StyleIndex is a Date Format
        //                    int styleIndex = -1;
        //                    if (Int32.TryParse(cell.StyleIndex?.InnerText, out styleIndex))
        //                    {
        //                        //Standard date format
        //                        if (styleIndex >= 12 && styleIndex <= 22 
        //                            //Formatted date format
        //                            || styleIndex >= 165 && styleIndex <= 180
        //                            //Number formats that can be interpreted as a number
        //                            || styleIndex >= 1 && styleIndex <= 5)
        //                        {
        //                            double dateTimeDouble;
        //                            if (double.TryParse(cell.CellValue.Text, out dateTimeDouble))
        //                            {
        //                                entry.Add(DateTime.FromOADate(dateTimeDouble).ToString("dd/MM/yyyy"));
        //                            }
        //                        }
        //                    }
        //                    //Default is number (if StyleIndex is null or any other StyleIndex
        //                    else
        //                        entry.Add(cell.CellValue.Text);
        //                }                                            
        //            }

        //            entries.Add(entry);
        //        }

        //        return entries;
        //    }
        //}

        public static List<List<object>> ReadSpreadsheet(string filepath, string worksheetName)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData, false, false);

            //Try to read SharedStringTable if it exists. If not, make sure to do NOT try to read from it
            SharedStringTable sharedStringTable = spreadsheetDocument?.WorkbookPart?.SharedStringTablePart?.SharedStringTable;

            List<List<object>> rowsList = new List<List<object>>();

            foreach (Row row in sheetData.Elements<Row>())
            {
                List<object> rowList = new List<object>();

                foreach (Cell cell in row.Elements<Cell>())
                {
                    rowList.Add(ReadCell(cell, sharedStringTable));
                }

                rowsList.Add(rowList);
            }

            //This will only close the document to release resources
            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return rowsList;
        }        
    }
}

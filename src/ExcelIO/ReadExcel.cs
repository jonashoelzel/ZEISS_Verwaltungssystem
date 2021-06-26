using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


namespace Zeiss.PublicationManager.Data.Excel.IO.Read
{
    public abstract class ReadExcel : ExcelIOBase
    {
        //return: <letterID, columnName>
        protected static Dictionary<string, string> GetColumnLetterIDsOfColumnNames(ref SpreadsheetDocument spreadsheetDocument, SheetData sheetData, List<string> columnNames, out int rowIndex)
        {
            rowIndex = -1;
            //<letterID, columnName>
            Dictionary<string, string> letterIDsAndColumnNames = new();
            List<object> objectList = new();
            foreach (string strobj in columnNames)
            {
                objectList.Add(strobj);
            }

            Row row = SearchRow(ref spreadsheetDocument, sheetData, objectList);
            if (row is not null)
            {
                foreach (string name in columnNames)
                {
                    string letterID = GetColumnLetterIDsOfColumnNames(ref spreadsheetDocument, row, name, out rowIndex);
                    if (letterID is not null)
                        letterIDsAndColumnNames.Add(letterID, name);
                }
            }

            return letterIDsAndColumnNames;
        }
    }
}

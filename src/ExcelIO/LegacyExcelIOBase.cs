using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Zeiss.PublicationManager.Data.Excel;

namespace Zeiss.PublicationManager.Data.Excel.IO.Legacy
{
    public class LegacyExcelIOBase : ExcelIOBase
    {
        public static new List<string> GetColumnLetterIDsOfColumnNames(ref SpreadsheetDocument spreadsheetDocument, SheetData sheetData, List<string> columnNames, out int rowIndex)
        {
            rowIndex = -1;
            List<string> letterIDs = new();
            List<object> objectList = new();
            foreach (string strobj in columnNames)
            {
                objectList.Add(strobj);
            }

            Row row = ExcelIOBase.SearchRow(ref spreadsheetDocument, sheetData, objectList);
            if (row is not null)
            {
                foreach (string name in columnNames)
                {
                    letterIDs.Add(
                        GetColumnLetterIDsOfColumnNames(
                            ref spreadsheetDocument,
                            row,
                            name, out rowIndex));
                }
            }        

            return letterIDs;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Zeiss.PublicationManager.Data.Excel.IO.Write.Legacy
{
    public class LegacyWriteExcel : WriteExcel
    {
        public static void CreateCell(ref SpreadsheetDocument spreadsheetDocument, Row row, string cellReference, object cellEntry)
        {
            //Get reference cell
            Cell referenceCell = GetReferenceCell(row, cellReference);
            if (referenceCell is null)
                throw new ArgumentException("The Cell-Reference: " + referenceCell + " is invalid.");
            // Add the cell to the cell table at cellReference.
            Cell newCell = new() { CellReference = cellReference };
            row.InsertBefore(newCell, referenceCell);

            switch (cellEntry)
            {
                case string objstr:
                    AddSharedString(ref spreadsheetDocument, ref newCell, objstr);
                    break;

                case DateTime objdate:
                    //Normal way. Does NOT work for xlsx (!)
                    //string strdate = objdate.ToOADate().ToString();
                    //newCell.DataType = CellValues.Date;
                    //newCell.CellValue = new CellValue(strdate);

                    //"StyleIndex" is "1", because we added a new stylesheet (index 0 would be default) with "NumberFormatId=14"
                    //is in the 2nd item of 'CellFormats' array.
                    newCell.DataType = new EnumValue<CellValues>(CellValues.Number);
                    newCell.StyleIndex = 1;
                    newCell.CellValue = new CellValue(objdate.ToOADate().ToString(CultureInfo.InvariantCulture));
                    break;

                case bool objbool:
                    AddSharedString(ref spreadsheetDocument, ref newCell, objbool.ToString());
                    break;

                default:
                    if (cellEntry is not null && Decimal.TryParse(cellEntry.ToString(), out decimal objdec))
                    {
                        newCell.DataType = new EnumValue<CellValues>(CellValues.Number);
                        newCell.CellValue = new CellValue(objdec.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        //Enter an empty cell to make IO easier
                        AddSharedString(ref spreadsheetDocument, ref newCell, " ");
                    }
                    break;
            }
        }

    }
}

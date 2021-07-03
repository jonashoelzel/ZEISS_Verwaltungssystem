using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Zeiss.PublicationManager.Data.Excel.IO
{
    public class ExcelIOAPIs : ExcelIOBase
    {
        /// <summary>
        /// Check if a worksheet does exist in a spreadsheet.
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file that should be opened.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet that should be searched.
        /// </param>
        /// <returns>
        /// True, if worksheet with (parameter) 'worksheetName' does exist, otherwise False.
        /// </returns>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
        public static bool WorksheetExists(string filepath, string worksheetName)
        {
            if (!ExcelIOBase.CheckPathExist(ref filepath))
                return false;

            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filepath, false);

            bool isExists = WorksheetExists(ref spreadsheetDocument, worksheetName, out _);

            spreadsheetDocument.Close();

            return isExists;
        }


        /// <summary>
        /// Checks if a specified ID does exist in a worksheet of the spreadsheet and that is below a specific (so called) 'header-column'.
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file that should be opened.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet that should be opened.
        /// </param>
        /// <param name="headerColumnAndID">
        /// The key is the (so called) 'header-column' 
        /// and the value is the condition a cell should match (the cell should match data-type and value) and that is below the (so called) 'header-column' in the key.
        /// </param>
        /// <returns>
        /// True, if the value in (parameter) 'headerColumnAndID' was found below the (so called) 'header-column' below the key of (parameter) 'headerColumnAndID'.
        /// </returns>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
        public static bool IsIDOfWorksheet(string filepath, string worksheetName, KeyValuePair<string, object> headerColumnAndID)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData, false, false);

            string letterID = GetColumnLetterIDsOfColumnNames(ref spreadsheetDocument, sheetData, headerColumnAndID.Key, out _);
            if (letterID is null)
                throw new ArgumentException("The Header-Column: " + headerColumnAndID.Key + " does not exist.");

            //For easier usage, we take KeyValuePair<columnHeaderName, guid>, but we need the format KeyValuePair<columnLetterID, guid>
            KeyValuePair<string, object> letterIDAndSearchID = new(letterID, headerColumnAndID.Value);
            bool found = (SearchRow(ref spreadsheetDocument, sheetData, letterIDAndSearchID) is not null);

            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return found;
        }



        /// <summary>
        /// Check if a row with all of the entered (so called) 'header-columns' do exist in the worksheet.
        /// </summary>
        /// <param name="filepath">
        /// Relative/absolute filepath to a *.xlsx file that should be opened.
        /// </param>
        /// <param name="worksheetName">
        /// Name of the worksheet that should be opened.
        /// </param>
        /// <param name="headerColumns">
        /// Every entry represents one (so called) 'header-column' that should be searched.
        /// </param>
        /// <returns>
        /// True, if all (so called) 'header-columns' where found in the same row, otherwise false.
        /// </returns>
        /// <exception cref="FileNotFoundException">Thrown if File was not found</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when misssing permission to access File</exception>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        /// <exception cref="ArgumentNullException">Thrown when an Argument was or became Null</exception>
        /// <exception cref="ArgumentException">Thrown when an entred argument was or became invalid</exception>
        /// <exception cref="InvalidCastException">Thrown when an entered value had an unexpected data-type</exception>
        /// <exception cref="OpenXmlPackageException">Thrown when exception occurred in the OpenXML-Package</exception>
        public static bool CheckHeaderColumnsExist(string filepath, string worksheetName, List<object> headerColumns)
        {
            SpreadsheetDocument spreadsheetDocument = OpenSpreadsheetDocument(filepath, worksheetName, out SheetData sheetData, false, false);

            bool found = (SearchRow(ref spreadsheetDocument, sheetData, headerColumns) is not null);

            SaveSpreadsheetDocument(ref spreadsheetDocument);

            return found;
        }


        /// <summary>
        /// Check if a path at the specified (parameter) 'filepath' does exist. 
        /// If the filepath is too long it'll try to access directly to the OS-File-System.
        /// </summary>
        /// <param name="filepath">
        /// The path to the file that should be searched.
        /// If the filepath is too long it'll try to access directly to the OS-File-System to search for the file.
        /// </param>
        /// <returns>
        /// True, if the file exists, otherwise false.
        /// </returns>
        /// <exception cref="PathTooLongException">Thrown when File-path is too long and path cannot be conveted</exception>
        public static new bool CheckPathExist(ref string filepath)
        {
            return ExcelIOBase.CheckPathExist(ref filepath);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zeiss.PublicationManager.Data.Excel.IO.Write;

namespace Zeiss.PublicationManager.Data.DataSet.IO.Write
{
    public class WriteDataSet
    {
        
        public WriteDataSet(string filePaht, string workSheetName)
        {
            DataSetBase.FilePath = filePaht;
            DataSetBase.WorkSheetName = workSheetName;
        }

        public void Insert(IPublicationDataSet dataSet)
        {
            throw new NotImplementedException();
        }

        public static void Insert(string filepath, string worksheetName, List<IPublicationDataSet> dataSets)
        {
            foreach (var dataSet in dataSets)
            {
                Insert(filepath, worksheetName, dataSet);
            }
        }

        public static void Insert(string filepath, string worksheetName, IPublicationDataSet dataSet)
        {
            InitializeDataSetWorksheet(filepath, worksheetName);
            RowInsert.Insert(filepath, worksheetName, DataSetBase.GetNewRow(dataSet));
        }

        public static void InsertIntelligent(string filepath, string worksheetName, IPublicationDataSet dataSet)
        {
            InitializeDataSetWorksheet(filepath, worksheetName);                
            RowInsert.Insert(filepath, worksheetName, DataSetBase.GetColumnNames().Select(x => x.ToString()).ToList(), DataSetBase.GetNewRow(dataSet));
        }


        /*
        public static void Insert(string filepath, string worksheetName, List<string> columnNames, IPublicationDataSet dataSet)
        {
            InitializeDataSetWorksheet(filepath, worksheetName);

            List<object> entry = new List<object>()
            {
                dataSet.ID,
                dataSet.WorkingTitle,
                dataSet.PublicationTitle,

                dataSet.TypeOfPublication.Name,

                dataSet.MainAuthor.ID,
                dataSet.MainAuthor.Name,
                dataSet.MainAuthor.Surname,
                ConvertCoAuthorsToCSV(dataSet.CoAuthors),
                dataSet.Division,

                dataSet.DateOfStartWorking.Year,
                dataSet.CurrentState,
                dataSet.DateOfRelease,

                dataSet.PublishedBy.ID,
                dataSet.PublishedBy.Name,

                ConvertTagsToCSV(dataSet.Tags),
                dataSet.Description,
                dataSet.AdditionalInformation
            };

            ExcelInsert.Insert(filepath, worksheetName, entry);
        }
        */

        private static void InitializeDataSetWorksheet(string filepath, string worksheetName)
        {
            if (!WriteExcel.WorksheetExists(ref filepath, worksheetName))
            {              
                RowInsert.Insert(filepath, worksheetName, DataSetBase.GetColumnNames());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zeiss.PublicationManager.Data.Excel.IO.Read;

namespace Zeiss.PublicationManager.Data.DataSet.IO.Read
{
    public class ReadDataSet
    {
        public static List<IPublicationDataSet> SelectIntelligent(string filepath, string worksheetName, List<string> headerColumns)
        {
            List<List<object>> table = RowSelect.Select(filepath, worksheetName, headerColumns);
            List<IPublicationDataSet> dataSets = new();

            int columnsCount = table.Count;
            int rowsCount = table[0].Count;

            for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
            {
                List<object> row = new();
                for (int columnIndex = 0; columnIndex < columnsCount; columnIndex++)
                {
                    row.Add(table[columnIndex][rowIndex]);
                }
                dataSets.Add(ConvertToDataSet(row));
            }

            return dataSets;
        }

        private static IPublicationDataSet ConvertToDataSet(List<object> row)
        {
            PublicationDataSet dataSet = new();

            dataSet.ID = Convert.ToInt32(row[0]);
            dataSet.WorkingTitle = row[1].ToString();
            dataSet.PublicationTitle = row[2].ToString();


            //dataSet.TypeOfPublication.ID = Convert.ToInt32(row[3]);
            dataSet.TypeOfPublication.Name = row[3].ToString();

            dataSet.MainAuthor.ID = Convert.ToInt32(row[4]);
            dataSet.MainAuthor.Name = row[5].ToString();
            dataSet.MainAuthor.Surname = row[6].ToString();

            dataSet.CoAuthors = DataSetBase.ConvertCSVToCoAuthors(row[7].ToString());

            dataSet.Division = row[8].ToString();

            dataSet.DateOfStartWorking = Convert.ToDateTime(row[9]);
            //dataSet.CurrentState.ID = row[10].ToString();
            dataSet.CurrentState.Name = row[10].ToString();
            dataSet.DateOfRelease = Convert.ToDateTime(row[11]);

            dataSet.PublishedBy.ID = Convert.ToInt32(row[12]);
            dataSet.PublishedBy.Name = row[13].ToString();

            dataSet.Tags = DataSetBase.ConvertCSVToTags(row[14].ToString());
            dataSet.Description = row[15].ToString();
            dataSet.AdditionalInformation = row[16].ToString();

            return dataSet;
        }
    }
}

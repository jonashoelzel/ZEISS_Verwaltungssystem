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
            List<IPublicationDataSet> dataSets = new();

            Dictionary<string, List<object>> table = RowSelect.Select(filepath, worksheetName, headerColumns);
            if (table.Any())
            {
                int rowsCount = table[headerColumns[0]].Count;

                for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                {
                    Dictionary<string, object> row = new();
                    for (int i = 0; i < headerColumns.Count; i++)
                    {
                        row.Add(headerColumns[i], table[headerColumns[i]][rowIndex]);
                    }
                    dataSets.Add(ConvertToDataSet(row));
                }
            }                   

            return dataSets;
        }

        private static IPublicationDataSet ConvertToDataSet(Dictionary<string, object> row)
        {
            throw new NotImplementedException("Not working with new data structure");

            /*
            PublicationDataSet dataSet = new();

            dataSet.ID = Guid.Parse(row["PublicationID"].ToString());
            dataSet.WorkingTitle = row["WorkingTitle"].ToString();
            dataSet.PublicationTitle = row["PublictionTitle"].ToString();

            //dataSet.TypeOfPublication.ID = Convert.ToInt32(row[3]);
            dataSet.TypeOfPublication.Name = row["TypeOfPublication"].ToString();

            dataSet.MainAuthor.ID = Guid.Parse(row["AuthorID"].ToString());
            dataSet.MainAuthor.Name = row["AuthorName"].ToString();
            dataSet.MainAuthor.Surname = row["AuthorSurname"].ToString();

            dataSet.CoAuthors = DataSetBase.ConvertCSVToCoAuthors(row["CoAuthors"].ToString());

            dataSet.Division.ID = Guid.Parse(row["Division"].ToString());
            dataSet.Division.Name = row["Division"].ToString();

            dataSet.DateOfStartWorking = Convert.ToDateTime(row["DateOfStartWorking"]);
            //dataSet.CurrentState.ID = row[10].ToString();
            dataSet.CurrentState.Name = row["CurrentState"].ToString();
            dataSet.DateOfRelease = Convert.ToDateTime(row["DateOfRelease"]);

            dataSet.PublishedBy.ID = Guid.Parse(row["PublisherID"].ToString());
            dataSet.PublishedBy.Name = row["PublisherName"].ToString();

            dataSet.Tags = DataSetBase.ConvertCSVToTags(row["Tags"].ToString());
            dataSet.Description = row["Description"].ToString();
            dataSet.AdditionalInformation = row["AdditionalINformation"].ToString();

            return dataSet;
            */
        }
    }
}

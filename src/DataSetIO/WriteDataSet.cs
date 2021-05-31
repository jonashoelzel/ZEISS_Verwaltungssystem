using System;
using System.Collections.Generic;

using Zeiss.PublicationManager.Data.Excel.IO.Write;

namespace Zeiss.PublicationManager.Data.DataSet.IO.Write
{
    public class WriteDataSet : DataSetBase
    {

        public WriteDataSet(string filePaht)
        {
            FilePath = filePaht;
        }

        public void DeletePublication(Guid guid)
        {
            RowDelete.Delete(FilePath, worksheets[0], new Dictionary<string, object>() { { "Publication_ID", guid.ToString() }, });
        }

        public void InsertPublication(IPublicationDataSet dataSet)
        {
            CheckWorkBook();

            var attributes = PublicationToAttributes(dataSet);

            var id = new KeyValuePair<string, object>("Publication_ID", attributes["Publication_ID"]);

            if (Excel.IO.ExcelIOBase.IsIDOfWorksheet(FilePath, worksheets[0], id))
            {
                var idColumn = new Dictionary<string, object>() { { id.Key, id.Value } };
                RowUpdate.Update(FilePath, worksheets[0], idColumn, attributes);
                return;
            }

            RowInsert.Insert(FilePath, worksheets[0], attributes);
        }

        public void InsertAuthor(IAuthor author)
        {
            CheckWorkBook();

            var attributes = AuthorToAttributes(author);

            var id = new KeyValuePair<string, object>("Author_ID", attributes["Author_ID"]);

            if (Excel.IO.ExcelIOBase.IsIDOfWorksheet(FilePath, worksheets[1], id))
            {
                var idColumn = new Dictionary<string, object>() { { id.Key, id.Value } };
                RowUpdate.Update(FilePath, worksheets[1], idColumn, attributes);
                return;
            }

            RowInsert.Insert(FilePath, worksheets[1], attributes);
        }

        public void InsertDivision(IDivision division)
        {
            CheckWorkBook();
            var attributes = DivisionToAttributes(division);

            var id = new KeyValuePair<string, object>("Division_ID", attributes["Division_ID"]);

            if (Excel.IO.ExcelIOBase.IsIDOfWorksheet(FilePath, worksheets[2], id))
            {
                var idColumn = new Dictionary<string, object>() { { id.Key, id.Value } };
                RowUpdate.Update(FilePath, worksheets[2], idColumn, attributes);
                return;
            }

            RowInsert.Insert(FilePath, worksheets[2], attributes);
        }

        public void InsertPublicationType(IPublicationType publicationType)
        {
            CheckWorkBook();

            var attributes = PublicationTypeToAttributes(publicationType);

            var id = new KeyValuePair<string, object>("PublicationType_ID", attributes["PublicationType_ID"]);

            if (Excel.IO.ExcelIOBase.IsIDOfWorksheet(FilePath, worksheets[3], id))
            {
                var idColumn = new Dictionary<string, object>() { { id.Key, id.Value } };
                RowUpdate.Update(FilePath, worksheets[3], idColumn, attributes);
                return;
            }

            RowInsert.Insert(FilePath, worksheets[3], attributes);
        }

        public void InsertState(IState state)
        {
            CheckWorkBook();

            var attributes = StateToAttributes(state);

            var id = new KeyValuePair<string, object>("State_ID", attributes["State_ID"]);

            if (Excel.IO.ExcelIOBase.IsIDOfWorksheet(FilePath, worksheets[4], id))
            {
                var idColumn = new Dictionary<string, object>() { { id.Key, id.Value } };
                RowUpdate.Update(FilePath, worksheets[4], idColumn, attributes);
                return;
            }

            RowInsert.Insert(FilePath, worksheets[4], attributes);
        }

        public void InsertTag(ITag tag)
        {
            CheckWorkBook();

            var attributes = TagToAttributes(tag);

            var id = new KeyValuePair<string, object>("Tag_ID", attributes["Tag_ID"]);

            if (Excel.IO.ExcelIOBase.IsIDOfWorksheet(FilePath, worksheets[5], id))
            {
                var idColumn = new Dictionary<string, object>() { { id.Key, id.Value } };
                RowUpdate.Update(FilePath, worksheets[5], idColumn, attributes);
                return;
            }

            RowInsert.Insert(FilePath, worksheets[5], attributes);
        }

        public void InsertPublisher(IPublisher publisher)
        {
            CheckWorkBook();

            var attributes = PublisherToAttributes(publisher);

            var id = new KeyValuePair<string, object>("Publisher_ID", attributes["Publisher_ID"]);

            if (Excel.IO.ExcelIOBase.IsIDOfWorksheet(FilePath, worksheets[6], id))
            {
                var idColumn = new Dictionary<string, object>() { { id.Key, id.Value } };
                RowUpdate.Update(FilePath, worksheets[6], idColumn, attributes);
                return;
            }

            RowInsert.Insert(FilePath, worksheets[6], attributes);
        }




        /*
        private static string AuthorsToCsvIDs(List<IAuthor> authors)
        {
            string csv = string.Empty;
            if (authors is null)
                return csv;

            if (authors.Count < 1)
                return csv;

            foreach (var author in authors)
            {
                csv += author.ID.ToString();
                csv += ",";
            }
            return csv[..^1];
        }

        private static string TagsToCsvIDs(List<ITag> tags)
        {
            string csv = string.Empty;
            if (tags is null)
                return csv;

            if (tags.Count < 1)
                return csv;

            foreach (var tag in tags)
            {
                csv += tag.ID.ToString();
                csv += ",";
            }
            return csv[..^1];
        }
        */


        public static Dictionary<string, object> PublicationToAttributes(IPublicationDataSet dataSet)
        {
            var publication = new Dictionary<string, object>();

            publication.Add("Publication_ID", dataSet.ID.ToString());
            publication.Add("WorkingTitle", dataSet.WorkingTitle);
            publication.Add("PublicationTitle", dataSet.PublicationTitle);
            publication.Add("DateOfStartWorking", dataSet.DateOfStartWorking);
            publication.Add("DateOfRelease", dataSet.DateOfRelease);
            publication.Add("Description", dataSet.Description);
            publication.Add("AdditionalInformation", dataSet.AdditionalInformation);

            string divisionID = string.Empty;
            if (!string.IsNullOrEmpty(dataSet.Division.Name))
                divisionID = dataSet.Division.ID.ToString();
            publication.Add("Division_ID", divisionID);

            string authorID = string.Empty;
            if (!string.IsNullOrEmpty(dataSet.MainAuthor.Name))
                authorID = dataSet.MainAuthor.ID.ToString();
            publication.Add("Author_ID", authorID);


            publication.Add("CoAuthor_IDs", ConvertAuthorsToCSV(dataSet.CoAuthors));

            string pubTypeID = string.Empty;
            if (!string.IsNullOrEmpty(dataSet.TypeOfPublication.Name))
                pubTypeID = dataSet.TypeOfPublication.ID.ToString();
            publication.Add("PublicationType_ID", pubTypeID);

            string stateID = string.Empty;
            if (!string.IsNullOrEmpty(dataSet.CurrentState.Name))
                stateID = dataSet.CurrentState.ID.ToString();
            publication.Add("State_ID", stateID);

            publication.Add("Tag_ID", ConvertTagsToCSV(dataSet.Tags));

            string publisherID = string.Empty;
            if (!string.IsNullOrEmpty(dataSet.PublishedBy?.Name))
                publisherID = dataSet.PublishedBy.ID.ToString();
            publication.Add("Publisher_ID", publisherID);

            return publication;
        }


        private static Dictionary<string, object> AuthorToAttributes(IAuthor author)
        {
            return new Dictionary<string, object>()
            {
                { "Author_ID", author.ID.ToString() },
                { "Name", author.Name },
                { "Surname", author.Surname },
            };
        }

        private static Dictionary<string, object> DivisionToAttributes(IDivision division)
        {
            return new Dictionary<string, object>()
            {
                { "Division_ID", division.ID.ToString() },
                { "Name", division.Name },
            };
        }

        private static Dictionary<string, object> PublicationTypeToAttributes(IPublicationType publicationType)
        {
            return new Dictionary<string, object>()
            {
                { "PublicationType_ID", publicationType.ID.ToString() },
                { "Name", publicationType.Name },
            };
        }

        private static Dictionary<string, object> StateToAttributes(IState state)
        {
            return new Dictionary<string, object>()
            {
                { "State_ID", state.ID.ToString() },
                { "Name", state.Name },
            };
        }

        private static Dictionary<string, object> TagToAttributes(ITag tag)
        {
            return new Dictionary<string, object>()
            {
                { "Tag_ID", tag.ID.ToString() },
                { "Name", tag.Name },
            };
        }

        private static Dictionary<string, object> PublisherToAttributes(IPublisher publisher)
        {
            return new Dictionary<string, object>()
            {
                { "Publisher_ID", publisher.ID.ToString() },
                { "Name", publisher.Name },
            };
        }

    }
}

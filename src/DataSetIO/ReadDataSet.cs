using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zeiss.PublicationManager.Data.Excel.IO.Read;

namespace Zeiss.PublicationManager.Data.DataSet.IO.Read
{
    public class ReadDataSet : DataSetBase
    {
        public ReadDataSet(string filePaht, string workSheetName)
        {
            FilePath = filePaht;
            WorkSheetName = workSheetName;
        }

        public static List<T> GetAllFromTable<T>(string filepath, string worksheetName, List<string> headerColumns, Func<Dictionary<string, object>,T> convertAttributesFunction)
        {
            List<T> dataSets = new();

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
                    dataSets.Add(convertAttributesFunction(row));
                }
            }

            return dataSets;
        }

        public List<PublicationDataSet> ReadPublicationDataSet()
        {
            var headerColumns = WorksheetsHeader()["Publication"].ConvertAll(e => e.ToString());
            return GetAllFromTable<PublicationDataSet>(FilePath, worksheets[0], headerColumns, AttributesToPublicationDataSet);
        }

        public List<Author> ReadAuthors()
        {
            var headerColumns = WorksheetsHeader()["Author"].ConvertAll(e => e.ToString());
            return GetAllFromTable<Author>(FilePath, worksheets[1], headerColumns, AttributesToAuthor);
        }

        public List<Division> ReadDivisions()
        {
            var headerColumns = WorksheetsHeader()["Division"].ConvertAll(e => e.ToString());
            return GetAllFromTable<Division>(FilePath, worksheets[2], headerColumns, AttributesToDivisions);
        }

        public List<PublicationType> ReadPublicationTypes()
        {
            var headerColumns = WorksheetsHeader()["PublicationType"].ConvertAll(e => e.ToString());
            return GetAllFromTable<PublicationType>(FilePath, worksheets[3], headerColumns, AttributesToPublicationType);
        }

        public List<State> ReadStates()
        {
            var headerColumns = WorksheetsHeader()["State"].ConvertAll(e => e.ToString());
            return GetAllFromTable<State>(FilePath, worksheets[4], headerColumns, AttributesToState);
        }

        public List<Tag> ReadTags()
        {
            var headerColumns = WorksheetsHeader()["Tag"].ConvertAll(e => e.ToString());
            return GetAllFromTable<Tag>(FilePath, worksheets[5], headerColumns, AttributesToTag);
        }

        public List<Publisher> ReadPublishers()
        {
            var headerColumns = WorksheetsHeader()["Publisher"].ConvertAll(e => e.ToString());
            return GetAllFromTable<Publisher>(FilePath, worksheets[6], headerColumns, AttributesToPublisher);
        }

        private PublicationDataSet AttributesToPublicationDataSet(Dictionary<string, object> attributes)
        {
            PublicationDataSet publication = new();

            publication.PublicationTitle = attributes["PublicationTitle"].ToString();
            publication.WorkingTitle = attributes["WorkingTitle"].ToString();
            publication.DateOfStartWorking = (DateTime)attributes["DateOfStartWorking"];
            publication.DateOfRelease = (DateTime)attributes["DateOfRelease"];
            publication.Description = attributes["Description"].ToString();
            publication.AdditionalInformation = attributes["AdditionalInformation"].ToString();

            var authorID = Guid.Parse(attributes["Author_ID"].ToString());
            publication.MainAuthor = ReadAuthors().First(e => e.ID.Equals(authorID));

            var coAuthorIDs = ConvertCSVToGuids(attributes["CoAuthor_IDs"].ToString());
            var allAuthors = ReadAuthors();
            foreach (var coAuthorID in coAuthorIDs)
                publication.CoAuthors.Add(allAuthors.First(e => e.ID.Equals(coAuthorID)));

            var divisionID = Guid.Parse(attributes["Division_ID"].ToString());
            publication.Division = ReadDivisions().First(e => e.ID.Equals(divisionID));

            var publicationTypeID = Guid.Parse(attributes["PublicationType_ID"].ToString());
            publication.TypeOfPublication = ReadPublicationTypes().First(e => e.ID.Equals(publicationTypeID));

            var stateID = Guid.Parse(attributes["State_ID"].ToString());
            publication.CurrentState = ReadStates().First(e => e.ID.Equals(stateID));

            var tagIDs = ConvertCSVToGuids(attributes["Tag_ID"].ToString());
            var allTags = ReadTags();
            foreach (var tagID in tagIDs)
                publication.Tags.Add(allTags.First(e => e.ID.Equals(tagID)));

            var publisherID = Guid.Parse(attributes["Publisher_ID"].ToString());
            publication.PublishedBy = ReadPublishers().First(e => e.ID.Equals(publisherID));


            if (Guid.TryParse(attributes["Publisher_ID"].ToString(), out Guid id))
                publication.ID = id;
            else
                throw new Exception("File corrupt");

            return publication;
        }

        private Author AttributesToAuthor(Dictionary<string, object> attributes)
        {
            Author author = new();

            author.Name = attributes["Name"].ToString();
            author.Surname = attributes["Surname"].ToString();

            if (Guid.TryParse(attributes["Author_ID"].ToString(), out Guid id))
                author.ID = id;
            else
                throw new Exception("File corrupt");

            return author;
        }

        private Division AttributesToDivisions(Dictionary<string, object> attributes)
        {
            Division division = new();

            division.Name = attributes["Name"].ToString();

            if (Guid.TryParse(attributes["Division_ID"].ToString(), out Guid id))
                division.ID = id;
            else
                throw new Exception("File corrupt");

            return division;
        }

        private PublicationType AttributesToPublicationType(Dictionary<string, object> attributes)
        {
            PublicationType publicationType = new();

            publicationType.Name = attributes["Name"].ToString();

            if (Guid.TryParse(attributes["PublicationType_ID"].ToString(), out Guid id))
                publicationType.ID = id;
            else
                throw new Exception("File corrupt");

            return publicationType;
        }

        private State AttributesToState(Dictionary<string, object> attributes)
        {
            State state = new();

            state.Name = attributes["Name"].ToString();

            if (Guid.TryParse(attributes["State_ID"].ToString(), out Guid id))
                state.ID = id;
            else
                throw new Exception("File corrupt");

            return state;
        }

        private Tag AttributesToTag(Dictionary<string, object> attributes)
        {
            Tag tag = new();

            tag.Name = attributes["Name"].ToString();

            if (Guid.TryParse(attributes["Tag_ID"].ToString(), out Guid id))
                tag.ID = id;
            else
                throw new Exception("File corrupt");

            return tag;
        }

        private Publisher AttributesToPublisher(Dictionary<string, object> attributes)
        {
            Publisher publisher = new();

            publisher.Name = attributes["Name"].ToString();

            if (Guid.TryParse(attributes["Publisher_ID"].ToString(), out Guid id))
                publisher.ID = id;
            else
                throw new Exception("File corrupt");

            return publisher;
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

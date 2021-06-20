using System;
using System.Collections.Generic;
using System.Linq;

using Zeiss.PublicationManager.Data.Excel.IO.Read;

namespace Zeiss.PublicationManager.Data.DataSet.IO.Read
{
    public class ReadDataSet : DataSetBase
    {
        public ReadDataSet(string filePaht)
        {
            FilePath = filePaht;
        }

        public static List<PublicationDataSet> ChachedPublications { get; set; } = new();
        public static List<Author> ChachedAuthors { get; set; } = new();
        public static List<Division> ChachedDivisions { get; set; } = new();
        public static List<PublicationType> ChachedPublicationTypes { get; set; } = new();
        public static List<State> ChachedStates { get; set; } = new();
        public static List<Tag> ChachedTags { get; set; } = new();
        public static List<Publisher> ChachedPublishers { get; set; } = new();

        public static List<T> GetAllFromTable<T>(string filepath, string worksheetName, List<string> headerColumns, Func<Dictionary<string, object>, T> convertAttributesFunction)
        {
            List<T> dataSets = new();

            List<Dictionary<string, object>> table = RowSelect.SelectAsRows(filepath, worksheetName, headerColumns);
            if (table.Any())
            {
                foreach (Dictionary<string, object> row in table)
                {
                    dataSets.Add(convertAttributesFunction(row));
                }
            }

            return dataSets;
        }

        public List<PublicationDataSet> ReadPublicationDataSets()
        {
            LoadAndChacheData();

            var headerColumns = WorksheetsHeader()["Publication"].ConvertAll(e => e.ToString());
            return GetAllFromTable<PublicationDataSet>(FilePath, worksheets[0], headerColumns, AttributesToPublicationDataSet);
        }

        public void LoadAndChacheData()
        {
            ChachedAuthors = ReadAuthors();
            ChachedDivisions = ReadDivisions();
            ChachedPublicationTypes = ReadPublicationTypes();
            ChachedStates = ReadStates();
            ChachedTags = ReadTags();
            ChachedPublishers = ReadPublishers();
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

            if (attributes["DateOfStartWorking"].GetType() == typeof(DateTime))
                publication.DateOfStartWorking = (DateTime)attributes["DateOfStartWorking"];
            else throw new Exception("type is not excepted");

            if (attributes["DateOfRelease"].GetType() == typeof(DateTime))
                publication.DateOfRelease = (DateTime)attributes["DateOfRelease"];
            else throw new Exception("type is not excepted");

            publication.Description = attributes["Description"].ToString();
            publication.AdditionalInformation = attributes["AdditionalInformation"].ToString();

            var authorID = Guid.Parse(attributes["Author_ID"].ToString());
            publication.MainAuthor = ChachedAuthors.First(e => e.ID.Equals(authorID));

            var coAuthorIDs = ConvertCSVToGuids(attributes["CoAuthor_IDs"].ToString());
            foreach (var coAuthorID in coAuthorIDs)
                publication.CoAuthors.Add(ChachedAuthors.First(e => e.ID.Equals(coAuthorID)));

            if (!string.IsNullOrWhiteSpace(attributes["Division_ID"].ToString()))
            {
                if (Guid.TryParse(attributes["Division_ID"].ToString(), out Guid divisionID))
                    publication.Division = ChachedDivisions.First(e => e.ID.Equals(divisionID));
                else throw new Exception("File Corrupt");
            }

            if (!string.IsNullOrWhiteSpace(attributes["PublicationType_ID"].ToString()))
            {
                if (Guid.TryParse(attributes["PublicationType_ID"].ToString(), out Guid publicationTypeID))
                    publication.TypeOfPublication = ChachedPublicationTypes.First(e => e.ID.Equals(publicationTypeID));
                else throw new Exception("File Corrupt");
            }

            if (!string.IsNullOrWhiteSpace(attributes["State_ID"].ToString()))
            {
                if (Guid.TryParse(attributes["State_ID"].ToString(), out Guid stateID))
                    publication.CurrentState = ChachedStates.First(e => e.ID.Equals(stateID));
                else throw new Exception("File Corrupt");
            }

            var tagIDs = ConvertCSVToGuids(attributes["Tag_ID"].ToString());
            foreach (var tagID in tagIDs)
                publication.Tags.Add(ChachedTags.First(e => e.ID.Equals(tagID)));

            if (!string.IsNullOrWhiteSpace(attributes["Publisher_ID"].ToString()))
            {
                if (Guid.TryParse(attributes["Publisher_ID"].ToString(), out Guid publisherID))
                    publication.PublishedBy = ChachedPublishers.First(e => e.ID.Equals(publisherID));
                else throw new Exception("File Corrupt");
            }

            if (!string.IsNullOrWhiteSpace(attributes["Publication_ID"].ToString()))
            {
                if (Guid.TryParse(attributes["Publication_ID"].ToString(), out Guid id))
                    publication.ID = id;
                else throw new Exception("File Corrupt");
            }

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
    }
}

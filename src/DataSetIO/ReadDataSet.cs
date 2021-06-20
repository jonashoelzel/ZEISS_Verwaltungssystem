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

        public static List<IPublicationDataSet> ChachedPublications { get; set; } = new();
        public static List<IAuthor> ChachedAuthors { get; set; } = new();
        public static List<IDivision> ChachedDivisions { get; set; } = new();
        public static List<IPublicationType> ChachedPublicationTypes { get; set; } = new();
        public static List<IState> ChachedStates { get; set; } = new();
        public static List<ITag> ChachedTags { get; set; } = new();
        public static List<IPublisher> ChachedPublishers { get; set; } = new();

        public static void LoadAndChacheData()
        {
            ChachedAuthors = ReadAuthors();
            ChachedDivisions = ReadDivisions();
            ChachedPublicationTypes = ReadPublicationTypes();
            ChachedStates = ReadStates();
            ChachedTags = ReadTags();
            ChachedPublishers = ReadPublishers();

            ChachedPublications = ReadPublicationDataSets();
        }


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

        public static List<IPublicationDataSet> ReadPublicationDataSets()
        {
            var headerColumns = WorksheetsHeader()["Publication"].ConvertAll(e => e.ToString());
            return GetAllFromTable<IPublicationDataSet>(FilePath, worksheets[0], headerColumns, AttributesToPublicationDataSet);
        }

        public static List<IAuthor> ReadAuthors()
        {
            var headerColumns = WorksheetsHeader()["Author"].ConvertAll(e => e.ToString());
            return GetAllFromTable<IAuthor>(FilePath, worksheets[1], headerColumns, AttributesToAuthor);
        }

        public static List<IDivision> ReadDivisions()
        {
            var headerColumns = WorksheetsHeader()["Division"].ConvertAll(e => e.ToString());
            return GetAllFromTable<IDivision>(FilePath, worksheets[2], headerColumns, AttributesToDivisions);
        }

        public static List<IPublicationType> ReadPublicationTypes()
        {
            var headerColumns = WorksheetsHeader()["PublicationType"].ConvertAll(e => e.ToString());
            return GetAllFromTable<IPublicationType>(FilePath, worksheets[3], headerColumns, AttributesToPublicationType);
        }

        public static List<IState> ReadStates()
        {
            var headerColumns = WorksheetsHeader()["State"].ConvertAll(e => e.ToString());
            return GetAllFromTable<IState>(FilePath, worksheets[4], headerColumns, AttributesToState);
        }

        public static List<ITag> ReadTags()
        {
            var headerColumns = WorksheetsHeader()["Tag"].ConvertAll(e => e.ToString());
            return GetAllFromTable<ITag>(FilePath, worksheets[5], headerColumns, AttributesToTag);
        }

        public static List<IPublisher> ReadPublishers()
        {
            var headerColumns = WorksheetsHeader()["Publisher"].ConvertAll(e => e.ToString());
            return GetAllFromTable<IPublisher>(FilePath, worksheets[6], headerColumns, AttributesToPublisher);
        }

        private static IPublicationDataSet AttributesToPublicationDataSet(Dictionary<string, object> attributes)
        {
            IPublicationDataSet publication = new PublicationDataSet
            {
                PublicationTitle = attributes["PublicationTitle"].ToString(),
                WorkingTitle = attributes["WorkingTitle"].ToString()
            };

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

        private static IAuthor AttributesToAuthor(Dictionary<string, object> attributes)
        {
            IAuthor author = new Author
            {
                Name = attributes["Name"].ToString(),
                Surname = attributes["Surname"].ToString()
            };

            if (Guid.TryParse(attributes["Author_ID"].ToString(), out Guid id))
                author.ID = id;
            else
                throw new Exception("File corrupt");

            return author;
        }

        private static IDivision AttributesToDivisions(Dictionary<string, object> attributes)
        {
            IDivision division = new Division
            {
                Name = attributes["Name"].ToString()
            };

            if (Guid.TryParse(attributes["Division_ID"].ToString(), out Guid id))
                division.ID = id;
            else
                throw new Exception("File corrupt");

            return division;
        }

        private static IPublicationType AttributesToPublicationType(Dictionary<string, object> attributes)
        {
            IPublicationType publicationType = new PublicationType
            {
                Name = attributes["Name"].ToString()
            };

            if (Guid.TryParse(attributes["PublicationType_ID"].ToString(), out Guid id))
                publicationType.ID = id;
            else
                throw new Exception("File corrupt");

            return publicationType;
        }

        private static IState AttributesToState(Dictionary<string, object> attributes)
        {
            IState state = new State
            {
                Name = attributes["Name"].ToString()
            };

            if (Guid.TryParse(attributes["State_ID"].ToString(), out Guid id))
                state.ID = id;
            else
                throw new Exception("File corrupt");

            return state;
        }

        private static ITag AttributesToTag(Dictionary<string, object> attributes)
        {
            ITag tag = new Tag
            {
                Name = attributes["Name"].ToString()
            };

            if (Guid.TryParse(attributes["Tag_ID"].ToString(), out Guid id))
                tag.ID = id;
            else
                throw new Exception("File corrupt");

            return tag;
        }

        private static IPublisher AttributesToPublisher(Dictionary<string, object> attributes)
        {
            IPublisher publisher = new Publisher
            {
                Name = attributes["Name"].ToString()
            };

            if (Guid.TryParse(attributes["Publisher_ID"].ToString(), out Guid id))
                publisher.ID = id;
            else
                throw new Exception("File corrupt");

            return publisher;
        }
    }
}

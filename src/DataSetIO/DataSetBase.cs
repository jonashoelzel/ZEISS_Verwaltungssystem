using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Zeiss.PublicationManager.Data.DataSet.IO
{
    public class DataSetBase
    {
        private static string _filePath;
        private static string _workSheetName;

        public static string FilePath { get => _filePath; set => _filePath = value; }
        public static string WorkSheetName { get => _workSheetName; set => _workSheetName = value; }

        public static List<object> GetColumnNames()
        {
            return new List<object>()
            {
                "Publikations-ID",
                "Arbeitstitel",
                "Veröffentlichungstitel",
                "Veröffentlichungsmedium",

                "Autor-ID",
                "Vorname",
                "Nachname",
                "Co-Autoren",
                "Division",

                "Arbeitsbeginn (Startjahr)",
                "Derzeitiger Arbeitsstatus",
                "Veröffentlichungsdatum",

                "Publisher-ID",
                "Publisher",

                "Tags",
                "Beschreibung (zusätzlich)",
                "Zusätzliche Informationen",
            };
        }

        public static List<object> GetNewRow(IPublicationDataSet dataSet)
        {
            return new List<object>()
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
                dataSet.AdditionalInformation,
            };
        }


        public static string ConvertCoAuthorsToCSV(List<IAuthor> coAuthors)
        {
            if (coAuthors is null)
                return String.Empty;

            List<string> authorCSVs = new();
            foreach (var author in coAuthors)
            {
                authorCSVs.Add(
                    DotYexLibrary.CSVHandler.IO.Write.CSVWriter.
                    WriteCSVLine(new List<string> { author.ID.ToString(), author.Name, author.Surname }, escapeAll: false).Trim('\n'));
            }

            return DotYexLibrary.CSVHandler.IO.Write.CSVWriter.
                    WriteCSVLine(authorCSVs, ';').Trim('\n');
        }

        public static List<IAuthor> ConvertCSVToCoAuthors(string csv)
        {
            List<IAuthor> coAuthors = new();
            if (String.IsNullOrEmpty(csv))
                return coAuthors;

            string[] authorsCSVs = DotYexLibrary.CSVHandler.IO.Read.CSVReader.ReadCSVLine(csv, ';');
            foreach (string coAuthorCSV in authorsCSVs)
            {
                string[] authorInformation = DotYexLibrary.CSVHandler.IO.Read.CSVReader.ReadCSVLine(coAuthorCSV);
                Author coAuthor = new();
                coAuthor.ID = Convert.ToInt32(authorInformation[0]);
                coAuthor.Name = authorInformation[1];
                coAuthor.Surname = authorInformation[2];

                coAuthors.Add(coAuthor);
            }

            return coAuthors;
        }

        public static string ConvertTagsToCSV(List<ITag> tags)
        {
            if (tags is null)
                return String.Empty;

            List<string> tagCSV = new();
            foreach (var tag in tags)
            {
                tagCSV.Add(tag.Name);
            }

            return DotYexLibrary.CSVHandler.IO.Write.CSVWriter.
                    WriteCSVLine(tagCSV).Trim('\n');
        }

        public static List<ITag> ConvertCSVToTags(string csv)
        {
            List<ITag> tags = new();
            if (String.IsNullOrEmpty(csv))
                return tags;

            string[] tagNames = DotYexLibrary.CSVHandler.IO.Read.CSVReader.ReadCSVLine(csv, ';');
            foreach (string tagName in tagNames)
            {               
                Tag tag = new();
                tag.Name = tagName;
                tags.Add(tag);
            }

            return tags;
        }
    }
}

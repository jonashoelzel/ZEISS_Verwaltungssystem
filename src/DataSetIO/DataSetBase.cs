using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zeiss.PublicationManager.Data;
using Zeiss.PublicationManager.Data.Excel.IO.Write;

namespace Zeiss.PublicationManager.Data.DataSet.IO
{
    public class DataSetBase
    {
        private static string _filePath;
        private static string _workSheetName;

        protected internal static string FilePath { get => _filePath; set => _filePath = value; }
        protected internal static string WorkSheetName { get => _workSheetName; set => _workSheetName = value; }

        private static readonly List<object> Publication = new()
        {
            "Publication_ID",
            "WorkingTitle",
            "PublicationTitle",
            "DateOfStartWorking",
            "DateOfRelease",
            "Description",
            "AdditionalInformation",
            "Author_ID",
            "Division_ID",
            "CoAuthor_IDs",
            "PublicationType_ID",
            "State_ID",
            "Tag_ID",
            "Publisher_ID",
        };

        private static readonly List<object> Author = new()
        {
            "Author_ID",
            "Name",
            "Surname",
        };

        private static readonly List<object> Division = new()
        {
            "Division_ID",
            "Name",
        };

        private static readonly List<object> TypeOfPublication = new()
        {
            "PublicationType_ID",
            "Name",
        };

        private static readonly List<object> State = new()
        {
            "State_ID",
            "Name",
        };

        private static readonly List<object> Tag = new()
        {
            "Tag_ID",
            "Name",
        };

        private static readonly List<object> Publisher = new()
        {
            "Publisher_ID",
            "Name",
        };

        protected internal static Dictionary<string, List<object>> WorksheetsHeader()
        {
            return new Dictionary<string, List<object>>()
            {
                { "Publication", Publication },
                { "Author", Author },
                { "Division", Division },
                { "PublicationType", TypeOfPublication },
                { "State", State },
                { "Tag", Tag },
                { "Publisher", Publisher },
            };
        }

        public enum Sheets
        {
            Publication,
            Author,
            Division,
            PublicationType,
            State,
            Tag,
            Publisher
        }

        protected internal static readonly List<string> worksheets = new()
        {
            "Publication",
            "Author",
            "Division",
            "PublicationType",
            "State",
            "Tag",
            "Publisher",
        };


        protected internal static string ConvertAuthorsToCSV(List<IAuthor> coAuthors)
        {
            if (coAuthors is null)
                return string.Empty;

            List<string> authorCSVs = new();
            foreach (var author in coAuthors)
            {
                authorCSVs.Add(
                    CSVHandler.IO.Write.CSVWriter.
                    WriteCSVLine(new List<string> { author.ID.ToString(), author.Name, author.Surname }, escapeAll: false).Trim('\n'));
            }

            return CSVHandler.IO.Write.CSVWriter.
                    WriteCSVLine(authorCSVs, ';').Trim('\n');
        }

        protected internal static List<IAuthor> ConvertCSVToAuthors(string csv)
        {
            List<IAuthor> coAuthors = new();
            if (String.IsNullOrEmpty(csv))
                return coAuthors;

            string[] authorsCSVs = CSVHandler.IO.Read.CSVReader.ReadCSVLine(csv, ';');
            foreach (string coAuthorCSV in authorsCSVs)
            {
                string[] authorInformation = CSVHandler.IO.Read.CSVReader.ReadCSVLine(coAuthorCSV);
                Author coAuthor = new();
                coAuthor.ID = Guid.Parse(authorInformation[0]);
                coAuthor.Name = authorInformation[1];
                coAuthor.Surname = authorInformation[2];

                coAuthors.Add(coAuthor);
            }

            return coAuthors;
        }

        protected internal static string ConvertTagsToCSV(List<ITag> tags)
        {
            if (tags is null)
                return String.Empty;

            List<string> tagCSV = new();
            foreach (var tag in tags)
            {
                tagCSV.Add(tag.Name);
            }

            return CSVHandler.IO.Write.CSVWriter.
                    WriteCSVLine(tagCSV).Trim('\n');
        }

        protected internal static List<Tag> ConvertCSVToTags(string csv)
        {
            List<Tag> tags = new();
            if (String.IsNullOrEmpty(csv))
                return tags;

            string[] tagNames = CSVHandler.IO.Read.CSVReader.ReadCSVLine(csv, ';');
            foreach (string tagName in tagNames)
            {               
                Tag tag = new();
                tag.Name = tagName;
                tags.Add(tag);
            }

            return tags;
        }


        protected internal static void CheckWorkBook(ref string filepath)
        {
            if (!ValidateWorkBook(ref filepath))
                InitializeWorkBook(filepath);
        }

        private static void InitializeWorkBook(string filepath)
        {
            foreach (var worksheet in WorksheetsHeader())
            {
                if (!WriteExcel.WorksheetExists(ref filepath, worksheet.Key))
                {
                    // TODO: Check if columns exist
                    // Create new Worksheet
                    RowInsert.Insert(filepath, worksheet.Key, worksheet.Value);
                }
            }
        }

        private static bool ValidateWorkBook(ref string filepath)
        {
            foreach (var worksheet in WorksheetsHeader())
            {
                if (!WriteExcel.WorksheetExists(ref filepath, worksheet.Key))
                    return false;

                if (!Excel.IO.ExcelIOBase.CheckHeaderColumnsExist(filepath, worksheet.Key, worksheet.Value))
                    return false;

            }
            return true;
        }

    }
}

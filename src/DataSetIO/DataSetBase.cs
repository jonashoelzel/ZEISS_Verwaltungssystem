using System;
using System.Collections.Generic;
using Zeiss.PublicationManager.Data.Excel.IO;
using Zeiss.PublicationManager.Data.Excel.IO.Write;

namespace Zeiss.PublicationManager.Data.DataSet.IO
{
    public class DataSetBase
    {
        private static string _filePath;

        public static string FilePath { get => _filePath; set => _filePath = value; }

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

        protected static Dictionary<string, List<object>> WorksheetsHeader()
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

        protected static readonly List<string> worksheets = new()
        {
            "Publication",
            "Author",
            "Division",
            "PublicationType",
            "State",
            "Tag",
            "Publisher",
        };


        protected static string ConvertAuthorsToCSV(List<IAuthor> coAuthors)
        {
            if (coAuthors is null)
                return string.Empty;

            List<string> authorCSVs = new();
            foreach (var author in coAuthors)
            {
                authorCSVs.Add(author.ID.ToString() + ",");
            }

            return authorCSVs.ToString()[..^2];
        }

        protected static List<Guid> ConvertCSVToGuids(string csv)
        {
            List<Guid> ids = new();
            if (string.IsNullOrEmpty(csv))
                return ids;

            string[] readIds = csv.Split(",");
            foreach (string readId in readIds)
            {
                ids.Add(Guid.Parse(readId));
            }

            return ids;
        }

        protected static string ConvertTagsToCSV(List<ITag> tags)
        {
            if (tags is null)
                return string.Empty;

            List<string> tagCSV = new();
            foreach (var tag in tags)
            {
                tagCSV.Add(tag.ID.ToString() + ",");
            }

            return tagCSV.ToString()[..^2];
        }



        public static void CheckWorkBook()
        {
            if (!ValidateWorkBook())
                InitializeWorkBook();
        }

        private static void InitializeWorkBook()
        {
            var dirPath = _filePath.Remove(_filePath.LastIndexOf('\\'));
            if (!System.IO.File.Exists(dirPath))
                System.IO.Directory.CreateDirectory(dirPath);

            foreach (var worksheet in WorksheetsHeader())
            {
                if (!ExcelIOAPIs.WorksheetExists(_filePath, worksheet.Key))
                {
                    // TODO: Check if columns exist
                    // Create new Worksheet
                    RowInsert.Insert(FilePath, worksheet.Key, worksheet.Value);
                }
            }
        }

        private static bool ValidateWorkBook()
        {
            foreach (var worksheet in WorksheetsHeader())
            {
                if (!ExcelIOAPIs.WorksheetExists(_filePath, worksheet.Key))
                    return false;

                if (!Excel.IO.ExcelIOAPIs.CheckHeaderColumnsExist(FilePath, worksheet.Key, worksheet.Value))
                    return false;

            }
            return true;
        }

    }
}

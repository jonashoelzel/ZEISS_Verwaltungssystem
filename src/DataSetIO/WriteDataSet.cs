using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            Excel.IO.Write.Legacy.LegacyRowInsert.Insert(filepath, worksheetName, DataSetBase.GetNewRow(dataSet));
        }

        public static void InsertIntelligent(string filepath, string worksheetName, IPublicationDataSet dataSet)
        {
            InitializeDataSetWorksheet(filepath, worksheetName);                
            Excel.IO.Write.Legacy.LegacyRowInsert.Insert(filepath, worksheetName, DataSetBase.GetColumnNames().Select(x => x.ToString()).ToList(), DataSetBase.GetNewRow(dataSet));
        }


        //New Interface

        public static void InsertPublication(string filepath, IPublicationDataSet dataSet)
        {
            CheckWorkBook(filepath);

            RowInsert.Insert(filepath, worksheets[0], PublicationToAttributes(dataSet));
        }

        public static void InsertAuthor(string filepath, IAuthor author)
        {
            CheckWorkBook(filepath);

            RowInsert.Insert(filepath, worksheets[1], AuthorToAttributes(author));
        }

        public static void InsertDivision(string filepath, IDivision division)
        {
            CheckWorkBook(filepath);

            RowInsert.Insert(filepath, worksheets[2], DivisionToAttributes(division));
        }

        public static void InsertTypeOfPublication(string filepath, IPublicationType publicationType)
        {
            CheckWorkBook(filepath);

            RowInsert.Insert(filepath, worksheets[3], PublicationTypeToAttributes(publicationType));
        }

        public static void InsertState(string filepath, IState state)
        {
            CheckWorkBook(filepath);

            RowInsert.Insert(filepath, worksheets[4], StateToAttributes(state));
        }

        public static void InsertTag(string filepath, ITag tag)
        {
            CheckWorkBook(filepath);

            RowInsert.Insert(filepath, worksheets[5], TagToAttributes(tag));
        }

        public static void InsertPublisher(string filepath, IPublisher publisher)
        {
            CheckWorkBook(filepath);

            RowInsert.Insert(filepath, worksheets[6], PublisherToAttributes(publisher));
        }



        private static void CheckWorkBook(string filepath)
        {
            if (!ValidateWorkBook(filepath))
                InitializeWorkBook(filepath);
        }

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

        private static void InitializeWorkBook(string filepath)
        {
            foreach (var worksheet in worksheetsHead())
            {
                if (!WriteExcel.WorksheetExists(ref filepath, worksheet.Key))
                {
                    // TODO: Check if columns exist
                    RowInsert.Insert(filepath, worksheet.Key, worksheet.Value);
                }
            }
        }

        private static bool ValidateWorkBook(string filepath)
        {
            foreach (var worksheet in worksheetsHead())
            {
                if (!WriteExcel.WorksheetExists(ref filepath, worksheet.Key))
                {
                    return false;
                }
            }
            return true;
        }

        private static readonly List<string> worksheets = new()
        {
            "Publication",
            "Author",
            "Division",
            "TypeOfPublication",
            "State",
            "Tag",
            "Publisher",
        };

        private static Dictionary<string, List<object>> worksheetsHead()
        {
            return new Dictionary<string, List<object>>()
            {
                { "Publication", Publication },
                { "Author", Author },
                { "Division", Division },
                { "TypeOfPublication", TypeOfPublication },
                { "State", State },
                { "Tag", Tag },
                { "Publisher", Publisher },
            };
        }


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
            "TypeOfPublication_ID",
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
            "TypeOfPublication_ID",
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


        public static Dictionary<string, object> PublicationToAttributes(IPublicationDataSet dataSet)
        {
            return new Dictionary<string, object>()
            {
                { "Publication_ID", dataSet.ID.ToString() },
                { "WorkingTitle", dataSet.WorkingTitle },
                { "PublicationTitle", dataSet.PublicationTitle },
                { "DateOfStartWorking", dataSet.DateOfStartWorking },
                { "DateOfRelease", dataSet.DateOfRelease },
                { "Description", dataSet.Description },
                { "AdditionalInformation", dataSet.AdditionalInformation },
                { "Division_ID", dataSet.Division.ID.ToString() },
                { "Author_ID", dataSet.MainAuthor.ID.ToString() },
                { "CoAuthor_IDs", AuthorsToCsvIDs(dataSet.CoAuthors) },
                { "TypeOfPublication_ID", dataSet.TypeOfPublication.ID.ToString() },
                { "State_ID", dataSet.CurrentState.ID.ToString() },
                { "Tag_ID", TagsToCsvIDs(dataSet.Tags) },
                { "Publisher_ID", dataSet.PublishedBy.ID.ToString() },
            };
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
                { "TypeOfPublication_ID", publicationType.ID.ToString() },
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

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

        private static void InitializeDataSetWorksheet(string filepath, string worksheetName)
        {
            if (!WriteExcel.WorksheetExists(ref filepath, worksheetName))
            {
                Excel.IO.Write.Legacy.LegacyRowInsert.Insert(filepath, worksheetName, DataSetBase.GetColumnNames());
            }
        }


        //New Interface

        public static void InsertPublication(ref string filepath, IPublicationDataSet dataSet)
        {
            CheckWorkBook(ref filepath);

            var attributes = PublicationToAttributes(dataSet);

            var id = new KeyValuePair<string, object>("Publication_ID", attributes["Publication_ID"]);

            if (Excel.IO.ExcelIOBase.IsIDOfWorksheet(filepath, worksheets[0], id))
            {
                var idColumn = new Dictionary<string, object>() { { id.Key, id.Value } };
                RowUpdate.Update(filepath, worksheets[0], idColumn, attributes);
                return;
            }

            RowInsert.Insert(filepath, worksheets[0], attributes);
        }

        public static void InsertAuthor(ref string filepath, IAuthor author)
        {
            CheckWorkBook(ref filepath);

            var attributes = AuthorToAttributes(author);

            var id = new KeyValuePair<string, object>("Author_ID", attributes["Author_ID"]);

            if (Excel.IO.ExcelIOBase.IsIDOfWorksheet(filepath, worksheets[1], id))
            {
                var idColumn = new Dictionary<string, object>() { { id.Key, id.Value } };
                RowUpdate.Update(filepath, worksheets[1], idColumn, attributes);
                return;
            }

            RowInsert.Insert(filepath, worksheets[1], attributes);
        }

        public static void InsertDivision(ref string filepath, IDivision division)
        {
            CheckWorkBook(ref filepath);
            var attributes = DivisionToAttributes(division);

            var id = new KeyValuePair<string, object>("Division_ID", attributes["Division_ID"]);

            if (Excel.IO.ExcelIOBase.IsIDOfWorksheet(filepath, worksheets[2], id))
            {
                var idColumn = new Dictionary<string, object>() { { id.Key, id.Value } };
                RowUpdate.Update(filepath, worksheets[2], idColumn, attributes);
                return;
            }

            RowInsert.Insert(filepath, worksheets[2], attributes);
        }

        public static void InsertPublicationType(ref string filepath, IPublicationType publicationType)
        {
            CheckWorkBook(ref filepath);

            var attributes = PublicationTypeToAttributes(publicationType);

            var id = new KeyValuePair<string, object>("PublicationType_ID", attributes["PublicationType_ID"]);

            if (Excel.IO.ExcelIOBase.IsIDOfWorksheet(filepath, worksheets[3], id))
            {
                var idColumn = new Dictionary<string, object>() { { id.Key, id.Value } };
                RowUpdate.Update(filepath, worksheets[3], idColumn, attributes);
                return;
            }

            RowInsert.Insert(filepath, worksheets[3], attributes);
        }

        public static void InsertState(ref string filepath, IState state)
        {
            CheckWorkBook(ref filepath);

            var attributes = StateToAttributes(state);

            var id = new KeyValuePair<string, object>("State_ID", attributes["State_ID"]);

            if (Excel.IO.ExcelIOBase.IsIDOfWorksheet(filepath, worksheets[4], id))
            {
                var idColumn = new Dictionary<string, object>() { { id.Key, id.Value } };
                RowUpdate.Update(filepath, worksheets[4], idColumn, attributes);
                return;
            }

            RowInsert.Insert(filepath, worksheets[4], attributes);
        }

        public static void InsertTag(ref string filepath, ITag tag)
        {
            CheckWorkBook(ref filepath);

            var attributes = TagToAttributes(tag);

            var id = new KeyValuePair<string, object>("Tag_ID", attributes["Tag_ID"]);

            if (Excel.IO.ExcelIOBase.IsIDOfWorksheet(filepath, worksheets[5], id))
            {
                var idColumn = new Dictionary<string, object>() { { id.Key, id.Value } };
                RowUpdate.Update(filepath, worksheets[5], idColumn, attributes);
                return;
            }

            RowInsert.Insert(filepath, worksheets[5], attributes);
        }

        public static void InsertPublisher(ref string filepath, IPublisher publisher)
        {
            CheckWorkBook(ref filepath);

            var attributes = PublisherToAttributes(publisher);

            var id = new KeyValuePair<string, object>("Publisher_ID", attributes["Publisher_ID"]);

            if (Excel.IO.ExcelIOBase.IsIDOfWorksheet(filepath, worksheets[6], id))
            {
                var idColumn = new Dictionary<string, object>() { { id.Key, id.Value } };
                RowUpdate.Update(filepath, worksheets[6], idColumn, attributes);
                return;
            }

            RowInsert.Insert(filepath, worksheets[6], attributes);
        }



        private static void CheckWorkBook(ref string filepath)
        {
            if (!ValidateWorkBook(ref filepath))
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
            foreach (var worksheet in WorksheetsHead())
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
            foreach (var worksheet in WorksheetsHead())
            {
                if (!WriteExcel.WorksheetExists(ref filepath, worksheet.Key))
                    return false;

                if (!Excel.IO.ExcelIOBase.CheckHeaderColumnsExist(filepath, worksheet.Key, worksheet.Value))
                    return false;

            }
            return true;
        }

        private static readonly List<string> worksheets = new()
        {
            "Publication",
            "Author",
            "Division",
            "PublicationType",
            "State",
            "Tag",
            "Publisher",
        };

        private static Dictionary<string, List<object>> WorksheetsHead()
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
                { "PublicationType_ID", dataSet.TypeOfPublication.ID.ToString() },
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

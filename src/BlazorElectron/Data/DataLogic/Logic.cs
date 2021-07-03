using System;
using System.Collections.Generic;
using System.Linq;

using Zeiss.PublicationManager.Data.DataSet;
using Zeiss.PublicationManager.Data.DataSet.IO.Read;
using Zeiss.PublicationManager.Data.DataSet.IO.Write;

using System.IO;

namespace Zeiss.PublicationManager.Business.Logic.IO
{
    public class DataHandler
    {
        private static string fileName = @"\ExcelDataBase.xlsx";
        public string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TestFiles" + fileName;

        private WriteDataSet excelWriter;
        private ReadDataSet excelReader;

        public WriteDataSet ExcelWriter { get => excelWriter; private set { } }
        public ReadDataSet ExcelReader { get => excelReader; private set { } }

        public DataHandler()
        {
            excelReader = new ReadDataSet(filePath);
            excelWriter = new WriteDataSet(filePath);
        }

        public DataHandler(string filePath) : this()
        {
            this.filePath = filePath;        
        }

        public void SetFilePath(string filePath)
        {
            this.filePath = filePath;
            ReadDataSet.FilePath = filePath;
            WriteDataSet.FilePath = filePath;
            ReadDataSet.CheckWorkBook();
        }

        public void CreateNewExcelIfNotExisting()
        {
            ReadDataSet.CheckWorkBook();
        }

        public void Save(IPublicationDataSet dataSet)
        {
            WriteDataSet.InsertPublication(dataSet);

            if (!string.IsNullOrEmpty(dataSet.MainAuthor.Name))
                WriteDataSet.InsertAuthor(dataSet.MainAuthor);

            if (dataSet.CoAuthors is not null)
                foreach (var author in dataSet.CoAuthors)
                    WriteDataSet.InsertAuthor(author);

            if (!string.IsNullOrEmpty(dataSet.Division.Name))
                WriteDataSet.InsertDivision(dataSet.Division);

            if (!string.IsNullOrEmpty(dataSet.PublishedBy?.Name))
                WriteDataSet.InsertPublisher(dataSet.PublishedBy);

            if (!string.IsNullOrEmpty(dataSet.CurrentState.Name))
                WriteDataSet.InsertState(dataSet.CurrentState);

            if (!string.IsNullOrEmpty(dataSet.TypeOfPublication.Name))
                WriteDataSet.InsertPublicationType(dataSet.TypeOfPublication);

            if (dataSet.Tags is not null)
                foreach (var tag in dataSet.Tags)
                    WriteDataSet.InsertTag(tag);
        }

        public void SaveAuthor(IAuthor author)
        {
            WriteDataSet.InsertAuthor(author);
        }

        public void SaveDivision(IDivision division)
        {
            WriteDataSet.InsertDivision(division);
        }

        public void SavePublicationType(IPublicationType publicationType)
        {
            WriteDataSet.InsertPublicationType(publicationType);
        }

        public void SavePublisher(IPublisher publisher)
        {
            WriteDataSet.InsertPublisher(publisher);
        }

        public bool DeletePublication(Guid guid)
        {
            WriteDataSet.DeletePublication(guid);

            return true;
        }

        public bool DeleteAuthor(Guid guid)
        {
            if (!IsAuthorUsed(guid))
            {
                WriteDataSet.DeleteAuthor(guid);

                return true;
            }

            return false;
        }

        public bool IsAuthorUsed(Guid guid)
        {
            return ReadDataSet.CachedPublications.Where(x => (x?.MainAuthor?.ID == guid
            || (x?.CoAuthors?.Where(y => y?.ID == guid)?.Any() ?? false)))?.Any() ?? false;
        }

        public bool DeletePublicationType(Guid guid)
        {
            if (!IsPublicationTypeUsed(guid))
            {
                WriteDataSet.DeletePublicationType(guid);

                return true;
            }

            return false;
        }

        public bool IsPublicationTypeUsed(Guid guid)
        {
            return ReadDataSet.CachedPublications.Where(x => x?.TypeOfPublication?.ID == guid)?.Any() ?? false;
        }
    }
}

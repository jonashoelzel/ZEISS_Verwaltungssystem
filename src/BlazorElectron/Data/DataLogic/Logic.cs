using System;
using Zeiss.PublicationManager.Data.DataSet;
using Zeiss.PublicationManager.Data.DataSet.IO.Read;
using Zeiss.PublicationManager.Data.DataSet.IO.Write;

namespace Zeiss.PublicationManager.Business.Logic.IO
{
    public class DataHandler
    {
        private static string fileName = @"\ExcelDataBase.xlsx";
        private string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TestFiles" + fileName;

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
            excelReader = new ReadDataSet(filePath);
            excelWriter = new WriteDataSet(filePath);
            excelReader.CheckWorkBook();
        }

        public void Save(IPublicationDataSet dataSet)
        {

            excelWriter.InsertPublication(dataSet);

            if (!string.IsNullOrEmpty(dataSet.MainAuthor.Name))
                excelWriter.InsertAuthor(dataSet.MainAuthor);

            if (dataSet.CoAuthors is not null)
                foreach (var author in dataSet.CoAuthors)
                    excelWriter.InsertAuthor(author);

            if (!string.IsNullOrEmpty(dataSet.Division.Name))
                excelWriter.InsertDivision(dataSet.Division);

            if (!string.IsNullOrEmpty(dataSet.PublishedBy?.Name))
                excelWriter.InsertPublisher(dataSet.PublishedBy);

            if (!string.IsNullOrEmpty(dataSet.CurrentState.Name))
                excelWriter.InsertState(dataSet.CurrentState);

            if (!string.IsNullOrEmpty(dataSet.TypeOfPublication.Name))
                excelWriter.InsertPublicationType(dataSet.TypeOfPublication);

            if (dataSet.Tags is not null)
                foreach (var tag in dataSet.Tags)
                    excelWriter.InsertTag(tag);
        }

        public void SaveAuthor(IAuthor author)
        {
            excelWriter.InsertAuthor(author);
        }

        public void SaveDivision(IDivision division)
        {
            excelWriter.InsertDivision(division);
        }

        public void SavePublicationType(IPublicationType publicationType)
        {
            excelWriter.InsertPublicationType(publicationType);
        }

        public void SavePublisher(IPublisher publisher)
        {
            excelWriter.InsertPublisher(publisher);
        }
    }
}

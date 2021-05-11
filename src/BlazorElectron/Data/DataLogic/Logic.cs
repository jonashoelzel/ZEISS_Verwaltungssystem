using Zeiss.PublicationManager.Data.DataSet;
using System;
using System.IO;

using Zeiss.PublicationManager.Data.DataSet.IO.Write;


namespace Zeiss.PublicationManager.Business.Logic.IO.Write
{
    public class WriteData
    {
        public static string GetPath()
        {
            string folderPath = @"\TestFiles";
            string fileName = @"\ExcelDataBase.xlsx";
            string directory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + folderPath;
            Directory.CreateDirectory(directory);
            return directory + fileName;
        }
        public static void Save(IPublicationDataSet dataSet)
        {
            string filepath = GetPath();
            
            WriteDataSet.InsertPublication(ref filepath, dataSet);
            WriteDataSet.InsertAuthor(ref filepath, dataSet.MainAuthor);
            if (dataSet.CoAuthors is not null)
                foreach(var author in dataSet.CoAuthors)
                    WriteDataSet.InsertAuthor(ref filepath, author);

            WriteDataSet.InsertDivision(ref filepath, dataSet.Division);
            WriteDataSet.InsertPublisher(ref filepath, dataSet.PublishedBy);
            WriteDataSet.InsertState(ref filepath, dataSet.CurrentState);
            WriteDataSet.InsertPublicationType(ref filepath, dataSet.TypeOfPublication);
            
            if (dataSet.Tags is not null)
                foreach(var tag in dataSet.Tags)
                    WriteDataSet.InsertTag(ref filepath, tag);
        }

        public static void SaveAuthor(IAuthor author)
        {
            string filepath = GetPath();

            WriteDataSet.InsertAuthor(ref filepath, author);
        }

        public static void SaveDivision(IDivision division)
        {
            string filepath = GetPath();

            WriteDataSet.InsertDivision(ref filepath, division);
        }

        public static void SavePublicationType(IPublicationType publicationType)
        {
            string filepath = GetPath();

            WriteDataSet.InsertPublicationType(ref filepath, publicationType);
        }

        public static void SavePublisher(IPublisher publisher)
        {
            string filepath = GetPath();

            WriteDataSet.InsertPublisher(ref filepath, publisher);
        }
    }
}

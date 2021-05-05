using Zeiss.PublicationManager.Data.DataSet;
using System;
using System.IO;

using Zeiss.PublicationManager.Data.DataSet.IO.Write;


namespace Zeiss.PublicationManager.Business.Logic.IO.Write
{
    public class WriteData
    {
        public static void Save(IPublicationDataSet dataSet)
        {
            string folderPath = @"\TestFiles";
            string fileName = @"\ExcelDataBase.xlsx";
            string directory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + folderPath;
            Directory.CreateDirectory(directory);
            string filepath = directory + fileName;
            
            WriteDataSet.InsertPublication(filepath, dataSet);
            WriteDataSet.InsertAuthor(filepath, dataSet.MainAuthor);
            foreach(var author in dataSet.CoAuthors)
                WriteDataSet.InsertAuthor(filepath, author);
            WriteDataSet.InsertDivision(filepath, dataSet.Division);
            WriteDataSet.InsertPublisher(filepath, dataSet.PublishedBy);
            WriteDataSet.InsertState(filepath, dataSet.CurrentState);
            WriteDataSet.InsertPublicationType(filepath, dataSet.TypeOfPublication);
            foreach(var tag in dataSet.Tags)
                WriteDataSet.InsertTag(filepath, tag);
        }
    }
}

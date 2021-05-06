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
            
            WriteDataSet.InsertPublication(ref filepath, dataSet);
            WriteDataSet.InsertAuthor(ref filepath, dataSet.MainAuthor);
            //foreach(var author in dataSet.CoAuthors)
            //    WriteDataSet.InsertAuthor(ref filepath, author);
            //WriteDataSet.InsertDivision(ref filepath, dataSet.Division);
            //WriteDataSet.InsertPublisher(ref filepath, dataSet.PublishedBy);
            //WriteDataSet.InsertState(ref filepath, dataSet.CurrentState);
            //WriteDataSet.InsertPublicationType(ref filepath, dataSet.TypeOfPublication);
            //foreach(var tag in dataSet.Tags)
            //    WriteDataSet.InsertTag(ref filepath, tag);
        }
    }
}

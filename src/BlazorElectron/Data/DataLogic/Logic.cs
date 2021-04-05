using Zeiss.PublicationManager.Data.DataSet;
using System;
using System.IO;

using Zeiss.PublicationManager.Data.DataSet.IO.Write;

namespace Zeiss.PublicationManager.Business.Logic.Write
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
            // var excelIO = new WriteDataSet("filePaht", "sheetName");
            // excelIO.Insert(dataSet);



            WriteDataSet.Insert(filepath, "Publication", dataSet);
        }
    }
}

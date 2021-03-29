using Zeiss.PublicationManager.Data.IO.Excel;
using Zeiss.PublicationManager.Data.DataSet;
using System;
using System.IO;

namespace Zeiss.PublicationManager.Business.Logic
{
    public class DataLogic
    {
        public static void Save(IPublicationDataSet dataSet)
        {
            string folderPath = @"\TestFiles";
            string fileName = @"\ExcelDataBase.xlsx.xlsx";
            string directory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + folderPath;
            Directory.CreateDirectory(directory);
            string filepath = directory + fileName;
            // var excelIO = new WriteDataSet("filePaht", "sheetName");
            // excelIO.Insert(dataSet);



            WriteDataSet.Insert(filepath, "Publication", dataSet);
        }
    }
}

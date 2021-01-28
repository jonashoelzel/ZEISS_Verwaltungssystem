using ExcelIO;
using DataSet;

namespace BlazorElectron.Data.DataLogic
{
    public class Logic
    {
        public static void Save(IPublicationDataSet dataSet)
        {
            // var excelIO = new WriteDataSet("filePaht", "sheetName");
            // excelIO.Insert(dataSet);



            WriteDataSet.Insert("ExcelDataBase.xlsx", "Publication", dataSet);
        }
    }
}

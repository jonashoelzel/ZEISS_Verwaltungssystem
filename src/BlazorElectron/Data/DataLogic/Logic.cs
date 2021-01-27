using Data.DataSet;
using Data.ExcelIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorElectron.Data.DataLogic
{
    public class Logic
    {
        public static void Save(IPublicationDataSet dataSet)
        {
            // var excelIO = new WriteDataSet("filePaht", "sheetName");
            // excelIO.Insert(dataSet);



            WriteDataSet.Insert("PublicationManagement.xlsx", "Publication", dataSet);
        }
    }
}

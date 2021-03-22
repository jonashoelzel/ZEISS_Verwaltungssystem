using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Zeiss.Data.DataSet;
using Zeiss.Data.ExcelIO;

namespace Zeiss.Data.DataLogic
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

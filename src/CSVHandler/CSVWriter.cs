//********************************************************************************************************************************
//
//!!! Please do not make changes to this file !!!
//!!! This is PRIVATE Code !!!
//!!! Many parts are comment out for future development !!!
//
//********************************************************************************************************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeiss.PublicationManager.Data.CSVHandler.IO.Write
{
    public class CSVWriter : ObjectHandler.ObjectHandlerBase
    {
        public static string WriteCSVLine(object entries, char seperator = ',', char rangeEscape = '"', bool autoEscape = true, bool escapeAll = true)
        {
            StringBuilder csvLines = new();
            List<List<string>> lines = ConvertToListxListxString(entries);

            string sepStr = seperator.ToString();
            string ranEsStr = rangeEscape.ToString();

            foreach (var line in lines)
            {
                foreach (string entry in line)
                {
                    if (autoEscape)
                    {
                        string entr = entry.Replace(ranEsStr, (ranEsStr + ranEsStr));
                        //If the entry is not empty or it does not only contain rangeEscapes OR if the entry contains a seperator
                        if (escapeAll && !string.IsNullOrEmpty(entr.Trim(rangeEscape)) || entr.Contains(sepStr))
                        {
                            entr = ranEsStr + entr + ranEsStr;
                        }

                        csvLines.Append(entr + sepStr);
                    }
                    else
                    {
                        csvLines.Append(entries + sepStr);
                    }
                }

                csvLines.Append('\n');
            }

            csvLines = csvLines.Replace((sepStr + "\n"), "\n");
            return csvLines.ToString();
        }

        public static string WriteCSVComment(string comment, char outCommenter = '#')
        {
            return outCommenter.ToString() + comment + "\n";
        }
    }
}


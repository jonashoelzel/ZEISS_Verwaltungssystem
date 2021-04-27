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


namespace Zeiss.PublicationManager.Data.CSVHandler.IO.Read
{
    public class CSVReader : ObjectHandler.ObjectHandlerBase
    {
        public static string[] ReadCSVLine(string csvLine, char seperator = ',', char rangeEscape = '"', char outCommenter = '#')
        {
            StringBuilder indexCSV = new();
            string csv = csvLine;

            if (string.IsNullOrEmpty(csvLine) || csvLine[0] == outCommenter)
                return Array.Empty<string>();

            for (int i = 0; i < csv.Length; i++)
            {
                if (csv[i] == seperator)
                {
                    indexCSV.Append(i + ",");
                }
                else if (csv[i] == rangeEscape)
                {
                    i = GetEndIndexOfRangeEscape(ref csv, i, rangeEscape);
                }
            }

            return SplitCSV(csv, GetCommaPositions(indexCSV, csv.Length));
        }

        //currentIndex is the startIndex of the rangeEscape
        //It'll return the index of the closing rangeEscape
        private static int GetEndIndexOfRangeEscape(ref string csvLine, int currentIndex, char rangeEscape = '"')
        {
            //Remove Escape Sequence
            csvLine = csvLine.Remove(currentIndex, 1);
            //Check next char that would have been next after the Escape Sequence
            if (currentIndex < csvLine.Length)
            {
                //If the rangeEscape wasn't used to escape it'll parse until the end of rangeEscape
                if (!EscapeRangeEscape(ref csvLine, currentIndex, rangeEscape))
                {
                    if (currentIndex + 1 < csvLine.Length)
                    {
                        ++currentIndex;
                        while (currentIndex < csvLine.Length)
                        {
                            //currentIndex++;
                            if (csvLine[currentIndex] == rangeEscape)
                            {
                                //Remove Escape Sequence
                                csvLine = csvLine.Remove(currentIndex, 1);
                                //If the rangeEscape wasn't used to escape it'll return the index of rangeEscape
                                if (!EscapeRangeEscape(ref csvLine, currentIndex, rangeEscape))
                                    break;
                            }

                            ++currentIndex;
                        }
                        //Because we remove the escape sequence, we need to check the char that would come after the escape
                        //If we wouldn't subtract one, we would skip the char because the next char after the escape in now at the same index as the removed escape
                        --currentIndex;
                    }
                }
            }

            return currentIndex;
        }

        private static bool EscapeRangeEscape(ref string csvLine, int currentIndex, char rangeEscape = '"')
        {
            if (currentIndex < csvLine.Length)
            {
                //If this char (and the previous (already removed) char) are rangeEscape, the first will escape the second
                if (csvLine[currentIndex] == rangeEscape)
                    return true;
            }

            return false;
        }

        private static string[] SplitCSV(string csvLine, int[] commaPositions)
        {
            string[] splitCSV = new string[commaPositions.Length];
            int startIndex = 0;
            int endIndex;

            for (int i = 0; i < commaPositions.Length; i++)
            {
                endIndex = commaPositions[i];
                //If there is a comma at the end of the line OR if there are to (or more) commas in a row
                if (startIndex == csvLine.Length || startIndex == endIndex)
                    splitCSV[i] = String.Empty;
                else
                    splitCSV[i] = csvLine[startIndex..endIndex];

                startIndex = endIndex + 1;
            }

            return splitCSV;
        }

        private static int[] GetCommaPositions(StringBuilder indexCSV, int lineLenght)
        {
            //If there were no commas the whole line is one entry (This is a fake comma for better parsing)
            if (String.IsNullOrEmpty(indexCSV.ToString()))
                return new int[1] { lineLenght };

            string[] indexStr = indexCSV.ToString().Trim(',').Split(',');
            int[] commaPositions = new int[indexStr.Length + 1];

            for (int i = 0; i < commaPositions.Length - 1; i++)
            {
                commaPositions[i] = Convert.ToInt32(indexStr[i]);
            }

            //Add fake comma for better parsing
            commaPositions[^1] = lineLenght;
            return commaPositions;
        }
    }
}


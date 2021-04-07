﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zeiss.PublicationManager.Data.DataSet;

namespace Zeiss.PublicationManager.Data.IO.Excel
{
    public class WriteDataSet
    {
        private string _filePath;
        private string _workSheetName;

        public WriteDataSet(string filePaht, string workSheetName)
        {
            _filePath = filePaht;
            _workSheetName = workSheetName;
        }

        public void Insert(IPublicationDataSet dataSet)
        {
            throw new NotImplementedException();
        }

        public static void Insert(string filepath, string worksheetName, List<IPublicationDataSet> dataSets)
        {
            foreach (var dataSet in dataSets)
            {
                Insert(filepath, worksheetName, dataSet);
            }
        }

        public static void Insert(string filepath, string worksheetName, IPublicationDataSet dataSet)
        {
            InitializeDataSetWorksheet(filepath, worksheetName);

            List<object> entry = new List<object>()
            {
                dataSet.ID,
                dataSet.WorkingTitle,
                dataSet.PublicationTitle,

                dataSet.TypeOfPublication.Name,

                dataSet.MainAuthor.ID,
                dataSet.MainAuthor.Name,
                dataSet.MainAuthor.Surname,
                ConvertCoAuthorsToCSV(dataSet.CoAuthors),
                dataSet.Division,

                dataSet.DateOfStartWorking.Year,
                dataSet.CurrentState,
                dataSet.DateOfRelease,

                dataSet.PublishedBy.ID,
                dataSet.PublishedBy.Name,

                ConvertTagsToCSV(dataSet.Tags),
                dataSet.Description,
                dataSet.AdditionalInformation
            };

            WriteExcel.Insert(filepath, worksheetName, entry);
        }

        private static string ConvertCoAuthorsToCSV(List<IAuthor> coAuthors)
        {
            if (coAuthors is null)
                return string.Empty;

            StringBuilder csv = new StringBuilder();

            foreach (var author in coAuthors)
            {
                csv.Append(
                    author.ID + "," +
                    author.Name + "," +
                    author.Surname + ";"
                    );
            }

            string csvstr = csv.ToString();
            return ((!String.IsNullOrEmpty(csvstr)) ? csvstr[..^1] : "");
        }

        private static string ConvertTagsToCSV(List<ITag> tags)
        {
            if (tags is null)
                return string.Empty;

            StringBuilder csv = new StringBuilder();

            foreach (var tag in tags)
            {
                csv.Append(tag.Name + ",");
            }

            string csvstr = csv.ToString();
            return ((!String.IsNullOrEmpty(csvstr)) ? csvstr[..^1] : "");
        }

        private static void InitializeDataSetWorksheet(string filepath, string worksheetName)
        {
            if (!WriteExcel.WorksheetExists(ref filepath, worksheetName))
            {
                List<object> entry = new List<object>()
                {
                    "Publikations-ID",
                    "Arbeitstitel",
                    "Veröffentlichungstitel",
                    "Veröffentlichungsmedium",

                    "Autor-ID",
                    "Vorname",
                    "Nachname",
                    "Co-Autoren",
                    "Division",

                    "Arbeitsbeginn (Startjahr)",
                    "Derzeitiger Arbeitsstatus",
                    "Veröffentlichungsdatum",

                    "Publisher-ID",
                    "Publisher",

                    "Tags",
                    "Beschreibung (zusätzlich)",
                    "Zusätzliche Informationen"
                };

                WriteExcel.Insert(filepath, worksheetName, entry);
            }
        }
    }

}
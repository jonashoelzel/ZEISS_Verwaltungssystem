using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Zeiss.PublicationManager.Data.DataSet;
using Zeiss.PublicationManager.Data.DataSet.IO.Write;


namespace Zeiss.PublicationManager.Data.Excel.IO.ComponentTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestDataSetIO testIO = new TestDataSetIO();

            Console.WriteLine("Initialize DataSet IO Component Test");
            Console.WriteLine("Enter number of tests:\n");

            int count = Convert.ToInt32(Console.ReadLine());
            testIO.DataSetWriteOnly(count);

            Console.WriteLine("\nTask complete. Press any key to close\n");
            Console.ReadLine();
        }
    }

    public class TestDataSetIO
    {
        private string folderPath = @"\TestFiles";
        private string fileName = @"\IntelligentExcelIOComponentTestFileV2.xlsx";

        private string[] worksheetNames = new string[] { "sheet00", "Sheet0", "sheet1", "sheet2", "sheet3", "Publications", "test" };


        private int[] IDs = new int[] { 1, 2, 3, 5, 7, 11, 13, 37, 42, 73, 97, 100 };
        private string[] Titles = new string[] { "C#" , ".NET", "Visual Studio", "C# 9", ".NET 5", "Visual Studio 2019", "C-Sharp", "dot-NET", "Visual Studio Code",
            "42", "37", "73", "Excel", "Spreadsheet", "Worksheet", "Test", "Workbook", "Zeiss", "ZDI", "YeGaSoft" };

        private string[] PublicationType = new string[] { "Magazin", "Artikel", "Buch", "Zeitungsartikel", "Onlineauftritt", "Videobeitrag", "Dokumentation", "Vorlesung", "Konferenz", "Test" };

        private string[] Names = new string[] { "Vanessa", "Jonas", "Nina", "Paul", "Stefan", "Jean", "Pierre", "Peter", "Oliver", "Stephan", "YeXtaiZ", "Sebastian", "Sabine", "Hendrik" };
        private string[] Surnames = new string[] { "Hölzel", "Plüsch", "Plüschmann", "Joschk", "Kaiser", "Keiser", "Wünsche", "Joneleit", "YeXtaiZ", "Meyer", "Raab", "Würst", "Parker", "Lösch" };


        private string[] Divisions = new string[] { "IT", "Management", "Chairmen", "Office", "QS", "Studio", "Entertainment", "Health", "Research", "Security", "Headmaster", "Production", "Transit" };

        private Random Randomizer = new Random();
        private DateTime startDate = new DateTime(2000, 1, 1);
        private DateTime GetRandomDate()
        {      
            int range = (DateTime.Today.AddDays(Randomizer.Next(365)) - startDate).Days;
            return startDate.AddDays(Randomizer.Next(range));
        }

        private string[] CurrentStates = new string[] { "Started", "In Progress", "Paused", "Stopped", "Released", "Unknown", "Undefined", "In Check", "Editing", "None" };


        private string[] Publishers = new string[] { "YeGaSoft", "YeXtaiZ Studio", "YeXtaiZ Corporation", "Zeiss", "ZDI", "Microsoft", "Xbox" };


        private string[] Tags = new string[] { "C#", ".NET", "Visual Studio", "Zeiss", "ZDI", "YeGaSoft", "Video", "Music", "Picture", "IT", "Security", "Office", "Studio", "QS", "Excel" };

        private string[] Words = new string[] {"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
                                                "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
                                                "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat" };


        private PublicationDataSet GenerateDataSet()
        {
            PublicationDataSet dataSet = new PublicationDataSet();

            dataSet.ID = IDs[Randomizer.Next(IDs.Length)];
            dataSet.WorkingTitle = Titles[Randomizer.Next(Titles.Length)] + " " + Titles[Randomizer.Next(Titles.Length)];
            dataSet.PublicationTitle = Titles[Randomizer.Next(Titles.Length)] + " " + Titles[Randomizer.Next(Titles.Length)] + " " + Titles[Randomizer.Next(Titles.Length)];

            dataSet.TypeOfPublication.Name = PublicationType[Randomizer.Next(PublicationType.Length)];

            dataSet.MainAuthor = GenerateAuthor();
            dataSet.CoAuthors = GenerateCoAuthors();

            dataSet.Division = Divisions[Randomizer.Next(Divisions.Length)];

            dataSet.DateOfStartWorking = GetRandomDate();
            dataSet.CurrentState.Name = CurrentStates[Randomizer.Next(CurrentStates.Length)];
            dataSet.DateOfRelease = GetRandomDate();

            dataSet.PublishedBy.ID = IDs[Randomizer.Next(IDs.Length)];
            dataSet.PublishedBy.Name = Publishers[Randomizer.Next(Publishers.Length)];

            dataSet.Tags = GenerateTags();
            dataSet.Description = GenerateText();
            dataSet.AdditionalInformation = GenerateText();

            return dataSet;
        }

        private Author GenerateAuthor()
        {
            Author author = new Author();

            author.ID = IDs[Randomizer.Next(IDs.Length)];
            author.Name = Names[Randomizer.Next(Names.Length)];
            author.Surname = Surnames[Randomizer.Next(Surnames.Length)];

            return author;
        }

        private List<IAuthor> GenerateCoAuthors()
        {
            List<IAuthor> authors = new List<IAuthor>();

            int count = Randomizer.Next(5);
            for (int i = 0; i < count; i++)
            {
                authors.Add(GenerateAuthor());
            }

            return authors;
        }

        private Tag GenerateTag()
        {
            Tag tag = new Tag();

            string[][] tagArrays = new string[][] { Titles, Tags };
            int tarr = Randomizer.Next(tagArrays.GetLength(0));
            tag.Name = tagArrays[tarr][Randomizer.Next(tagArrays[tarr].Length)];

            return tag;
        }

        private List<ITag> GenerateTags()
        {
            List<ITag> tags = new List<ITag>();

            int count = Randomizer.Next(5);
            for (int i = 0; i < count; i++)
            {
                tags.Add(GenerateTag());
            }

            return tags;
        }

        private string GenerateText()
        {
            StringBuilder text = new StringBuilder();

            int count = Randomizer.Next(15);
            for (int i = 0; i <= count; i++)
            {
                text.Append(Words[Randomizer.Next(Words.Length)] + " ");
            }

            return text.ToString();
        }


        public void DataSetWriteOnly(int count = 100)
        {
            string directory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + folderPath;
            Directory.CreateDirectory(directory);
            string filepath = directory + fileName;

            for (int i = 1; i <= count; i++)
            {
                //WriteDataSet.Insert(filepath, worksheetNames[Randomizer.Next(worksheetNames.Length)], GenerateDataSet());
                WriteDataSet.InsertIntelligent(filepath, worksheetNames[Randomizer.Next(worksheetNames.Length)], GenerateDataSet());

                //System.Threading.Thread.Sleep(500);

                Console.WriteLine("Wrote {0} of {1} datasets", i, count);
            }

            Console.WriteLine("Test Complete");
        }
    }
}

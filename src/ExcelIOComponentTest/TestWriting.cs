using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Zeiss.PublicationManager.Data.DataSet;
using Zeiss.PublicationManager.Data.DataSet.IO.Write;


namespace Zeiss.PublicationManager.Data.Excel.IO.ComponentTest.Write
{
    public class TestWriting
    {
        private readonly SpreadsheetInfos SheetInfo = new();

        //private readonly int[] IDs = new int[] { 1, 2, 3, 5, 7, 11, 13, 37, 42, 73, 97, 100 };
        private readonly string[] Titles = new string[] { "C#" , ".NET", "Visual Studio", "C# 9", ".NET 5", "Visual Studio 2019", "C-Sharp", "dot-NET", "Visual Studio Code",
            "42", "37", "73", "Excel", "Spreadsheet", "Worksheet", "Test", "Workbook", "Zeiss", "ZDI", "YeGaSoft" };

        private readonly string[] PublicationType = new string[] { "Magazin", "Artikel", "Buch", "Zeitungsartikel", "Onlineauftritt", "Videobeitrag", "Dokumentation", "Vorlesung", "Konferenz", "Test" };

        private readonly string[] Names = new string[] { "Vanessa", "Jonas", "Nina", "Paul", "Stefan", "Jean", "Pierre", "Peter", "Oliver", "Stephan", "YeXtaiZ", "Sebastian", "Sabine", "Hendrik" };
        private readonly string[] Surnames = new string[] { "Hölzel", "Plüsch", "Plüschmann", "Joschk", "Kaiser", "Keiser", "Wünsche", "Joneleit", "YeXtaiZ", "Meyer", "Raab", "Würst", "Parker", "Lösch" };


        private readonly string[] Divisions = new string[] { "IT", "Management", "Chairmen", "Office", "QS", "Studio", "Entertainment", "Health", "Research", "Security", "Headmaster", "Production", "Transit" };

        private Random Randomizer = new();
        public Random NewRandom { get => Randomizer; set => Randomizer = value; }


        private readonly DateTime startDate = new(2000, 1, 1);
        private DateTime GetRandomDate()
        {
            int range = (DateTime.Today.AddDays(Randomizer.Next(365)) - startDate).Days;
            return startDate.AddDays(Randomizer.Next(range));
        }

        private readonly string[] CurrentStates = new string[] { "Started", "In Progress", "Paused", "Stopped", "Released", "Unknown", "Undefined", "In Check", "Editing", "None" };


        private readonly string[] Publishers = new string[] { "YeGaSoft", "YeXtaiZ Studio", "YeXtaiZ Corporation", "Zeiss", "ZDI", "Microsoft", "Xbox" };


        private readonly string[] Tags = new string[] { "C#", ".NET", "Visual Studio", "Zeiss", "ZDI", "YeGaSoft", "Video", "Music", "Picture", "IT", "Security", "Office", "Studio", "QS", "Excel" };

        private readonly string[] Words = new string[] {"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
                                                "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
                                                "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat" };


        private PublicationDataSet GenerateRandomDataSet()
        {
            PublicationDataSet dataSet = new()
            {
                ID = Guid.NewGuid(),
                WorkingTitle = Titles[Randomizer.Next(Titles.Length)] + " " + Titles[Randomizer.Next(Titles.Length)],
                PublicationTitle = Titles[Randomizer.Next(Titles.Length)] + " " + Titles[Randomizer.Next(Titles.Length)] + " " + Titles[Randomizer.Next(Titles.Length)]
            };

            dataSet.TypeOfPublication = GenerateRandomPublicationType();

            dataSet.MainAuthor = GenerateRandomAuthor();
            dataSet.CoAuthors = GenerateRandomCoAuthors();

            dataSet.Division = GenerateRandomDivision();

            dataSet.DateOfStartWorking = GetRandomDate();
            dataSet.CurrentState = GenerateRandomState();
            dataSet.DateOfRelease = GetRandomDate();

            dataSet.PublishedBy = GenerateRandomPublisher();

            dataSet.Tags = GenerateRandomTags();
            dataSet.Description = GenerateRandomText();
            dataSet.AdditionalInformation = GenerateRandomText();

            return dataSet;
        }

        private IPublicationType GenerateRandomPublicationType()
        {
            return new PublicationType()
            {
                ID = Guid.NewGuid(),
                Name = PublicationType[Randomizer.Next(PublicationType.Length)],
            };
        }

        private IState GenerateRandomState()
        {
            return new State()
            {
                ID = Guid.NewGuid(),
                Name = CurrentStates[Randomizer.Next(CurrentStates.Length)],
            };
        }

        private IPublisher GenerateRandomPublisher()
        {
            return new Publisher()
            {
                ID = Guid.NewGuid(),
                Name = Publishers[Randomizer.Next(Publishers.Length)],
            };
        }

        private IDivision GenerateRandomDivision()
        {
            return new Division()
            {
                ID = Guid.NewGuid(),
                Name = Divisions[Randomizer.Next(Divisions.Length)],
            };
        }

        private Author GenerateRandomAuthor()
        {
            Author author = new()
            {
                ID = Guid.NewGuid(),
                Name = Names[Randomizer.Next(Names.Length)],
                Surname = Surnames[Randomizer.Next(Surnames.Length)]
            };

            return author;
        }

        private List<IAuthor> GenerateRandomCoAuthors()
        {
            List<IAuthor> authors = new();

            int count = Randomizer.Next(5);
            for (int i = 0; i < count; i++)
            {
                authors.Add(GenerateRandomAuthor());
            }

            return authors;
        }

        private Tag GenerateRandomTag()
        {
            Tag tag = new();

            string[][] tagArrays = new string[][] { Titles, Tags };
            int tarr = Randomizer.Next(tagArrays.GetLength(0));
            tag.Name = tagArrays[tarr][Randomizer.Next(tagArrays[tarr].Length)];

            return tag;
        }

        private List<ITag> GenerateRandomTags()
        {
            List<ITag> tags = new();

            int count = Randomizer.Next(5);
            for (int i = 0; i < count; i++)
            {
                tags.Add(GenerateRandomTag());
            }

            return tags;
        }

        private string GenerateRandomText()
        {
            StringBuilder text = new();

            int count = Randomizer.Next(15);
            for (int i = 0; i <= count; i++)
            {
                text.Append(Words[Randomizer.Next(Words.Length)] + " ");
            }

            return text.ToString();
        }


        public void WriteRandomDataSet(int count)
        {
            string directory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + SheetInfo.FolderPath;
            Directory.CreateDirectory(directory);
            string filepath = directory + SheetInfo.FileName;

            for (int i = 1; i <= count; i++)
            {
                //WriteDataSet.Insert(filepath, worksheetNames[Randomizer.Next(worksheetNames.Length)], GenerateDataSet());
                //WriteDataSet.InsertIntelligent(filepath, SheetInfo.WorksheetNames[Randomizer.Next(SheetInfo.WorksheetNames.Length)], GenerateRandomDataSet());

                //System.Threading.Thread.Sleep(500);

                Console.WriteLine("Wrote {0} of {1} datasets", i, count);
            }

            Console.WriteLine("\nWrite Test Complete\n");
        }
    }
}

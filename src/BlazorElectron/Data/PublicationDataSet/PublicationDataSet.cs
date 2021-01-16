using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorElectron.Data.PublicationDataSet
{
    public class PublicationDataSet : DataObject
    {
        // Publication Information
        public string WorkingTitle { get; set; } // What is the difference between working and publication title ?
        public string PublicationTitle { get; set; }
        //Type of medium the medium will be published (newspaper, magazin, book, blog, [...])
        public PublicationType TypeOfPublication = new PublicationType();


        // Additional Information
        public string CurrentState { get; set; }
        public DateTime DateOfStartWorking { get; set; }
        public DateTime DateOfRelease { get; set; }
        public List<string> Tags { get; set; }
        public string Description { get; set; }
        public string AdditionalInformation { get; set; }


        // Author Information
        public Author MainAuthor = new Author();
        public List<Author> CoAuthors { get; set; }


        // Publisher Information
        public Publisher PublishedBy = new Publisher();
    }

    public class PublicationType : DataObject
    {
        public string Name { get; set; }
    }

    public class Author : DataObject
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        //What division is exatctly meant?!
        public string Division { get; set; }
    }


    public class Publisher : DataObject
    {
        public string Name { get; set; }
    }

    public class DataObject
    {
        private int? _ID;
        public int? ID
        {
            get { return _ID; }
            set
            {
                if (_ID is null)
                    _ID = ID;
            }
        }
    }
}

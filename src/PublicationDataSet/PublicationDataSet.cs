using System;
using System.Collections.Generic;

namespace PublicationDataSet
{
    public class PublicationDataSet : DataObject, IPublicationDataSet
    {
        // Publication Information
        public string WorkingTitle { get; set; }
        public string PublicationTitle { get; set; }

        //Type of medium the medium will be published (newspaper, magazine, book, blog, [...])
        public IPublicationType TypeOfPublication { get; set; } = new PublicationType();


        // Author Information
        public IAuthor MainAuthor { get; set; } = new Author();
        public List<IAuthor> CoAuthors { get; set; }


        // Additional Information
        public IState CurrentState { get; set; } = new State();
        public DateTime DateOfStartWorking { get; set; }
        public DateTime DateOfRelease { get; set; }
        public List<ITag> Tags { get; set; }
        public string Description { get; set; }
        public string AdditionalInformation { get; set; }

        public string Division { get; set; }



        // Publisher Information
        public IPublisher PublishedBy { get; set; } = new Publisher();
    }
}

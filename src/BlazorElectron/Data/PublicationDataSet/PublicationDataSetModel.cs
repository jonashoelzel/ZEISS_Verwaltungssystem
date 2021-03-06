using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


using Zeiss.PublicationManager.Data.Excel.IO;
using Zeiss.PublicationManager.Data.DataSet;


namespace Zeiss.PublicationManager.Data.DataSet.Model
{
    public class PublicationDataSetModel : DataObject, IPublicationDataSet
    {
        public PublicationDataSetModel()
        {
            ID = Guid.NewGuid();
        }

        // Publication Information
        [Required(ErrorMessage = "Titel ist ein Pflichtfeld")]
        [MaxLength(200, ErrorMessage = "Titel ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Titel eingeben")]
        public string WorkingTitle { get; set; }

        [Required(ErrorMessage = "Titel ist ein Pflichtfeld")]
        [MaxLength(200, ErrorMessage = "Titel ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Titel eingeben")]
        public string PublicationTitle { get; set; }

        //Type of medium the medium will be published (newspaper, magazine, book, blog, [...])
        public IPublicationType TypeOfPublication { get; set; } = new PublicationTypeModel();


        // Author Information
        public IAuthor MainAuthor { get; set; } = new AuthorModel();
        public List<IAuthor> CoAuthors { get; set; }


        // Additional Information
        public IState CurrentState { get; set; } = new StateModel();

        public DateTime DateOfStartWorking { get; set; } = DateTime.Now;
        public DateTime DateOfRelease { get; set; } = DateTime.Now;

        public List<ITag> Tags { get; set; }
        public string Description { get; set; }
        public string AdditionalInformation { get; set; }

        public IDivision Division { get; set; } = new DivisionModel();



        // Publisher Information
        public IPublisher PublishedBy { get; set; } = new PublisherModel();

        public void Reset()
        {
            ID = Guid.NewGuid();
            WorkingTitle = "";
            PublicationTitle = "";
            TypeOfPublication = new PublicationTypeModel();
            MainAuthor = new AuthorModel();
            CoAuthors = new List<IAuthor>();
            CurrentState = new StateModel();
            DateOfStartWorking = DateTime.Now;
            DateOfRelease = DateTime.Now;
            Tags = new List<ITag>();
            Description = "";
            AdditionalInformation = "";
            Division = new DivisionModel();
            PublishedBy = new PublisherModel();
        }

        public void Set(IPublicationDataSet publication)
        {
            ID = publication.ID;
            WorkingTitle = publication.WorkingTitle;
            PublicationTitle = publication.PublicationTitle;
            TypeOfPublication = publication.TypeOfPublication;
            MainAuthor = publication.MainAuthor;
            CoAuthors = publication.CoAuthors;
            CurrentState = publication.CurrentState;
            DateOfStartWorking = publication.DateOfStartWorking;
            DateOfRelease = publication.DateOfRelease;
            Tags = publication.Tags;
            Description = publication.Description;
            AdditionalInformation = publication.AdditionalInformation;
            Division = publication.Division;
            PublishedBy = publication.PublishedBy;
        }
    }
}

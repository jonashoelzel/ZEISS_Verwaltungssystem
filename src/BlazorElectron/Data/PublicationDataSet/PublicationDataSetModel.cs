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
            ID = new Guid();
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
    }
}

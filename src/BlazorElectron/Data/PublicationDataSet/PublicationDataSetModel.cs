using PublicationDataSet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorElectron.Data.PublicationDataSet
{
    public class PublicationDataSetModel : DataObject, IPublicationDataSet
    {
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
        public DateTime DateOfStartWorking { get; set; }
        public DateTime DateOfRelease { get; set; }
        public List<ITag> Tags { get; set; }
        public string Description { get; set; }
        public string AdditionalInformation { get; set; }

        public string Division { get; set; }



        // Publisher Information
        public IPublisher PublishedBy { get; set; } = new PublisherModel();
    }
}

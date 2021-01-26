using BlazorElectron.Data.DataLogic;
using Data.DataSet;
using System.ComponentModel.DataAnnotations;
using DataSetIOComponentTest;

namespace BlazorElectron.Data.PublicationDataSet
{
    public class PublicationTypeModel : DataObject, IPublicationType
    {
        public PublicationTypeModel()
        {
            ID = TestDataSetIO.GetRandomID();
        }
        [Required]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }
    }

    public class StateModel : DataObject, IState
    {
        public StateModel()
        {
            ID = TestDataSetIO.GetRandomID();
        }
        [Required(ErrorMessage = "Name ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }
    }

    public class TagModel : DataObject, ITag
    {
        public TagModel()
        {
            ID = TestDataSetIO.GetRandomID();
        }
        [Required(ErrorMessage = "Name ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }
    }

    public class AuthorModel : DataObject, IAuthor
    {
        public AuthorModel()
        {
            // ID = Logic.GetNewAuthorID();
            ID = TestDataSetIO.GetRandomID();
        }
        [Required(ErrorMessage = "Vorname ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Nachname ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Surname { get; set; }
    }

    public class PublisherModel : DataObject, IPublisher
    {
        public PublisherModel()
        {
            ID = TestDataSetIO.GetRandomID();
        }
        [Required(ErrorMessage = "Name ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }
    }
}

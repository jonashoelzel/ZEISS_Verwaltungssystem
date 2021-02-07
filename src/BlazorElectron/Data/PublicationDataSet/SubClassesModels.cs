using PublicationDataSet;
using System.ComponentModel.DataAnnotations;

namespace BlazorElectron.Data.PublicationDataSet
{
    public class PublicationTypeModel : DataObject, IPublicationType
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }
    }

    public class StateModel : DataObject, IState
    {
        [Required(ErrorMessage = "Name ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }
    }

    public class TagModel : DataObject, ITag
    {
        [Required(ErrorMessage = "Name ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }
    }

    public class AuthorModel : DataObject, IAuthor
    {
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
        [Required(ErrorMessage = "Name ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }
    }
}

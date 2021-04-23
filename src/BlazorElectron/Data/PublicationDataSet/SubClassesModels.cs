using System.ComponentModel.DataAnnotations;

using Zeiss.PublicationManager.Data.Excel.IO;
using Zeiss.PublicationManager.Data.DataSet;

namespace Zeiss.PublicationManager.Data.DataSet.Model
{
    public class PublicationTypeModel : DataObject, IPublicationType
    {
        public PublicationTypeModel()
        {
            ID = Randomizer.GetRandomID();
        }
        [Required(ErrorMessage = "Art der Veröffentlichung ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }
    }

    public class StateModel : DataObject, IState
    {
        public StateModel()
        {
            ID = Randomizer.GetRandomID();
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
            ID = Randomizer.GetRandomID();
        }

        public string Name { get; set; }
    }

    public class AuthorModel : DataObject, IAuthor
    {
        public AuthorModel()
        {
            // ID = Logic.GetNewAuthorID();
            ID = Randomizer.GetRandomID();
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
            ID = Randomizer.GetRandomID();
        }
        [Required(ErrorMessage = "Name ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }
    }
}

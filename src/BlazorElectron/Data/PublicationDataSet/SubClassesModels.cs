using System.ComponentModel.DataAnnotations;

using Zeiss.PublicationManager.Data.Excel.IO;
using Zeiss.PublicationManager.Data.DataSet;
using System;

namespace Zeiss.PublicationManager.Data.DataSet.Model
{
    public class PublicationTypeModel : DataObject, IPublicationType
    {
        public PublicationTypeModel()
        {
            ID = Guid.NewGuid();
        }
        [Required(ErrorMessage = "Art der Veröffentlichung ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }

        public void Set(IPublicationType publicationType)
        {
            ID = publicationType.ID;
            Name = publicationType.Name;
        }
    }

    public class StateModel : DataObject, IState
    {
        public StateModel()
        {
            ID = Guid.NewGuid();
        }
        [Required(ErrorMessage = "Name ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }

        public void Set(IState state)
        {
            ID = state.ID;
            Name = state.Name;
        }
    }

    public class TagModel : DataObject, ITag
    {
        public TagModel()
        {
            ID = Guid.NewGuid();
        }
        [Required(ErrorMessage = "Name ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }

        public void Set(ITag tag)
        {
            ID = tag.ID;
            Name = tag.Name;
        }
    }

    public class AuthorModel : DataObject, IAuthor
    {
        public AuthorModel()
        {
            ID = Guid.NewGuid();
        }
        [Required(ErrorMessage = "Vorname ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Nachname ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Surname { get; set; }

        public void Set(IAuthor author)
        {
            ID = author.ID;
            Name = author.Name;
            Surname = author.Surname;
        }
    }

    public class PublisherModel : DataObject, IPublisher
    {
        public PublisherModel()
        {
            ID = Guid.NewGuid();
        }
        [Required(ErrorMessage = "Name ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }

        public void Set(IPublisher publisher)
        {
            ID = publisher.ID;
            Name = publisher.Name;
        }
    }

    public class DivisionModel : DataObject, IDivision
    {
        public DivisionModel()
        {
            ID = Guid.NewGuid();
        }

        [Required(ErrorMessage = "Geschäftsbereich ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Geschäftsbereich ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Geschäftsbereich eingeben")]
        public string Name { get; set; }

        public void Set(IDivision division)
        {
            ID = division.ID;
            Name = division.Name;
        }
    }
}

using System.ComponentModel.DataAnnotations;

using Zeiss.Data.DataSet;
using Zeiss.Data.ExcelIO;


namespace Zeiss.Data.PublicationDataSetModel
{
    public class PublicationTypeModel : DataObject, IPublicationType
    {
        public PublicationTypeModel()
        {
            ID = ExcelIO.ComponentTest.WriteDataSet.TestDataSetIO.GetRandomID();
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
            ID = ExcelIO.ComponentTest.WriteDataSet.TestDataSetIO.GetRandomID();
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
            ID = ExcelIO.ComponentTest.WriteDataSet.TestDataSetIO.GetRandomID();
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
            ID = ExcelIO.ComponentTest.WriteDataSet.TestDataSetIO.GetRandomID();
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
            ID = ExcelIO.ComponentTest.WriteDataSet.TestDataSetIO.GetRandomID();
        }
        [Required(ErrorMessage = "Name ist ein Pflichtfeld")]
        [MaxLength(100, ErrorMessage = "Name ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Name eingeben")]
        public string Name { get; set; }
    }
}

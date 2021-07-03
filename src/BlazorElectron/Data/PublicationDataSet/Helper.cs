using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Zeiss.PublicationManager.Data.DataSet.Model
{
    public class TitleModel
    {
        [Required(ErrorMessage = "Titel ist ein Pflichtfeld")]
        [MaxLength(200, ErrorMessage = "Titel ist zu lang")]
        [MinLength(1, ErrorMessage = "Bitte Titel eingeben")]
        public string Name { get; set; }
    }

    public class DescriptionModel
    {
        [MaxLength(200, ErrorMessage = "Beschreibung ist zu lang")]
        public string Description { get; set; }


        [MaxLength(2000, ErrorMessage = "Beschreibung ist zu lang")]
        public string AdditionalInformation { get; set; }
    }
}

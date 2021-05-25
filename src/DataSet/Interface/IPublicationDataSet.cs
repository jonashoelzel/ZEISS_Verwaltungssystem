using System;
using System.Collections.Generic;

namespace Zeiss.PublicationManager.Data.DataSet
{
    public interface IPublicationDataSet : IDataObject
    {
        string WorkingTitle { get; set; }
        string PublicationTitle { get; set; }
        IPublicationType TypeOfPublication { get; set; }
        IAuthor MainAuthor { get; set; }
        List<IAuthor> CoAuthors { get; set; }
        IState CurrentState { get; set; }
        DateTime DateOfRelease { get; set; }
        DateTime DateOfStartWorking { get; set; }
        List<ITag> Tags { get; set; }
        string Description { get; set; }
        string AdditionalInformation { get; set; }
        IDivision Division { get; set; }
        IPublisher PublishedBy { get; set; }
    }
}
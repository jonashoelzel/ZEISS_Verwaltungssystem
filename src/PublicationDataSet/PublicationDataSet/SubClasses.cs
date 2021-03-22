namespace Zeiss.Data.DataSet
{
    public class PublicationType : DataObject, IPublicationType
    {
        public string Name { get; set; }
    }

    public class State : DataObject, IState
    {
        public string Name { get; set; }
    }

    public class Tag : DataObject, ITag
    {
        public string Name { get; set; }
    }

    public class Author : DataObject, IAuthor
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }

    public class Publisher : DataObject, IPublisher
    {
        public string Name { get; set; }
    }
}

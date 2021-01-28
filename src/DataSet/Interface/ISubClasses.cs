namespace DataSet
{
    public interface IPublicationType : IDataObject
    {
        string Name { get; set; }
    }

    public interface IState : IDataObject
    {
        string Name { get; set; }
    }

    public interface ITag : IDataObject
    {
        string Name { get; set; }
    }

    public interface IAuthor : IDataObject
    {
        string Name { get; set; }
        string Surname { get; set; }
    }

    public interface IPublisher : IDataObject
    {
        string Name { get; set; }
    }
}
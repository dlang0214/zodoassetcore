namespace HZC.Infrastructure
{
    public interface IAppUser<T>
    {
        T Id { get; set; }

        string Name { get; set; }
    }

    public interface IAppUser : IAppUser<int>
    { }
}

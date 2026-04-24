namespace NovaFashion.API.Shared.Abstractions
{
    public interface IHasKey<T>
    {
        T Id { get; set; }
    }
}

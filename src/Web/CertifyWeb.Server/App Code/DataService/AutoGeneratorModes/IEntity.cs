namespace AutoGeneratorModes
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
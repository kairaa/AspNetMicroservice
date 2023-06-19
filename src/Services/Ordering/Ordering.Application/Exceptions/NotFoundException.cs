namespace Ordering.Application.Exceptions
{
    //kursta exception yerine applicationexception'dan miras aliyor
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
}

namespace eCommerce.Model.Extensions;

public class ClinetException : Exception
{
    public ClinetException(string message) : base(message)
    {
    }

    public ClinetException(string message, Exception inner) : base(message, inner)
    {
    }
}
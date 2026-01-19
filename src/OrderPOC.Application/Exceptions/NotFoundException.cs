namespace OrderPOC.Application.Exceptions;

public sealed class NotFoundException(string message) : Exception(message)
{
    public int StatusCode => 404;
}
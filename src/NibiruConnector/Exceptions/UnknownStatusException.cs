namespace NibiruConnector.Exceptions;

public class UnknownStatusException : Exception
{
    public UnknownStatusException(int code) : base(
        $"Unknown status code {code}")
    {
    }

    public UnknownStatusException(int code, string message) : base(
        $"Unknown status code {code}: {message}")
    {
    }
}

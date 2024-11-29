using System.Diagnostics.CodeAnalysis;

namespace GenerateTemplate.Domain.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class ExceptionFilter : Exception
{
    public ExceptionFilter(string message) : base(message) { }

    public ExceptionFilter(string message, Exception inner) : base(message, inner) { }
}
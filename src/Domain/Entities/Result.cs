using System;

namespace CHKS.Domain.Entities;

public abstract record Result
{
    public sealed record Success(object Data) : Result;
    public sealed record Failure(string Code, string Message,
        Dictionary<string, string[]> ValidationErrors = null) : Result;
}

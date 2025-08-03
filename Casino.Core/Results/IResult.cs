namespace Casino.Core.Results;

public interface IResult
{
    bool IsSuccess { get; }
    string Message { get; }
    decimal? NewBalance { get; }
}
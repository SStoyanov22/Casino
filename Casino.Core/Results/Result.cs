namespace Casino.Core.Results;

public abstract class Result : IResult
{
    public bool IsSuccess { get; protected set; }
    public string Message { get; protected set; }
    public decimal? NewBalance { get; protected set; }
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}
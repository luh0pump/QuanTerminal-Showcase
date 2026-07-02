namespace QuanTerminal.Core.Abstractions;

public interface IDataSource
{
    string SourceId { get; }
    bool IsPointInTime { get; }

    Task<SeriesResult> FetchAsync(
        SeriesRequest request,
        DateOnly? asOf = null,
        CancellationToken ct = default);
}

public sealed record SeriesRequest(string SeriesCode, DateOnly Start, DateOnly End);

public sealed record SeriesResult(
    string SourceId,
    string SeriesCode,
    IReadOnlyList<Observation> Observations,
    bool IsRevised);

public readonly record struct Observation(DateOnly Date, double Value);

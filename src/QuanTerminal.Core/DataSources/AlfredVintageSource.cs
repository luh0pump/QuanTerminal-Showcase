using QuanTerminal.Core.Abstractions;

namespace QuanTerminal.Core.DataSources;

public sealed class AlfredVintageSource(IHttpFetcher fetcher, ISeriesCache cache) : IDataSource
{
    public string SourceId => "fred-alfred";
    public bool IsPointInTime => true;

    public async Task<SeriesResult> FetchAsync(
        SeriesRequest request,
        DateOnly? asOf = null,
        CancellationToken ct = default)
    {
        if (cache.TryGet(SourceId, request, asOf, out var cached))
            return cached;

        var vintage = asOf ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var raw = await fetcher.GetVintageAsync(request.SeriesCode, vintage, ct);

        var observations = raw
            .Where(o => o.Date >= request.Start && o.Date <= request.End)
            .Select(o => new Observation(o.Date, o.Value))
            .ToList();

        var result = new SeriesResult(SourceId, request.SeriesCode, observations, IsRevised: false);

        cache.Put(SourceId, request, asOf, result);
        return result;
    }
}

public interface IHttpFetcher
{
    Task<IReadOnlyList<Observation>> GetVintageAsync(
        string seriesCode, DateOnly vintage, CancellationToken ct);
}

public interface ISeriesCache
{
    bool TryGet(string sourceId, SeriesRequest req, DateOnly? asOf, out SeriesResult result);
    void Put(string sourceId, SeriesRequest req, DateOnly? asOf, SeriesResult result);
}

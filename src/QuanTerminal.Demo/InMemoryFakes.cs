using QuanTerminal.Core.Abstractions;
using QuanTerminal.Core.DataSources;

namespace QuanTerminal.Demo;

public sealed class InMemoryFetcher : IHttpFetcher
{
    public Task<IReadOnlyList<Observation>> GetVintageAsync(
        string seriesCode, DateOnly vintage, CancellationToken ct)
    {
        var start = new DateOnly(2025, 1, 1);
        var obs = Enumerable.Range(0, 18)
            .Select(i => new Observation(start.AddMonths(i), 100 + i * 1.3 + (i % 3)))
            .ToList();

        return Task.FromResult<IReadOnlyList<Observation>>(obs);
    }
}

public sealed class InMemoryCache : ISeriesCache
{
    private readonly Dictionary<string, SeriesResult> _store = new();

    public bool TryGet(string sourceId, SeriesRequest req, DateOnly? asOf, out SeriesResult result)
    {
        var key = Key(sourceId, req, asOf);
        if (_store.TryGetValue(key, out var found))
        {
            result = found;
            return true;
        }
        result = null!;
        return false;
    }

    public void Put(string sourceId, SeriesRequest req, DateOnly? asOf, SeriesResult result)
        => _store[Key(sourceId, req, asOf)] = result;

    private static string Key(string sourceId, SeriesRequest req, DateOnly? asOf)
        => $"{sourceId}:{req.SeriesCode}:{req.Start}:{req.End}:{asOf}";
}

public sealed class DemoVintageGate : IVintageGate
{
    public void Assert(SeriesResult series, DataUsage usage)
    {
        if (usage == DataUsage.BacktestRecord && series.IsRevised)
            throw new VintageViolationException($"revised data blocked for backtest record: {series.SeriesCode}");
    }
}

public sealed class DemoStatisticsSidecar : QuanTerminal.Validation.IStatisticsSidecar
{
    public Task<QuanTerminal.Validation.DeflationResult> DeflateAsync(
        QuanTerminal.Validation.CandidateSummary candidate, CancellationToken ct)
    {
        // hurdle grows with trial count — mirrors the sidecar's actual shape
        var hurdle = 0.02 * Math.Sqrt(Math.Log(Math.Max(candidate.TrialCount, 2)));
        return Task.FromResult(new QuanTerminal.Validation.DeflationResult(
            ObservedMetric: candidate.ObservedMetric,
            Hurdle: hurdle,
            AutocorrelationFlag: candidate.OutOfSampleN < 30));
    }
}

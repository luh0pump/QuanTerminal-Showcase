using QuanTerminal.Core.Abstractions;

namespace QuanTerminal.Core.Pipeline;

// Ordering is mandatory: transform in reference-month time, then align.
public sealed class PointInTimePipeline(IVintageGate gate)
{
    public AlignedPanel Build(
        IReadOnlyList<SeriesResult> series,
        Func<SeriesResult, SeriesResult> transform,
        DataUsage usage)
    {
        foreach (var s in series)
            gate.Assert(s, usage);

        var transformed = series.Select(transform).ToList();
        return Align(transformed);
    }

    private static AlignedPanel Align(IReadOnlyList<SeriesResult> transformed)
    {
        var dates = transformed
            .SelectMany(s => s.Observations.Select(o => o.Date))
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        var columns = transformed.ToDictionary(
            s => s.SeriesCode,
            s => s.Observations.ToDictionary(o => o.Date, o => o.Value));

        return new AlignedPanel(dates, columns);
    }
}

public sealed record AlignedPanel(
    IReadOnlyList<DateOnly> Dates,
    IReadOnlyDictionary<string, Dictionary<DateOnly, double>> Columns);

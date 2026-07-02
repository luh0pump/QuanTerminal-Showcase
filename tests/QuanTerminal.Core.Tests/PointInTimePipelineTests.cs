using QuanTerminal.Core.Abstractions;
using QuanTerminal.Core.Pipeline;
using Xunit;

namespace QuanTerminal.Core.Tests;

public class PointInTimePipelineTests
{
    private sealed class PassthroughGate : IVintageGate
    {
        public void Assert(SeriesResult series, DataUsage usage)
        {
            if (usage == DataUsage.BacktestRecord && series.IsRevised)
                throw new VintageViolationException($"revised data blocked: {series.SeriesCode}");
        }
    }

    [Fact]
    public void BacktestRecord_rejects_revised_series()
    {
        var pipeline = new PointInTimePipeline(new PassthroughGate());
        var revised = new SeriesResult("src", "X", new[] { new Observation(new(2026, 1, 1), 1.0) }, IsRevised: true);

        Assert.Throws<VintageViolationException>(() =>
            pipeline.Build(new[] { revised }, s => s, DataUsage.BacktestRecord));
    }

    [Fact]
    public void Discovery_allows_revised_series()
    {
        var pipeline = new PointInTimePipeline(new PassthroughGate());
        var revised = new SeriesResult("src", "X", new[] { new Observation(new(2026, 1, 1), 1.0) }, IsRevised: true);

        var panel = pipeline.Build(new[] { revised }, s => s, DataUsage.Discovery);

        Assert.Single(panel.Columns);
    }

    [Fact]
    public void Transform_runs_before_alignment()
    {
        var pipeline = new PointInTimePipeline(new PassthroughGate());
        var s = new SeriesResult("src", "X",
            new[] { new Observation(new(2026, 1, 1), 2.0), new Observation(new(2026, 2, 1), 4.0) },
            IsRevised: false);

        var panel = pipeline.Build(new[] { s },
            r => r with { Observations = r.Observations.Select(o => o with { Value = o.Value * 2 }).ToList() },
            DataUsage.BacktestRecord);

        Assert.Equal(4.0, panel.Columns["X"][new(2026, 1, 1)]);
        Assert.Equal(8.0, panel.Columns["X"][new(2026, 2, 1)]);
    }
}

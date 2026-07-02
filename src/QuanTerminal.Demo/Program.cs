using QuanTerminal.Core.Abstractions;
using QuanTerminal.Core.DataSources;
using QuanTerminal.Core.Pipeline;
using QuanTerminal.Demo;
using QuanTerminal.Validation;

var source = new AlfredVintageSource(new InMemoryFetcher(), new InMemoryCache());

var request = new SeriesRequest("DEMO_SERIES", new DateOnly(2025, 1, 1), new DateOnly(2026, 6, 1));
var series = await source.FetchAsync(request, asOf: new DateOnly(2026, 6, 1));

Console.WriteLine($"fetched {series.Observations.Count} observations from {series.SourceId}");

var pipeline = new PointInTimePipeline(new DemoVintageGate());
var panel = pipeline.Build(
    new[] { series },
    s => s with { Observations = s.Observations.Select(o => o with { Value = o.Value / 100.0 - 1.0 }).ToList() },
    DataUsage.BacktestRecord);

Console.WriteLine($"aligned panel: {panel.Dates.Count} dates, {panel.Columns.Count} series");

var returns = panel.Columns["DEMO_SERIES"].Values.ToArray();
var candidate = new CandidateSummary(TrialCount: 40, ObservedMetric: Mean(returns) / StdDev(returns), OutOfSampleN: 18);

var gate = new ValidationGate(new DemoStatisticsSidecar());
var verdict = await gate.EvaluateAsync(candidate);

Console.WriteLine($"observed metric: {candidate.ObservedMetric:F4}");
Console.WriteLine($"gate verdict: {verdict.Outcome}{(verdict.Reason is null ? "" : $" ({verdict.Reason})")}");

static double Mean(double[] xs) => xs.Average();
static double StdDev(double[] xs)
{
    var m = Mean(xs);
    var variance = xs.Select(x => (x - m) * (x - m)).Sum() / (xs.Length - 1);
    return Math.Sqrt(variance);
}

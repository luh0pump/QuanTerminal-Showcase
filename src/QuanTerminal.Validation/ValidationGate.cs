namespace QuanTerminal.Validation;

public sealed class ValidationGate(IStatisticsSidecar sidecar)
{
    public async Task<GateVerdict> EvaluateAsync(CandidateSummary candidate, CancellationToken ct = default)
    {
        var deflated = await sidecar.DeflateAsync(candidate, ct);

        if (deflated.ObservedMetric < deflated.Hurdle)
            return GateVerdict.Kill($"below deflated hurdle ({deflated.ObservedMetric:F4} < {deflated.Hurdle:F4})");

        if (deflated.AutocorrelationFlag)
            return GateVerdict.Downgrade("serial-correlation flag");

        return GateVerdict.Pass();
    }
}

public interface IStatisticsSidecar
{
    Task<DeflationResult> DeflateAsync(CandidateSummary candidate, CancellationToken ct);
}

public sealed record CandidateSummary(int TrialCount, double ObservedMetric, int OutOfSampleN);
public sealed record DeflationResult(double ObservedMetric, double Hurdle, bool AutocorrelationFlag);

public sealed record GateVerdict(GateOutcome Outcome, string? Reason)
{
    public static GateVerdict Pass() => new(GateOutcome.Pass, null);
    public static GateVerdict Downgrade(string r) => new(GateOutcome.Downgrade, r);
    public static GateVerdict Kill(string r) => new(GateOutcome.Kill, r);
}

public enum GateOutcome { Pass, Downgrade, Kill }

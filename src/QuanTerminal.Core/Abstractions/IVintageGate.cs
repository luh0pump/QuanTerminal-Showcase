namespace QuanTerminal.Core.Abstractions;

public interface IVintageGate
{
    void Assert(SeriesResult series, DataUsage usage);
}

public enum DataUsage
{
    Discovery,
    BacktestRecord
}

public sealed class VintageViolationException(string message) : Exception(message);

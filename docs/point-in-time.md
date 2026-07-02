# Point-in-time data discipline

Look-ahead bias is the quiet killer of backtests: if a feature is built from data that
wasn't actually available on the decision date, the backtest measures hindsight, not
edge. QuanTerminal removes two of the most common leaks structurally.

## 1. Vintages, not revisions

Macro series get revised for months or years after first publication. Using the
*current* value of, say, an employment figure to reconstruct a decision made two years
ago is a leak.

- Sources that expose vintages (e.g. ALFRED) are queried **as-of** the decision date, so
  the pipeline sees only what was published then.
- Sources that only return revised data are **flagged**. Revised data is allowed for
  discovery but blocked from any backtest record by the vintage gate
  (`IVintageGate` / `DataUsage.BacktestRecord`).

## 2. Transform before align

The mandatory ordering is: **transform each series in its own reference-month time,
then align onto a common publication-aware timeline.**

Doing it the other way — align first, transform second — smears information across the
alignment boundary and can pull future observations into a feature. `PointInTimePipeline`
exposes no code path that aligns before transforming; the order is baked into the API.

## Why enforce it in code, not convention

Conventions get forgotten under deadline pressure. A gate that throws, and a pipeline
whose only public method does the steps in the correct order, mean the safe path is the
only path.

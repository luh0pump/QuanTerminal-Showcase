# Validation methodology

The central risk in strategy research is **selection under multiple testing**: try enough
ideas and the luckiest one looks brilliant. QuanTerminal's answer is a layered gate that
raises the bar in proportion to how much searching was done.

## honest-N — count, don't drop

Every hypothesis attempt is appended to a register — including the ones that failed.
The trial count that feeds the statistics is the *honest* one, not just the survivors.
Dropping failed trials is the single most common way backtests lie to themselves.

## Deflated hurdle

A candidate's observed performance is compared against the expected best-of-N under the
null: with `N` trials, the maximum Sharpe you'd see by chance rises, so the pass mark
rises too. The sidecar implements a two-term approximation of that expected maximum
(see [`python/sidecar/deflation.py`](../python/sidecar/deflation.py)).

Two channels matter and can pull in opposite directions:

- more trials → higher hurdle (the obvious effect);
- correlated trials → lower effective variance of the trial estimates, which can *lower*
  the hurdle. Adding a near-duplicate hypothesis is not free, but it is not simply "one
  more trial" either.

## Staged rigor by cost

Checks are ordered cheapest-first so expensive validation only runs on candidates that
survive the cheap filters: discovery → paper-trade → deflated kill-gate → out-of-sample →
demo → manual live.

## Reversible vs. hard gates

Not every flag is fatal. Serial-correlation flags, for instance, are a **reversible
downgrade** — momentum edges are autocorrelated by construction — whereas failing the
deflated hurdle is a **hard kill**. The gate returns a verdict (`Pass` / `Downgrade` /
`Kill`) rather than a boolean so this distinction is explicit.

---

*Thresholds, the live register, cluster/spectral deflation, and BH-FDR batch correction
live in the private research repository.*

"""Deflated Sharpe hurdle: the pass mark rises with the number of trials."""

from __future__ import annotations

from dataclasses import dataclass
import numpy as np
from scipy.stats import norm

EULER_MASCHERONI = 0.5772156649015329


@dataclass(frozen=True)
class DeflationInput:
    returns: np.ndarray
    trial_count: int
    variance_of_trials: float


@dataclass(frozen=True)
class DeflationOutput:
    observed_sharpe: float
    hurdle_sharpe: float
    passed: bool


def _expected_max_sharpe(n_trials: int, variance_of_trials: float) -> float:
    if n_trials < 2:
        return 0.0
    e = np.e
    z1 = norm.ppf(1.0 - 1.0 / n_trials)
    z2 = norm.ppf(1.0 - 1.0 / (n_trials * e))
    g = (1.0 - EULER_MASCHERONI) * z1 + EULER_MASCHERONI * z2
    return float(np.sqrt(variance_of_trials) * g)


def observed_sharpe(returns: np.ndarray) -> float:
    mu = returns.mean()
    sd = returns.std(ddof=1)
    return float(mu / sd) if sd > 0 else 0.0


def deflate(inp: DeflationInput) -> DeflationOutput:
    obs = observed_sharpe(inp.returns)
    hurdle = _expected_max_sharpe(inp.trial_count, inp.variance_of_trials)
    return DeflationOutput(observed_sharpe=obs, hurdle_sharpe=hurdle, passed=obs > hurdle)

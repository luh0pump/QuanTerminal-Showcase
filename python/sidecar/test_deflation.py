import numpy as np
from deflation import DeflationInput, deflate, observed_sharpe, _expected_max_sharpe


def test_hurdle_rises_with_trial_count():
    low = _expected_max_sharpe(5, 0.02)
    high = _expected_max_sharpe(500, 0.02)
    assert high > low


def test_single_trial_has_zero_hurdle():
    assert _expected_max_sharpe(1, 0.02) == 0.0


def test_observed_sharpe_positive_drift():
    rng = np.random.default_rng(42)
    r = rng.normal(0.5, 1.0, 250)
    assert observed_sharpe(r) > 0


def test_deflate_flags_lucky_looking_candidate():
    rng = np.random.default_rng(0)
    # near-zero-edge series against a large trial count -> should not pass
    r = rng.normal(0.01, 1.0, 120)
    out = deflate(DeflationInput(returns=r, trial_count=1000, variance_of_trials=0.05))
    assert out.passed is False

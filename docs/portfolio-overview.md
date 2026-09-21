# Portfolio Overview

## Purpose

This public repository exists to demonstrate engineering capability without exposing the private QuanTerminal research system.

## Problem class

QuanTerminal is a long-running quantitative research platform where correctness depends on more than producing a backtest. The system has to preserve:

- point-in-time data semantics
- reproducible transformations
- explicit research and validation stages
- independent evidence for important state changes
- strict separation between experimental results and product-facing state

## My role

- product and system architecture
- research-methodology design
- prioritisation and release decisions
- acceptance-gate and evidence design
- AI-assisted implementation orchestration
- independent verification of implementation and results

## What the public code is meant to prove

### Software decomposition
Data access, transformation, validation and presentation are separated instead of being embedded in monolithic research scripts.

### Validation discipline
Tests and statistical validation are first-class parts of the workflow, not an afterthought.

### Reproducibility
The system is designed around deterministic inputs, explicit state and reviewable evidence.

### AI-assisted engineering with control
AI coding agents are used as an implementation layer, while specifications, acceptance criteria and final release decisions remain explicit and independently reviewable.

## Deliberate exclusions

This showcase does not disclose:

- profitable or experimental strategy definitions
- live signals
- private datasets
- detailed broker/execution configuration
- proprietary research history
- current private operational state

Those exclusions are intentional and do not reduce the engineering patterns demonstrated by the public repository.

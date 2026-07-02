# QuanTerminal — Architecture Showcase

> Public **engineering showcase** of QuanTerminal, a quantitative research terminal.
> This repository demonstrates the system architecture, data-integration patterns,
> validation methodology, and CI/testing discipline of the project.
>
> It intentionally contains **no live strategy logic, no signals, and no research
> registers** — only the reusable engineering scaffolding. The production research
> repository is private.

---

## What this project is

QuanTerminal is a cross-asset **quantitative research terminal** — explicitly *not* a
trading bot. Its purpose is to systematically discover, validate, and forward-test
statistical edges in futures / FX / macro markets under strict scientific discipline
(point-in-time data, multiple-testing correction, out-of-sample gating).

The core design principle: **AI as a force multiplier.** Specifications are written by
a human architect, implemented with AI assistance, then every result is independently
re-verified by hand. Nothing enters the research record without a reproducible,
byte-exact anchor.

## System at a glance

```
┌──────────────────────────────────────────────────────────────┐
│  Blazor SSR frontend (read-only)                             │
├──────────────────────────────────────────────────────────────┤
│  QuanTerminal.Core  (C# / .NET 8)                            │
│   • IDataSource adapters (FRED/ALFRED, Databento, CFTC, ECB) │
│   • Point-in-time pipeline (transform → align)               │
│   • DuckDB + Parquet cache-through warehouse                 │
├──────────────────────────────────────────────────────────────┤
│  Python statistics sidecar                                   │
│   • Deflation / multiple-testing gates (PSR / DSR)           │
├──────────────────────────────────────────────────────────────┤
│  Storage: DuckDB (local)  ·  Cloudflare R2 (raw market data) │
└──────────────────────────────────────────────────────────────┘
```

## Engineering highlights

- **Adapter-based data integration** — every source (macro vintages, futures, positioning)
  implements a single `IDataSource` contract, so new providers plug in without touching
  the pipeline. See [`src/QuanTerminal.Core/DataSources`](src/QuanTerminal.Core/DataSources).
- **Point-in-time discipline** — transforms are applied in reference-month time *before*
  alignment, never after. This prevents look-ahead bias structurally rather than by
  convention. See [`docs/point-in-time.md`](docs/point-in-time.md).
- **Validation as a first-class gate** — a Python sidecar computes deflated performance
  hurdles; the C# side treats validation as a hard gate, not an afterthought. Concept
  documented in [`docs/validation-methodology.md`](docs/validation-methodology.md).
- **Reproducibility** — cache-through DuckDB/Parquet warehouse, additive-only raw storage,
  CI that runs the full test suite on every push.

## Tech stack

| Layer            | Technology                                  |
|------------------|---------------------------------------------|
| Core terminal    | C# / .NET 8                                  |
| Statistics       | Python 3.11 (NumPy / SciPy / pandas)         |
| Frontend         | Blazor SSR (read-only)                       |
| Warehouse        | DuckDB + Parquet                             |
| Raw market data  | Cloudflare R2 (S3-compatible), rclone        |
| CI               | GitHub Actions                               |
| Containerisation | Docker                                       |

## Repository layout

```
src/
  QuanTerminal.Core/          # data sources, pipeline, abstractions
  QuanTerminal.Validation/    # validation-gate orchestration (C# side)
python/
  sidecar/                    # statistics sidecar (deflation gates)
tests/
  QuanTerminal.Core.Tests/    # unit tests for adapters + pipeline
docs/                         # architecture & methodology notes
```

## Running the tests

```bash
dotnet test
cd python && pytest
```

---

*Built by [@luh0pump](https://github.com/luh0pump). This showcase mirrors the
architecture of a larger private research codebase.*

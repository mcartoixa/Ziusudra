# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Ziusudra is a Windows-native client for [Deluge](https://www.deluge-torrent.org/) torrent servers. It has two parts with different audiences:

- **`Ziusudra.Core`** — a protocol-agnostic .NET library implementing the Deluge RPC protocol. It is meant to be reusable by other projects (including third parties), so treat its public surface as an API. This is where the substance is.
- **`Ziusudra.Desktop`** — an early-stage Windows Forms client (`net10.0-windows`). It mainly demonstrates how `Core` is meant to be consumed; it is not the focus.
- **`Ziusudra.Core.Tests`** — xUnit tests for `Core` (has `InternalsVisibleTo`, so tests reach internal members).

Everything targets `net10.0`; the SDK is pinned in `global.json` (`10.0.301`, `rollForward: latestFeature`). `ImplicitUsings` and `Nullable` are enabled.

## Architecture

`Core` is two layers. Understanding both requires reading across several files, so the big picture is below.

### Rencode (`Ziusudra.Core/Rencode`)

Deluge serializes with "rencode" (a compact bencode variant). This implements it, mirroring the reference at https://github.com/aresch/rencode (see its `SPEC.md` for exact byte ranges).

- `IEncoder` / `Encoder` / `Encoder<T>` are the abstraction; `RencodeStreamReader` / `RencodeStreamWriter` drive encoders.
- **Encoder order matters.** `Encoder.Encoders` is a fixed ordered list: `DictionaryEncoder` must precede `ListEncoder` (an `IDictionary` is also an `ICollection`), and `IntegerEncoder` is last and write-only. On read, the header byte selects the encoder via `CanRead`.
- Numeric encoders form a delegation chain (`BigInteger → Int64 → Int32 → Int16 → SByte`) that picks the **most compact** wire form. Consequence: values round-trip to their most compact CLR type — e.g. an `int` `5` reads back as `sbyte` `5`. Compare by value, not type.
- All wire-facing string/number formatting must be culture-invariant (`CultureInfo.InvariantCulture`); a culture-dependent negative sign or separator corrupts the wire format.

### DelugeRpc (`Ziusudra.Core/DelugeRpc`)

- **Wire framing:** each message is `[version:1 byte][length:4 bytes big-endian][zlib-compressed rencoded body]`, matching Deluge's `deluge/transfer.py`. `RpcStreamWriter` frames requests; `RpcStreamReader` reads exactly `length` bytes then decompresses that bounded buffer (reading the exact frame is required — decompressing straight off the socket over-reads into the next frame and desyncs).
- **Message model:** `IMessage → IExchangeMessage / IServerMessage → IClientRequest / IServerReply`. `RpcMessageType` (RESPONSE/ERROR/EVENT) is the first element of every server message.
- **Requests:** `RpcRequest<TResponse>` is the base. Each concrete request lives under `Daemon/` or `Core/` to match Deluge's `daemon.*` / `core.*` method namespaces, and nests its own `Response : RpcResponse`. Each request gets a random `Id` used to correlate the reply.
- **Typed events/errors:** `RpcEvent.CreateFromValues` and `RpcServerException.CreateFromValues` use reflection to map the server's event/error name to a matching subclass. To add one, create a subclass (under `Events/`, named exactly after the Deluge event) with an `internal` constructor taking `ICollection`.
- **`RpcClient`** is the orchestrator: TLS over TCP, then a background `RpcMessageLoop` that reads messages, completes the pending `TaskCompletionSource` matching each reply's `Id`, and raises `RpcEventReceived` for events. Fatal transport errors fault **all** pending requests and stop the loop; a decode error on a single received frame is logged and skipped (matching the daemon's behavior). `RpcMessageLoop` is deliberately split out from `RpcClient` behind the internal `IServerMessageReader` seam so its dispatch/error logic is unit-testable without sockets.
- **TLS:** Deluge daemons use a self-signed certificate and the reference client does not validate it, so `RpcClient` accepts any certificate by default. Validation/pinning is opt-in via `RpcClientOptions.CertificateValidationCallback`.

### Localization

User-facing and exception strings live in `SR.resx`; `SR.Designer.cs` is the generated accessor. Do not hardcode such messages — add a resource and reference `SR.<Name>`. This environment has no Visual Studio to regenerate the designer, so when adding a string **edit both `SR.resx` and `SR.Designer.cs` by hand** and keep them in sync.

## Building and testing

There is a custom MSBuild build system layered on top of the SDK. `Ziusudra.proj` + `.build/Common.targets` define the pipeline targets (`Clean`, `Compile`, `Test`, `Analyze`, `Package`, `Release`, `Build` = Compile+Test+Analyze, `Rebuild`). Build outputs are redirected to `./tmp` (binaries, test results, coverage) and tooling to `./.tmp`.

Full pipeline (what CI runs):

```
build.cmd [clean|compile|test|analyze|package|release|build|rebuild] [/log] [/NoPause]   # Windows
./build.sh                                                                                # POSIX
dotnet msbuild Ziusudra.proj -t:Release        # equivalent, no wrapper
```

The `Test` target runs VSTest with `XPlat Code Coverage` + the xUnit logger, then ReportGenerator emits cobertura/HTML into `tmp/`.

For fast iteration, use the SDK directly:

```
dotnet build Ziusudra.sln
dotnet test Ziusudra.Core.Tests/Ziusudra.Core.Tests.csproj                       # all tests
dotnet test Ziusudra.Core.Tests/Ziusudra.Core.Tests.csproj --filter "FullyQualifiedName~RencodeRoundTripTests"   # one class/test
```

## Analyzers and warnings

- **`Core` treats warnings as errors in Release only** (`Ziusudra.Core.csproj`). Debug is warning-tolerant for iterating. Before pushing, build Core in Release to catch what CI will: `dotnet build Ziusudra.Core/Ziusudra.Core.csproj -c Release`.
- Analyzer severity is elevated to `warning` by a **Core-scoped** `Ziusudra.Core/.editorconfig` (it inherits formatting from the repo-root `.editorconfig`). A few performance analyzers are intentionally downgraded there (`CA1848`, `CA1863`, `CA1859`); CA rules that don't fit the design are suppressed at the declaration with a justification rather than globally disabled.
- CA diagnostics the IDE shows but a plain build hides are below `warning` severity by default. To surface them from the command line: `dotnet build -p:AnalysisMode=All --no-incremental`.

## Conventions

- Private fields are `_PascalCase` (enforced by naming rules in `.editorconfig`).
- New tests use `async Task`, never `async void` (xUnit is removing support for the latter).
- `Core` has no build-time dependency on Deluge running; behavior against a real daemon is not covered by the automated tests (the stream/framing and message-loop layers are unit-tested over in-memory streams).

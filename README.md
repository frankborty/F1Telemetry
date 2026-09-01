# F1 Telemetry

Applicazione desktop **WPF** (.NET 10) per la visualizzazione in tempo reale della telemetria di F1. I dati vengono letti dal gioco via **UDP** (porta 20777) oppure da una **sorgente simulata** per lo sviluppo: il passaggio tra le due avviene a runtime via configurazione.

![.NET](https://img.shields.io/badge/.NET-10.0-purple) ![Platform](https://img.shields.io/badge/platform-Windows-blue)

## Funzionalità

- **Telemetria in tempo reale** del pilota: velocità, marcia, RPM con rev-lights, pedali (throttle/brake), sterzo, frizione, DRS.
- **Gomme e freni**: temperature (superficie/interno), pressioni e tipo di asfalto per i 4 angoli.
- **Power unit**: temperatura motore, batteria/ERS (deploy e recovery).
- **Race**: carburante, consumo, giro, settore e tempo giro.
- **Dark theme** tipo cockpit, aggiornamento UI a ~60fps.
- **Sorgente selezionabile a runtime**: dati reali F1 (`F1Game.UDP`) o simulati.
- **Reset** del display con conferma visiva.

## Stack

- **Linguaggio:** C# (.NET 10)
- **UI:** WPF (Windows Presentation Foundation) con pattern MVVM
- **Pattern:** Producer-Consumer con `System.Threading.Channels`
- **DI/Hosting:** `Microsoft.Extensions.Hosting` + `Microsoft.Extensions.DependencyInjection`
- **Parsing UDP F1:** pacchetto NuGet [`F1Game.UDP`](https://www.nuget.org/packages/F1Game.UDP/) v26 (F1 25 / 2026 Season Pack)

## Architettura

La soluzione è organizzata in 3 progetti con separazione netta dei confini:

```
┌──────────────────────────────┐
│         App (WPF)            │  Composition root + MVVM
│  MainWindow / MainViewModel  │  Converters, RelayCommand, appsettings.json
└──────────────┬───────────────┘
               │
┌──────────────▼───────────────┐
│       Infrastructure         │  Unico progetto accoppiato al gioco
│  F1TelemetrySource (UDP)     │  Dipendenza: F1Game.UDP
└──────────────┬───────────────┘
               │
┌──────────────▼───────────────┐
│          Core                │  Dominio e orchestrazione (pulito)
│  TelemetryData, interfacce,  │  Producer/Consumer via Channel<T>
│  FakeTelemetrySource         │  Nessuna dipendenza da F1/UDP
└──────────────────────────────┘
```

### Flusso dati

```
Sorgente (Fake o F1)                 Channel<T> (capacity 100)          UI
┌────────────────────┐  IAsyncEnumerable  ┌──────────────────────┐   event   ┌────────────┐
│ ITelemetrySource    │ ─────────────────► │ TelemetryProducer    │           │ Consumer → │
│ yield return ~10Hz  │   push-based, no   │ (BackgroundService)  │──► Writer │ ViewModel  │
└────────────────────┘   polling          └──────────────────────┘   │        └────────────┘
                                                                      │
                     ┌──────────────────────┐  IAsyncEnumerable      │
                     │ TelemetryConsumer    │ ◄──────────────────────┘
                     │ (BackgroundService)  │  Reader
                     └──────────────────────┘
```

### Scelte chiave

- **`ITelemetrySource` è push-based**: `IAsyncEnumerable<TelemetryData> GetTelemetryAsync(ct)`. L'UDP è un flusso push, quindi è la sorgente a decidere il cadenzamento (100ms per la fake, ogni pacchetto `CarTelemetry` per la reale). Il producer non fa polling.
- **Disaccoppiamento Producer/Consumer** con `Channel<T>` bounded: il canale applica backpressure e separa chi produce da chi consuma.
- **Lo switch Fake/F1 è pura configurazione DI**: in `App.xaml.cs` l'implementazione di `ITelemetrySource` viene scelta da `Telemetry:Source` (`appsettings.json` o argomento CLI).
- **La UI non conosce la sorgente**: i dati arrivano come `TelemetryData` e vengono pubblicati sul thread UI da un `DispatcherTimer` (16ms), tenendo i binding lontani dal thread di rete.

## Struttura del progetto

```
F1Telemetry/
├── src/
│   ├── F1Telemetry.App/            # WPF UI (composition root, MVVM)
│   ├── F1Telemetry.Core/           # Modello dati, interfacce, orchestrazione
│   └── F1Telemetry.Infrastructure/ # Sorgente UDP reale (F1Game.UDP)
├── tests/                          # Test (da implementare)
├── F1Telemetry.slnx                # Solution file
├── LICENSE.txt                     # Licenza MIT
└── README.md
```

## Configurazione della sorgente

La sorgente viene scelta dalla chiave `Telemetry:Source`:

- `Fake` (default) → dati simulati generati casualmente ogni 100ms
- `F1` → lettura UDP dal gioco sulla porta 20777

Modifica `src/F1Telemetry.App/appsettings.json`:

```json
{
  "Telemetry": {
    "Source": "F1"
  }
}
```

Oppure da riga di comando:

```bash
dotnet run --project src/F1Telemetry.App/F1Telemetry.App.csproj -- --Telemetry:Source=F1
```

> Nota: nel gioco abilita la telemetria UDP su **porta 20777** prima di avviare l'applicazione.

## Build ed Esecuzione

```bash
# Build della soluzione
dotnet build F1Telemetry.slnx

# Esecuzione
dotnet run --project src/F1Telemetry.App/F1Telemetry.App.csproj
```

Oppure in Visual Studio: apri `F1Telemetry.slnx`, imposta `F1Telemetry.App` come progetto di avvio e premi F5.

## Stato del Progetto

- ✅ Pipeline push-based (Producer/Consumer via `Channel<T>`)
- ✅ Sorgente UDP reale (`F1TelemetrySource`) con merging di CarTelemetry + CarStatus + LapData
- ✅ Sorgente simulata e switch a runtime via configurazione
- ✅ Dashboard UI dark theme completa
- ⬜ Test automatici (`tests/` vuota)
- ⬜ Chiusura esplicita del socket UDP (`IDisposable`)
- ⬜ Unità di misura coerenti tra sorgente reale e simulata (batteria/ERS)

## Licenza

MIT

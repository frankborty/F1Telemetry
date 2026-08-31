# F1 Telemetry

Applicazione desktop WPF per la visualizzazione in tempo reale di telemetria Formula 1, basata su un'architettura producer-consumer con pipeline asincrona.

## Stack Tecnologico

- **Linguaggio:** C# (.NET 10.0)
- **UI:** WPF (Windows Presentation Foundation)
- **Pattern:** Producer-Consumer con `System.Threading.Channels`
- **DI:** Microsoft.Extensions.DependencyInjection + Hosting

## Struttura del Progetto

```
F1Telemetry/
├── src/
│   ├── F1Telemetry.App/            # Applicazione WPF (UI layer)
│   ├── F1Telemetry.Core/           # Modelli dati e servizi di dominio
│   └── F1Telemetry.Infrastructure/ # Layer infrastruttura (futuro)
├── tests/                          # Test (da implementare)
├── F1Telemetry.slnx                # Solution file
└── LICENSE.txt                     # Licenza MIT
```

## Funzionalità

- Modello telemetria con 16 parametri (velocità, RPM, marcia, throttle, freno, DRS, ERS, carburante, giro, settore, ecc.)
- Producer che genera dati telemetria simulati
- Consumer che consuma i dati in modo asincrono via `IAsyncEnumerable`
- Pipeline basata su `Channel<T>` per la comunicazione tra producer e consumer
- Dependency Injection con Generic Host

## Requisiti

- Windows 10/11
- .NET 10.0 SDK
- Visual Studio 2022 (consigliato) oppure CLI `dotnet`

## Build ed Esecuzione

```bash
# Build
dotnet build F1Telemetry.slnx

# Esegui
dotnet run --project src/F1Telemetry.App/F1Telemetry.App.csproj
```

Oppure aprilo in Visual Studio, imposta `F1Telemetry.App` come startup project e premi F5.

## Stato del Progetto

Progetto in fase iniziale — l'architettura è definita ma la UI è una finestra vuota e i dati sono mock generati casualmente. Il layer Infrastructure è un placeholder.

## Licenza

MIT

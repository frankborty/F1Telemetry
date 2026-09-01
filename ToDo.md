# F1Telemetry — Roadmap

## Stato attuale

* Applicazione WPF `.NET 10`
* Telemetria proveniente da:

  * F1 via UDP
  * Fake source per sviluppo/test
* Architettura a 3 layer:

  * `F1Telemetry.Core`
  * `F1Telemetry.Infrastructure`
  * `F1Telemetry.App`
* Producer/Consumer tramite `Channel<T>`
* UI aggiornata a circa 60 FPS tramite `DispatcherTimer`
---

# Prossimi passi



---1
## 3. Gestione della cancellazione UDP

Verificare che la cancellazione dell'host interrompa correttamente la ricezione UDP.

Obiettivi:

* `CancellationToken` propagato correttamente;
* `ReceiveAsync()` non deve rimanere bloccato durante lo shutdown;
* il `BackgroundService` deve terminare correttamente;
* evitare socket o task lasciati attivi;
* verificare il comportamento di `StopAsync()`.

---
2
## 4. Uniformare `TelemetryData`

Rendere coerenti i dati prodotti dalla sorgente reale e dalla fake.

### In particolare

Verificare:

* ERS;
* batteria;
* energia deployata;
* energia recuperata;
* carburante;
* temperature;
* pressioni;
* percentuali.

### Obiettivo

La stessa proprietà di `TelemetryData` deve avere sempre lo stesso significato e la stessa unità di misura, indipendentemente dalla sorgente.

La `FakeTelemetrySource` deve simulare la stessa semantica della sorgente reale.

---3
## 5. Rendere robuste le cache

Analizzare la gestione delle cache di:

* `CarStatusData`
* `LapData`

Attualmente:

```text
CarTelemetry
      +
CarStatus cache
      +
LapData cache
      ↓
TelemetryData
```

Verificare i casi in cui:

* `CarTelemetry` arriva prima di `CarStatus`;
* `CarTelemetry` arriva prima di `LapData`;
* un pacchetto UDP viene perso;
* cambia `PlayerCarIndex`;
* inizia una nuova sessione;
* la cache contiene ancora dati della sessione precedente.

### Obiettivo

La sorgente deve continuare a produrre snapshot coerenti anche in presenza di pacchetti mancanti o ricevuti in ordine diverso.

---
4
## 6. Sistemare le limitazioni note

Rivedere progressivamente i problemi già identificati.

### `FuelConsumption`

Attualmente disponibile nella fake ma non valorizzato dalla sorgente reale.

Decidere se:

* rimuoverlo;
* calcolarlo;
* recuperarlo da un altro pacchetto;
* lasciarlo temporaneamente non disponibile.

### `OnTelemetryReceived`

Attualmente l'handler è:

```csharp
async void
```

Verificare se sia realmente necessario utilizzare un event asincrono oppure se l'evento possa essere semplificato.

### 22 vetture

Attualmente vengono scartati i dati delle altre vetture.

Per ora va bene, ma valutare in futuro se il modello debba supportare:

* solo il player;
* tutte le vetture;
* entrambe le modalità.

### Snapshot

Verificare esplicitamente quali proprietà di `TelemetryData` rappresentano:

* dati aggiornati ad ogni `CarTelemetry`;
* dati aggiornati solo quando arriva un determinato packet type;
* dati provenienti da cache.

---5
## 7. Aggiungere i test

La cartella `tests/` è attualmente vuota.

Partire dai test più importanti e deterministici.

### Priorità

1. `F1TelemetrySource.Map()`
2. mapping dei dati delle gomme
3. mapping dei freni
4. mapping del settore
5. mapping del tipo di superficie
6. mapping del cambio
7. fusione con `CarStatus`
8. fusione con `LapData`

Successivamente valutare test per:

* `TelemetryService`;
* `TelemetryProducer`;
* `TelemetryConsumer`.

Evitare inizialmente test WPF/UI se non sono necessari.

---
6
## 8. Aggiungere grafici e storico telemetria

Quando la pipeline sarà stabile, aggiungere la visualizzazione storica.

Possibili grafici:

* Speed
* RPM
* Throttle
* Brake
* Steering Angle
* ERS
* Engine Temperature

Possibile approccio:

```text
TelemetryData
      ↓
Storico ultimi N secondi
      ↓
Chart
      ↓
UI
```

Lo storico dovrà avere una dimensione limitata per evitare una crescita indefinita della memoria.

---

# Ordine consigliato

```text
[1] Gestione cancellazione UDP
             ↓
[2] Uniformare TelemetryData
             ↓
[3] Rendere robuste le cache
             ↓
[4] Sistemare le limitazioni note
             ↓
[5] Aggiungere test
             ↓
[6] Aggiungere grafici/storico
```

## Principio guida

Per ora **non aggiungere ulteriore complessità architetturale**.

Prima rendere solida la pipeline:

```text
F1
 ↓
UDP
 ↓
F1TelemetrySource
 ↓
TelemetryProducer
 ↓
Channel
 ↓
TelemetryConsumer
 ↓
MainViewModel
 ↓
WPF
```

e solo successivamente ampliare il progetto con nuove funzionalità.

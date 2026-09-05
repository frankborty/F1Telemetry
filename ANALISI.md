# Analisi F1Telemetry

## Valutazione generale

Il progetto ha una buona base architetturale per un MVP: separazione `App/Core/Infrastructure`, DI, `IAsyncEnumerable`, `Channel<T>` e sorgente fake.

La build attuale è pulita:

- 0 errori
- 0 warning
- Nessun test automatico presente

Il problema principale è che l'app è ancora più una demo funzionante che una dashboard completa e affidabile.

## Problemi prioritari

### 1. Correttezza dei dati UDP

In `F1TelemetrySource.cs`:

- `Timestamp` non viene valorizzato per i dati reali e resta `DateTimeOffset.MinValue`.
- `DriverName`, `TeamId` e `NationalityId` non vengono popolati.
- I dati `CarStatus` e `LapData` sono memorizzati senza associare sessione, `PlayerCarIndex` o timestamp.
- Un pacchetto `CarTelemetry` può essere combinato con dati vecchi o appartenenti a un'altra sessione.
- Non vengono gestiti esplicitamente pacchetti persi o ricevuti fuori ordine.
- `PlayerCarIndex` viene usato senza una validazione esplicita.
- Un pacchetto UDP malformato può terminare il producer.

Questa è la parte più importante da sistemare: una dashboard con dati sbagliati è peggiore di una dashboard con dati mancanti.

### 2. Unità di misura non definite

Le proprietà seguenti hanno un significato ambiguo:

```text
BatteryLevel
ErsDeployment
ErsRecovery
FuelRemaining
FuelConsumption
Throttle
Brake
SteeringAngle
```

Non è chiaro se i valori siano percentuali, valori normalizzati, energia in Joule/MegaJoule, litri, litri al giro o litri al secondo.

La fake source genera inoltre dati incompatibili con la sorgente reale:

- `Throttle` e `Brake` sono normalizzati tra `0` e `1`.
- L'ERS fake è tra `0` e `100`.
- L'ERS reale sembra provenire da valori energetici.
- `FuelConsumption` è presente nella fake ma non viene valorizzato dalla sorgente reale.

Serve definire un contratto chiaro per ogni proprietà, documentato nel modello o rappresentato con tipi dedicati.

### 3. UI incompleta rispetto al README

Il README dichiara gomme, freni, temperature, pressioni, ERS, batteria, carburante, giro, settore, tempi, rev lights, steering e clutch.

`TelemetryView.xaml` visualizza invece solo:

- speed
- RPM
- gear
- throttle
- brake
- DRS

Mancano quasi tutti i dati già raccolti dal backend. Inoltre:

- non ci sono unità di misura;
- throttle e brake vengono mostrati come numeri decimali;
- non ci sono formattazioni per tempi e percentuali;
- non c'è indicazione dello stato della connessione UDP;
- non c'è indicazione di dati vecchi o mancanti;
- non c'è uno stato "in attesa di telemetria";
- non vengono mostrati errori all'utente.

Il primo miglioramento di prodotto dovrebbe essere completare una singola dashboard realmente utile.

### 4. Assenza di test

La cartella `tests/` è vuota.

Priorità dei test:

1. Mapping di `CarTelemetry`.
2. Mapping gomme.
3. Mapping freni.
4. Conversione settore.
5. Conversione superfici.
6. Mapping cambio e RPM.
7. Fusione con `CarStatus`.
8. Fusione con `LapData`.
9. Gestione cache.
10. Cancellazione durante `ReceiveAsync`.
11. Comportamento del canale pieno.
12. Aggiornamento e filtro del `TelemetryViewModel`.

## Lifecycle e concorrenza

### Gestione degli errori di avvio

`OnStartup` e `OnExit` sono metodi `async void`, necessari per gli eventi WPF, ma non esiste una gestione adeguata degli errori.

Un errore durante `_host.StartAsync()` può diventare un'eccezione non gestita e impedire l'apertura della finestra. Servono logging e gestione esplicita degli errori di avvio e shutdown.

### Eventi sincroni nel consumer

`TelemetryConsumer` invoca direttamente tutti gli handler:

```csharp
TelemetryReceived?.Invoke(telemetryData);
```

Con molte dashboard:

- ogni dashboard riceve ogni snapshot;
- il consumer dipende dalla velocità degli handler;
- un'eccezione può terminare il consumer;
- il numero di aggiornamenti cresce rapidamente.

È preferibile usare uno snapshot condiviso oppure filtrare i dati prima di arrivare ai singoli ViewModel.

### Accesso cross-thread

`TelemetryViewModel.UpdateTelemetry()` viene chiamato dal thread del consumer, mentre `DispatcherTimer` legge `_latestTelemetry` dal thread UI.

È meglio usare una delle seguenti soluzioni:

- `Dispatcher.BeginInvoke`;
- un campo sincronizzato;
- una coda per dashboard;
- un servizio thread-safe per l'ultimo snapshot.

### Un timer per ogni dashboard

Ogni `TelemetryViewModel` crea un `DispatcherTimer` da 16 ms. Con molte dashboard si generano molti timer indipendenti.

Meglio usare un singolo timer globale o limitare l'aggiornamento UI a 20-30 FPS.

## Modello dati

`TelemetryData` è una classe grande composta quasi interamente da proprietà primitive.

Miglioramenti consigliati:

- usare `record` immutabili;
- introdurre tipi per gomme e freni;
- usare enum per settore, superficie, marcia e stato DRS;
- usare proprietà nullable quando il dato non è disponibile;
- distinguere dati aggiornati da dati provenienti da cache;
- aggiungere `SessionId`, `PacketTimestamp`, `CarIndex`, `IsStale`, `Source` e `DataQuality`.

Una struttura futura potrebbe essere:

```csharp
public sealed record TelemetrySnapshot(
    DateTimeOffset Timestamp,
    int CarIndex,
    TelemetryAvailability Availability,
    CarTelemetry Car,
    TyreTelemetry Tyres,
    BrakeTelemetry Brakes,
    RaceTelemetry Race,
    EnergyTelemetry Energy);
```

Non è necessario introdurla subito, ma il modello attuale crescerà rapidamente in modo difficile da mantenere.

## Gestione delle vetture

La sorgente reale produce dati per tutte le vetture, mentre la UI usa una lista statica di 19 piloti.

Problemi:

- la lista è hardcoded;
- può diventare obsoleta;
- non supporta correttamente 20 o 22 vetture;
- il nome del pilota del pacchetto `Participants` non viene utilizzato;
- il mapping dipende da una lista manuale separata.

Il pacchetto `Participants` dovrebbe diventare la fonte principale per nome pilota, team, numero gara, nazionalità e indice vettura. La lista statica può rimanere come fallback per la fake source.

## Fake source

La fake source è utile, ma genera valori completamente casuali:

- velocità e RPM non coerenti;
- marcia casuale;
- throttle e brake contemporaneamente casuali;
- giro e settore non progressivi;
- carburante che non diminuisce;
- temperature e pressioni indipendenti dall'uso;
- DRS casuale;
- nessuna simulazione di perdita pacchetti o disconnessione.

Sarebbe più utile una fake source deterministica e realistica con:

- velocità influenzata da throttle e brake;
- RPM coerenti con la marcia;
- giro incrementale;
- settori progressivi;
- carburante decrescente;
- ERS che si scarica e si ricarica;
- eventi simulabili come packet loss, cambio pilota e disconnessione.

Inoltre, i file `FakeTelemetrySource .cs` e `TelemetryService .cs` hanno uno spazio finale nel nome e andrebbero rinominati.

## Configurazione e deploy

Miglioramenti utili:

- validare la porta UDP tra `1` e `65535`;
- gestire esplicitamente valori `Source` non validi;
- permettere la configurazione dalla UI;
- aggiungere profili `Fake`, `F1` e `Replay`;
- rendere configurabili logging e frequenza UI;
- aggiungere `appsettings.Development.json`;
- preparare una pubblicazione self-contained per Windows.

La configurazione dovrebbe essere rappresentata da una classe options invece di leggere stringhe direttamente in `App.xaml.cs`.

## Qualità del codice

Da migliorare:

- rimuovere `using` inutilizzati;
- rimuovere i riferimenti inutilizzati a `System.Diagnostics`;
- evitare `public` dove basta `internal`;
- uniformare stile e formattazione;
- aggiungere logging con `ILogger`;
- migliorare nomi come `RemoveItself`;
- rendere `DriverInfo` `sealed` se non deve essere esteso;
- usare modelli immutabili in modo coerente.

## Funzionalità da implementare

### Priorità alta

1. Stato della connessione UDP: connesso, in attesa, disconnesso ed errore porta occupata.
2. Dashboard completa con speed, RPM, rev lights, marcia, throttle, brake, steering, DRS, gomme, freni, carburante, ERS, giro e settore.
3. Rilevamento nuova sessione e reset delle cache.
4. Gestione dati stantii e perdita pacchetti.
5. Test automatici del mapping e delle cache.
6. Logging diagnostico con pacchetti ricevuti, errori, snapshot persi e latenza.

### Priorità media

1. Storico degli ultimi 30-60 secondi.
2. Grafici di speed, RPM, throttle e brake.
3. Salvataggio sessione in CSV o JSON.
4. Replay di una sessione registrata.
5. Layout configurabili.
6. Modalità singolo pilota e tutti i piloti.
7. Personalizzazione colori e unità.
8. Modalità cockpit a schermo intero.
9. Hotkey per cambiare dashboard.

### Priorità futura

1. Delta rispetto al giro precedente e al best lap.
2. Confronto settori.
3. Consumo medio e carburante stimato al traguardo.
4. Pit window e strategia gomme.
5. Overlay trasparente sopra il gioco.
6. Esportazione live tramite WebSocket o HTTP.
7. Client remoto per telefono o tablet.
8. Supporto a più versioni dei giochi F1.

## Roadmap consigliata

### Fase 1: solidità

- correggere timestamp e unità;
- sistemare cache e sessioni;
- validare pacchetti e indici;
- aggiungere logging;
- scrivere i test del mapping;
- testare la cancellazione UDP.

### Fase 2: prodotto minimo completo

- completare la dashboard;
- aggiungere gli stati di connessione;
- migliorare la fake source;
- rendere dinamici i piloti;
- aggiungere formattazione e unità.

### Fase 3: analisi

- storico limitato;
- grafici;
- delta giro;
- consumo carburante;
- replay.

### Fase 4: distribuzione

- installer o pacchetto portable;
- configurazione guidata;
- gestione aggiornamenti;
- documentazione utente;
- CI con build e test automatici.

## Conclusione

La priorità assoluta è rendere affidabili mapping, cache, unità di misura e test. L'architettura attuale è sufficiente per la prossima fase: non aggiungerei altri layer finché la pipeline UDP non è verificata e osservabile.

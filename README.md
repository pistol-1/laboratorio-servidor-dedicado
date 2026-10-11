# Laboratorio: Servidor Dedicado

Comparación experimental entre sincronizar por posiciones (servicio de referencia) y sincronizar por eventos (servicio propio), con clientes en Unity que consultan por sondeo periódico.

## Estructura del repositorio

La raíz del repositorio es el proyecto de Unity.

- `Assets/Scripts/`: código del cliente, separado por carpetas (`Configuration`, `Models`, `Gameplay`, `Networking`, `Experiments`).
- `Assets/Scenes/LabScene.unity`: escena del laboratorio.
- `event-service/`: servicio de eventos (Flask + Docker).

## Servicios

Servicio de referencia (posiciones), repositorio `memin2522/DedicatedServer-Api`:

```
docker compose up --build
```

Queda en `http://localhost:5005` (Swagger en `/swagger-ui`).

Servicio de eventos:

```
cd event-service
docker compose up --build
```

Queda en `http://localhost:5006`.

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/games/{game_id}/events` | Registra un evento `{eventId, playerId, type, payload}`. Responde 201 con `seq` y `timestamp`, o 422 si faltan datos. Si el `eventId` ya existe responde 200 con el `seq` original. |
| GET | `/games/{game_id}/events?since={seq}` | Devuelve los eventos con `seq` mayor que `since` y el `lastSeq` de la partida. |

Para reiniciar el contador de eventos entre pruebas: `docker compose restart` dentro de `event-service`.

## Unity

1. Abrir el proyecto, abrir `Assets/Scenes/LabScene.unity` y agregarla a Build Profiles / Build Settings.
2. En Player Settings, Other Settings: `Allow downloads over HTTP` en `Always allowed`.
3. Configurar `gameId`, `localPlayerId`, `remotePlayerId` y las URLs en el objeto `ConnectionSettings`. La segunda instancia debe tener los jugadores invertidos.
4. Controles: `W A S D` mueve el cubo azul. En modo eventos: `Espacio` (ProjectileFired), `E` (PlayerHit), `Q` (ShieldRaised).

El panel de la esquina superior izquierda cambia el intervalo de sondeo (50, 200 y 1000 ms), el modo (posiciones o eventos) y ejecuta los experimentos. Los resultados se guardan como CSV en `Application.persistentDataPath/ExperimentResults`.

## Experimentos

Experimento A (modo posiciones, las dos instancias activas): pulsar `A: medir latencia (100 GET)` en ambas instancias para cada intervalo. El resultado muestra el promedio y las peticiones por segundo de esa instancia; las del sistema completo son la suma de ambas.

Experimento B (modo posiciones, sin mover el cubo):
1. Receptor: `B receptor: iniciar registro`.
2. Emisor: `B emisor: publicar 20 valores`.
3. Receptor, unos 6 segundos después: `B receptor: detener y guardar`.

Experimento C (modo eventos):
1. Receptor: `C receptor: iniciar registro`.
2. Emisor: `C emisor: publicar 20 eventos`.
3. Receptor, unos 4 segundos después: `C receptor: detener y guardar`.

Repetir B y C para cada intervalo. Antes de cada repetición de C conviene reiniciar el servicio de eventos.

#Videos
https://drive.google.com/file/d/1U3GiINeQn2J0xtiP8w_hQQdJrcbolOkE/view?usp=drive_link
https://drive.google.com/file/d/1naN47jvfjwNoVOPmZ24_kh8pH0X2ksQI/view?usp=drive_link

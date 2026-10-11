using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class EventPoller : RepeatingRequest
{
    public event Action<GameEvent> EventReceived;

    private int lastSeq;

    protected override IEnumerator SendRequest()
    {
        string url = $"{settings.EventsUrl()}?since={lastSeq}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ProcessBatch(request.downloadHandler.text);
            }
            else
            {
                Debug.LogWarning($"No se pudieron consultar los eventos: {request.error}");
            }
        }
    }

    private void ProcessBatch(string json)
    {
        GameEventBatch batch = ParseBatch(json);

        if (batch == null || batch.events == null)
        {
            return;
        }

        ResyncIfServerRestarted(batch.lastSeq);

        foreach (GameEvent gameEvent in batch.events)
        {
            lastSeq = gameEvent.seq;
            NotifyIfRemote(gameEvent);
        }
    }

    private void ResyncIfServerRestarted(int serverLastSeq)
    {
        if (serverLastSeq < lastSeq)
        {
            Debug.LogWarning("El servicio de eventos parece haberse reiniciado; se reinicia el contador local.");
            lastSeq = serverLastSeq;
        }
    }

    private void NotifyIfRemote(GameEvent gameEvent)
    {
        if (gameEvent.playerId != settings.localPlayerId)
        {
            EventReceived?.Invoke(gameEvent);
        }
    }

    private static GameEventBatch ParseBatch(string json)
    {
        try
        {
            return JsonUtility.FromJson<GameEventBatch>(json);
        }
        catch (ArgumentException exception)
        {
            Debug.LogWarning($"Respuesta de eventos con formato inválido: {exception.Message}");
            return null;
        }
    }
}

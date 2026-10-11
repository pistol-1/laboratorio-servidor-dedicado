using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class EventPublisher : MonoBehaviour
{
    public ConnectionSettings settings;

    public void Publish(string eventType, EventPayload payload)
    {
        if (string.IsNullOrEmpty(eventType) || !settings.IsValid())
        {
            Debug.LogWarning("No se publica el evento: tipo vacío o configuración inválida.");
            return;
        }

        GameEvent gameEvent = new GameEvent
        {
            eventId = Guid.NewGuid().ToString(),
            playerId = settings.localPlayerId,
            type = eventType,
            payload = payload
        };

        StartCoroutine(PostEvent(gameEvent));
    }

    private IEnumerator PostEvent(GameEvent gameEvent)
    {
        using (UnityWebRequest request = JsonRequests.CreatePost(settings.EventsUrl(), JsonUtility.ToJson(gameEvent)))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"No se pudo publicar el evento {gameEvent.type}: {request.error}");
            }
        }
    }
}

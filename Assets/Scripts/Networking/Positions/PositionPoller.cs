using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PositionPoller : RepeatingRequest
{
    public RemotePlayerView remotePlayer;

    public event Action<PlayerPosition> PositionReceived;

    private const long NotFoundStatusCode = 404;

    protected override IEnumerator SendRequest()
    {
        string url = settings.PositionUrl(settings.remotePlayerId);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                HandlePosition(request.downloadHandler.text);
            }
            else if (request.responseCode != NotFoundStatusCode)
            {
                Debug.LogWarning($"No se pudo consultar la posición remota: {request.error}");
            }
        }
    }

    private void HandlePosition(string json)
    {
        PlayerPosition position = ParsePosition(json);

        if (position == null)
        {
            return;
        }

        remotePlayer.MoveTo(position.ToVector3());
        PositionReceived?.Invoke(position);
    }

    private static PlayerPosition ParsePosition(string json)
    {
        try
        {
            return JsonUtility.FromJson<PlayerPosition>(json);
        }
        catch (ArgumentException exception)
        {
            Debug.LogWarning($"Respuesta de posición con formato inválido: {exception.Message}");
            return null;
        }
    }
}

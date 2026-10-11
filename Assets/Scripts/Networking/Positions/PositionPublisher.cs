using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PositionPublisher : RepeatingRequest
{
    public Transform localPlayer;

    public IEnumerator PublishPosition(PlayerPosition position)
    {
        string url = settings.PositionUrl(settings.localPlayerId);

        using (UnityWebRequest request = JsonRequests.CreatePost(url, JsonUtility.ToJson(position)))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"No se pudo publicar la posición: {request.error}");
            }
        }
    }

    protected override IEnumerator SendRequest()
    {
        yield return PublishPosition(PlayerPosition.From(localPlayer.position));
    }
}

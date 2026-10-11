using UnityEngine;

public class ConnectionSettings : MonoBehaviour
{
    public const float MillisecondsPerSecond = 1000f;

    [Header("Servicios")]
    public string positionServerUrl = "http://localhost:5005";
    public string eventServerUrl = "http://localhost:5006";

    [Header("Partida")]
    public string gameId = "game1";
    public string localPlayerId = "playerA";
    public string remotePlayerId = "playerB";

    [Header("Sondeo")]
    public int pollIntervalMilliseconds = 200;

    private const int MinimumIntervalMilliseconds = 10;

    public float PollIntervalSeconds
    {
        get { return pollIntervalMilliseconds / MillisecondsPerSecond; }
    }

    public string PositionUrl(string playerId)
    {
        return $"{positionServerUrl}/server/{gameId}/{playerId}";
    }

    public string EventsUrl()
    {
        return $"{eventServerUrl}/games/{gameId}/events";
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(positionServerUrl)
            && !string.IsNullOrWhiteSpace(eventServerUrl)
            && !string.IsNullOrWhiteSpace(gameId)
            && !string.IsNullOrWhiteSpace(localPlayerId)
            && !string.IsNullOrWhiteSpace(remotePlayerId)
            && pollIntervalMilliseconds >= MinimumIntervalMilliseconds;
    }
}

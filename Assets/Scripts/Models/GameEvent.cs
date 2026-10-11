using System;

[Serializable]
public class GameEvent
{
    public int seq;
    public string eventId;
    public string playerId;
    public string type;
    public EventPayload payload;
}

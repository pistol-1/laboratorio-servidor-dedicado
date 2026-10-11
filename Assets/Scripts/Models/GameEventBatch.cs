using System;

[Serializable]
public class GameEventBatch
{
    public GameEvent[] events;
    public int lastSeq;
}

using System;
using UnityEngine;

[Serializable]
public class EventPayload
{
    public float posX;
    public float posY;
    public float posZ;
    public int burstIndex;

    public static EventPayload At(Vector3 position, int burstIndex = 0)
    {
        return new EventPayload
        {
            posX = position.x,
            posY = position.y,
            posZ = position.z,
            burstIndex = burstIndex
        };
    }
}

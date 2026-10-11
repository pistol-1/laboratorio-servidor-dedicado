using System;
using UnityEngine;

[Serializable]
public class PlayerPosition
{
    public float posX;
    public float posY;
    public float posZ;

    public static PlayerPosition From(Vector3 position)
    {
        return new PlayerPosition { posX = position.x, posY = position.y, posZ = position.z };
    }

    public Vector3 ToVector3()
    {
        return new Vector3(posX, posY, posZ);
    }
}

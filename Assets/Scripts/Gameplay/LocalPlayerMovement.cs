using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class LocalPlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        float horizontal = ReadAxis(keyboard.dKey, keyboard.aKey);
        float vertical = ReadAxis(keyboard.wKey, keyboard.sKey);
        transform.position += new Vector3(horizontal, 0f, vertical) * speed * Time.deltaTime;
    }

    private static float ReadAxis(KeyControl positiveKey, KeyControl negativeKey)
    {
        float axis = 0f;

        if (positiveKey.isPressed)
        {
            axis += 1f;
        }

        if (negativeKey.isPressed)
        {
            axis -= 1f;
        }

        return axis;
    }
}

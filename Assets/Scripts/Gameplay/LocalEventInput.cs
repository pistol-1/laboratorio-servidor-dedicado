using UnityEngine;
using UnityEngine.InputSystem;

public class LocalEventInput : MonoBehaviour
{
    public EventPublisher eventPublisher;
    public Transform localPlayer;

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            PublishAtPlayerPosition(ArenaEvents.ProjectileFired);
        }

        if (keyboard.eKey.wasPressedThisFrame)
        {
            PublishAtPlayerPosition(ArenaEvents.PlayerHit);
        }

        if (keyboard.qKey.wasPressedThisFrame)
        {
            PublishAtPlayerPosition(ArenaEvents.ShieldRaised);
        }
    }

    private void PublishAtPlayerPosition(string eventType)
    {
        eventPublisher.Publish(eventType, EventPayload.At(localPlayer.position));
    }
}

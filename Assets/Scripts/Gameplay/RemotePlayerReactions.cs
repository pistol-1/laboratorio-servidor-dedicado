using System.Collections;
using UnityEngine;

public class RemotePlayerReactions : MonoBehaviour
{
    public EventPoller eventPoller;
    public PlayerColor remoteColor;
    public float reactionSeconds = 0.5f;
    public float projectileScale = 0.4f;

    private void OnEnable()
    {
        eventPoller.EventReceived += React;
    }

    private void OnDisable()
    {
        eventPoller.EventReceived -= React;
    }

    private void React(GameEvent gameEvent)
    {
        switch (gameEvent.type)
        {
            case ArenaEvents.ProjectileFired:
                SpawnProjectile(gameEvent.payload);
                break;
            case ArenaEvents.PlayerHit:
                StartCoroutine(FlashColor(Color.red));
                break;
            case ArenaEvents.ShieldRaised:
                StartCoroutine(FlashColor(Color.cyan));
                break;
            default:
                Debug.LogWarning($"Tipo de evento desconocido: {gameEvent.type}");
                break;
        }
    }

    private void SpawnProjectile(EventPayload payload)
    {
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.transform.position = new Vector3(payload.posX, payload.posY, payload.posZ);
        projectile.transform.localScale = Vector3.one * projectileScale;
        Destroy(projectile, reactionSeconds);
    }

    private IEnumerator FlashColor(Color flashColor)
    {
        remoteColor.Show(flashColor);
        yield return new WaitForSeconds(reactionSeconds);
        remoteColor.Restore();
    }
}

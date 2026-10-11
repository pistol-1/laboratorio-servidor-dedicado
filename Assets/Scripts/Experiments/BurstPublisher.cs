using System;
using System.Collections;
using UnityEngine;

public class BurstPublisher : MonoBehaviour
{
    public PositionPublisher positionPublisher;
    public EventPublisher eventPublisher;
    public Transform localPlayer;

    private const float BurstIntervalSeconds = 0.02f;
    private const float MarkerSettleSeconds = 1.5f;
    private const float BurstSettleSeconds = 2f;

    public void StartPositionBurst()
    {
        StartCoroutine(PublishPositionBurst());
    }

    public void StartEventBurst()
    {
        StartCoroutine(PublishEventBurst());
    }

    private IEnumerator PublishPositionBurst()
    {
        positionPublisher.enabled = false;

        yield return positionPublisher.PublishPosition(PositionWithX(ExperimentConstants.ResetMarkerValue));
        yield return new WaitForSecondsRealtime(MarkerSettleSeconds);
        yield return RunBurst(burstIndex => StartCoroutine(positionPublisher.PublishPosition(PositionWithX(burstIndex))));
        yield return new WaitForSecondsRealtime(BurstSettleSeconds);

        positionPublisher.enabled = true;
    }

    private IEnumerator PublishEventBurst()
    {
        yield return RunBurst(burstIndex => eventPublisher.Publish(
            ArenaEvents.ProjectileFired,
            EventPayload.At(localPlayer.position, burstIndex)));
    }

    private IEnumerator RunBurst(Action<int> sendItem)
    {
        float nextSendTime = Time.realtimeSinceStartup;

        for (int burstIndex = 1; burstIndex <= ExperimentConstants.BurstSize; burstIndex++)
        {
            while (Time.realtimeSinceStartup < nextSendTime)
            {
                yield return null;
            }

            sendItem(burstIndex);
            nextSendTime += BurstIntervalSeconds;
        }
    }

    private static PlayerPosition PositionWithX(float posX)
    {
        return PlayerPosition.From(new Vector3(posX, 0f, 0f));
    }
}

using System;
using System.Collections;
using UnityEngine;

public abstract class RepeatingRequest : MonoBehaviour
{
    public ConnectionSettings settings;

    public event Action<float> RoundTripMeasured;

    protected abstract IEnumerator SendRequest();

    private void OnEnable()
    {
        if (settings == null || !settings.IsValid())
        {
            Debug.LogError($"{name}: configuración de conexión inválida, no se inician las peticiones.");
            return;
        }

        StartCoroutine(RunLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator RunLoop()
    {
        while (true)
        {
            float startTime = Time.realtimeSinceStartup;

            yield return SendRequest();

            float elapsedSeconds = Time.realtimeSinceStartup - startTime;
            RoundTripMeasured?.Invoke(elapsedSeconds * ConnectionSettings.MillisecondsPerSecond);

            float remainingSeconds = settings.PollIntervalSeconds - elapsedSeconds;
            yield return remainingSeconds > 0f ? new WaitForSecondsRealtime(remainingSeconds) : null;
        }
    }
}

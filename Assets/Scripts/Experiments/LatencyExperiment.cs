using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class LatencyExperiment : MonoBehaviour
{
    public PositionPublisher publisher;
    public PositionPoller poller;
    public ConnectionSettings settings;

    private const int SampleCount = 100;

    private readonly List<float> roundTripsMilliseconds = new List<float>();
    private int completedRequests;
    private float startTime;
    private bool isMeasuring;

    public string LastResult { get; private set; } = string.Empty;

    private void OnEnable()
    {
        publisher.RoundTripMeasured += CountRequest;
        poller.RoundTripMeasured += RecordGetRoundTrip;
    }

    private void OnDisable()
    {
        publisher.RoundTripMeasured -= CountRequest;
        poller.RoundTripMeasured -= RecordGetRoundTrip;
    }

    public void StartMeasuring()
    {
        roundTripsMilliseconds.Clear();
        completedRequests = 0;
        startTime = Time.realtimeSinceStartup;
        isMeasuring = true;
        LastResult = "Medición A en curso...";
    }

    private void CountRequest(float roundTripMilliseconds)
    {
        if (isMeasuring)
        {
            completedRequests++;
        }
    }

    private void RecordGetRoundTrip(float roundTripMilliseconds)
    {
        if (!isMeasuring)
        {
            return;
        }

        CountRequest(roundTripMilliseconds);
        roundTripsMilliseconds.Add(roundTripMilliseconds);

        if (roundTripsMilliseconds.Count >= SampleCount)
        {
            FinishMeasuring();
        }
    }

    private void FinishMeasuring()
    {
        isMeasuring = false;

        float elapsedSeconds = Time.realtimeSinceStartup - startTime;
        float averageMilliseconds = roundTripsMilliseconds.Average();
        float requestsPerSecond = completedRequests / elapsedSeconds;

        string filePath = ExperimentResultsFile.Save(
            "experiment_a",
            settings.pollIntervalMilliseconds,
            BuildCsv(averageMilliseconds, requestsPerSecond));

        LastResult = string.Format(
            CultureInfo.InvariantCulture,
            "A (intervalo {0} ms): promedio {1:F2} ms, {2:F1} peticiones/s en esta instancia. Archivo: {3}",
            settings.pollIntervalMilliseconds,
            averageMilliseconds,
            requestsPerSecond,
            filePath);
    }

    private string BuildCsv(float averageMilliseconds, float requestsPerSecond)
    {
        StringBuilder csv = new StringBuilder();
        csv.AppendLine("intervalMs,samples,averageRoundTripMs,requestsPerSecond");
        csv.AppendLine(string.Format(
            CultureInfo.InvariantCulture,
            "{0},{1},{2:F2},{3:F2}",
            settings.pollIntervalMilliseconds,
            roundTripsMilliseconds.Count,
            averageMilliseconds,
            requestsPerSecond));
        csv.AppendLine("sample,roundTripMs");

        for (int index = 0; index < roundTripsMilliseconds.Count; index++)
        {
            csv.AppendLine(string.Format(
                CultureInfo.InvariantCulture,
                "{0},{1:F2}",
                index + 1,
                roundTripsMilliseconds[index]));
        }

        return csv.ToString();
    }
}

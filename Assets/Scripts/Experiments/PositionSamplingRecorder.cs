using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PositionSamplingRecorder : MonoBehaviour
{
    public PositionPoller poller;
    public ConnectionSettings settings;

    private readonly HashSet<float> observedValues = new HashSet<float>();
    private readonly List<float> observedOrder = new List<float>();
    private bool isRecording;
    private bool isArmed;

    public string LastResult { get; private set; } = string.Empty;

    private void OnEnable()
    {
        poller.PositionReceived += RecordPosition;
    }

    private void OnDisable()
    {
        poller.PositionReceived -= RecordPosition;
    }

    public void StartRecording()
    {
        observedValues.Clear();
        observedOrder.Clear();
        isArmed = false;
        isRecording = true;
        LastResult = "Registro B en curso...";
    }

    public void StopRecording()
    {
        if (!isRecording)
        {
            return;
        }

        isRecording = false;

        string filePath = ExperimentResultsFile.Save("experiment_b", settings.pollIntervalMilliseconds, BuildCsv());
        LastResult = $"B (intervalo {settings.pollIntervalMilliseconds} ms): {observedValues.Count} de {ExperimentConstants.BurstSize} valores distintos. Archivo: {filePath}";
    }

    private void RecordPosition(PlayerPosition position)
    {
        if (!isRecording)
        {
            return;
        }

        if (!isArmed)
        {
            isArmed = Mathf.Approximately(position.posX, ExperimentConstants.ResetMarkerValue);
            return;
        }

        bool isBurstValue = position.posX >= 1f && position.posX <= ExperimentConstants.BurstSize;

        if (isBurstValue && observedValues.Add(position.posX))
        {
            observedOrder.Add(position.posX);
        }
    }

    private string BuildCsv()
    {
        StringBuilder csv = new StringBuilder();
        csv.AppendLine("intervalMs,distinctValues,expectedValues");
        csv.AppendLine($"{settings.pollIntervalMilliseconds},{observedValues.Count},{ExperimentConstants.BurstSize}");
        csv.AppendLine("observedValuesInArrivalOrder");
        csv.AppendLine(string.Join(";", observedOrder));
        return csv.ToString();
    }
}

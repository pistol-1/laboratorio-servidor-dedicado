using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EventPropagationRecorder : MonoBehaviour
{
    public EventPoller poller;
    public ConnectionSettings settings;

    private readonly List<GameEvent> receivedEvents = new List<GameEvent>();
    private bool isRecording;

    public string LastResult { get; private set; } = string.Empty;

    private void OnEnable()
    {
        poller.EventReceived += RecordEvent;
    }

    private void OnDisable()
    {
        poller.EventReceived -= RecordEvent;
    }

    public void StartRecording()
    {
        receivedEvents.Clear();
        isRecording = true;
        LastResult = "Registro C en curso...";
    }

    public void StopRecording()
    {
        if (!isRecording)
        {
            return;
        }

        isRecording = false;

        int duplicates = CountDuplicates();
        bool inOrder = AreInOrder();
        string filePath = ExperimentResultsFile.Save("experiment_c", settings.pollIntervalMilliseconds, BuildCsv(inOrder, duplicates));

        LastResult = $"C (intervalo {settings.pollIntervalMilliseconds} ms): {receivedEvents.Count} de {ExperimentConstants.BurstSize} eventos, en orden: {(inOrder ? "sí" : "no")}, repetidos: {duplicates}. Archivo: {filePath}";
    }

    private void RecordEvent(GameEvent gameEvent)
    {
        if (isRecording)
        {
            receivedEvents.Add(gameEvent);
        }
    }

    private int CountDuplicates()
    {
        HashSet<int> seenSequences = new HashSet<int>();
        int duplicates = 0;

        foreach (GameEvent gameEvent in receivedEvents)
        {
            if (!seenSequences.Add(gameEvent.seq))
            {
                duplicates++;
            }
        }

        return duplicates;
    }

    private bool AreInOrder()
    {
        for (int index = 1; index < receivedEvents.Count; index++)
        {
            GameEvent previous = receivedEvents[index - 1];
            GameEvent current = receivedEvents[index];

            if (current.seq < previous.seq || current.payload.burstIndex < previous.payload.burstIndex)
            {
                return false;
            }
        }

        return true;
    }

    private string BuildCsv(bool inOrder, int duplicates)
    {
        StringBuilder csv = new StringBuilder();
        csv.AppendLine("intervalMs,receivedEvents,expectedEvents,inOrder,duplicates");
        csv.AppendLine($"{settings.pollIntervalMilliseconds},{receivedEvents.Count},{ExperimentConstants.BurstSize},{inOrder},{duplicates}");
        csv.AppendLine("seq,burstIndex");

        foreach (GameEvent gameEvent in receivedEvents)
        {
            csv.AppendLine($"{gameEvent.seq},{gameEvent.payload.burstIndex}");
        }

        return csv.ToString();
    }
}

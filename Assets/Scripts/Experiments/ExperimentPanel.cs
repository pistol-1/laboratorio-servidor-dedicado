using UnityEngine;

public class ExperimentPanel : MonoBehaviour
{
    public ConnectionSettings settings;
    public GameObject positionSync;
    public GameObject eventSync;
    public LatencyExperiment latencyExperiment;
    public PositionSamplingRecorder positionSamplingRecorder;
    public EventPropagationRecorder eventPropagationRecorder;
    public BurstPublisher burstPublisher;

    private const int FrameRateLimit = 240;
    private const float PanelWidth = 400f;
    private const float PanelMargin = 10f;

    private static readonly int[] IntervalOptionsMilliseconds = { 50, 200, 1000 };

    private void Awake()
    {
        ConfigureRuntimeForMeasurements();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(PanelMargin, PanelMargin, PanelWidth, Screen.height - 2f * PanelMargin));

        DrawHeader();
        DrawIntervalButtons();
        DrawModeButtons();
        DrawExperimentButtons();
        DrawResults();

        GUILayout.EndArea();
    }

    private static void ConfigureRuntimeForMeasurements()
    {
        Application.runInBackground = true;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = FrameRateLimit;
    }

    private void DrawHeader()
    {
        GUILayout.Label($"Jugador local: {settings.localPlayerId} | remoto: {settings.remotePlayerId}");
        GUILayout.Label($"Intervalo de sondeo: {settings.pollIntervalMilliseconds} ms");
    }

    private void DrawIntervalButtons()
    {
        GUILayout.BeginHorizontal();

        foreach (int intervalMilliseconds in IntervalOptionsMilliseconds)
        {
            if (GUILayout.Button($"{intervalMilliseconds} ms"))
            {
                settings.pollIntervalMilliseconds = intervalMilliseconds;
            }
        }

        GUILayout.EndHorizontal();
    }

    private void DrawModeButtons()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Parte 1: posiciones"))
        {
            SetSyncMode(true);
        }

        if (GUILayout.Button("Parte 2: eventos"))
        {
            SetSyncMode(false);
        }

        GUILayout.EndHorizontal();
    }

    private void SetSyncMode(bool usePositions)
    {
        positionSync.SetActive(usePositions);
        eventSync.SetActive(!usePositions);
    }

    private void DrawExperimentButtons()
    {
        if (positionSync.activeSelf)
        {
            DrawPositionExperiments();
        }
        else
        {
            DrawEventExperiments();
        }
    }

    private void DrawPositionExperiments()
    {
        if (GUILayout.Button("A: medir latencia (100 GET)"))
        {
            latencyExperiment.StartMeasuring();
        }

        if (GUILayout.Button("B receptor: iniciar registro"))
        {
            positionSamplingRecorder.StartRecording();
        }

        if (GUILayout.Button("B emisor: publicar 20 valores"))
        {
            burstPublisher.StartPositionBurst();
        }

        if (GUILayout.Button("B receptor: detener y guardar"))
        {
            positionSamplingRecorder.StopRecording();
        }
    }

    private void DrawEventExperiments()
    {
        if (GUILayout.Button("C receptor: iniciar registro"))
        {
            eventPropagationRecorder.StartRecording();
        }

        if (GUILayout.Button("C emisor: publicar 20 eventos"))
        {
            burstPublisher.StartEventBurst();
        }

        if (GUILayout.Button("C receptor: detener y guardar"))
        {
            eventPropagationRecorder.StopRecording();
        }
    }

    private void DrawResults()
    {
        DrawResult(latencyExperiment.LastResult);
        DrawResult(positionSamplingRecorder.LastResult);
        DrawResult(eventPropagationRecorder.LastResult);
    }

    private static void DrawResult(string result)
    {
        if (!string.IsNullOrEmpty(result))
        {
            GUILayout.Label(result);
        }
    }
}

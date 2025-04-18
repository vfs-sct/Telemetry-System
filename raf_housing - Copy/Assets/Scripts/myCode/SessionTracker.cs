using System.Collections.Generic;
using UnityEngine;

public class SessionTracker : MonoBehaviour
{
    private float sessionStartTime = 0f;

    void Start()
    {
        sessionStartTime = Time.time;

        TelemetryManager.Instance.LogEvent("session_start", new Dictionary<string, object>
        {
            { "startTime", System.DateTime.UtcNow.ToString("o")},
            { "token", SessionManager.Instance.AuthToken },
            { "name", SessionManager.Instance._name },
        });
    }

    private void OnApplicationQuit()
    {
        float sessionDuration = Time.time - sessionStartTime;
        TelemetryManager.Instance.LogEvent("session_end", new Dictionary<string, object>
        {
            {"durationSec", sessionDuration },
            { "endTime", System.DateTime.UtcNow.ToString("o")}
        });
    }
}

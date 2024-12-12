using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Signal
{
    public string name; // Name of the signal
    [Range(0, 23)] public int hour; // Hour of the signal (0-23)
    [Range(0, 59)] public int minute; // Minute of the signal (0-59)
}

public class DayCycleClock : MonoBehaviour
{
    [Header("Clock Settings")]
    [Range(0, 24)]
    public float currentTime; // Current time in hours
    public float secondsPerDay = 60f; // Duration of one full day in seconds

    [Header("Signals")]
    public List<Signal> signals = new List<Signal>(); // List of signals with time and name

    public event System.Action<string> OnSignalTriggered; // Event triggered with the signal name

    private HashSet<string> triggeredSignals = new HashSet<string>(); // Track triggered signals
    private const float TimeTolerance = 1f / 60f; // Time window tolerance (1 second)

    void Update()
    {
        // Recalculate timeIncrement to respect changes in secondsPerDay
        float timeIncrement = 24f / secondsPerDay;

        // Increment the clock
        currentTime += timeIncrement * Time.deltaTime;

        // Wrap around at 24 hours
        if (currentTime >= 24f)
        {
            currentTime = 0f;
            triggeredSignals.Clear(); // Reset for the new day
        }

        // Check and trigger signals
        foreach (var signal in signals)
        {
            float signalTime = signal.hour + (signal.minute / 60f); // Convert hour and minute to time in hours
            if (!triggeredSignals.Contains(signal.name) && Mathf.Abs(currentTime - signalTime) <= TimeTolerance)
            {
                triggeredSignals.Add(signal.name);
                OnSignalTriggered?.Invoke(signal.name); // Trigger the signal by name
                Debug.Log($"Signal Triggered: {signal.name} at {signal.hour:D2}:{signal.minute:D2}");
            }
        }
    }

    public string GetFormattedTime()
    {
        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt((currentTime - hours) * 60f);
        string period = hours >= 12 ? "PM" : "AM";
        int displayHours = hours % 12;
        if (displayHours == 0) displayHours = 12;

        return $"{displayHours:D2}:{minutes:D2} {period}";
    }
}

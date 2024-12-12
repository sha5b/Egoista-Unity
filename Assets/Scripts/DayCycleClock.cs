using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class DayCycleClock : MonoBehaviour
{
    [Header("Clock Settings")]
    [Range(0, 24)]
    public float currentTime; // Represents the current hour in the 24-hour format
    public float secondsPerDay = 60f; // Total duration of a day in real seconds

    [Header("Signal Times")]
    public List<float> signalTimes = new List<float>(); // List of times to emit signals

    public event System.Action<float> OnTimeSignal; // Event broadcasted with the time of the signal

    private float timeIncrement;
    private HashSet<float> triggeredSignals = new HashSet<float>(); // Track already triggered signals for this day

    void Start()
    {
        // Calculate time increment per frame
        timeIncrement = 24f / (secondsPerDay / Time.deltaTime);
    }

    void Update()
    {
        // Increment the clock
        currentTime += timeIncrement * Time.deltaTime;

        // Wrap around at 24 hours
        if (currentTime >= 24f)
        {
            currentTime = 0f;
            triggeredSignals.Clear(); // Reset signals for the new day
        }

        // Print current time in AM/PM format
        PrintCurrentTime();

        // Check signal times
        foreach (float signalTime in signalTimes)
        {
            if (!triggeredSignals.Contains(signalTime) && Mathf.Approximately(currentTime, signalTime))
            {
                triggeredSignals.Add(signalTime);
                OnTimeSignal?.Invoke(signalTime); // Emit the signal with the time
            }
        }
    }

    private void PrintCurrentTime()
    {
        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt((currentTime - hours) * 60f);
        string period = hours >= 12 ? "PM" : "AM";

        int displayHours = hours % 12;
        if (displayHours == 0) displayHours = 12; // Handle midnight and noon

        Debug.Log($"Current Time: {displayHours:D2}:{minutes:D2} {period}");
    }
}

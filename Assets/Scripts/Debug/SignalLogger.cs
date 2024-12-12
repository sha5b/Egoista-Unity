using UnityEngine;

public class SignalLogger : MonoBehaviour
{
    public DayCycleClock clock; // Reference to the DayCycleClock

    private void OnEnable()
    {
        if (clock != null)
        {
            clock.OnSignalTriggered += LogSignal;
            Debug.Log("SignalLogger: Subscribed to DayCycleClock.");
        }
        else
        {
            Debug.LogError("SignalLogger: Clock reference is missing!");
        }
    }

    private void OnDisable()
    {
        if (clock != null)
        {
            clock.OnSignalTriggered -= LogSignal;
            Debug.Log("SignalLogger: Unsubscribed from DayCycleClock.");
        }
    }

    private void LogSignal(string signalName)
    {
        string formattedTime = clock?.GetFormattedTime() ?? "Unknown Time";
        Debug.Log($"SignalLogger: Signal '{signalName}' emitted at {formattedTime}");
    }
}

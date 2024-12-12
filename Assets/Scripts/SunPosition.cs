using UnityEngine;

public class SunPosition : MonoBehaviour
{
    public DayCycleClock clock; // Reference to the DayCycleClock
    public Transform sun; // The directional light to control

    private void OnEnable()
    {
        if (clock != null)
        {
            clock.OnSignalTriggered += HandleSignal; // Subscribe to the OnSignalTriggered event
        }
    }

    private void OnDisable()
    {
        if (clock != null)
        {
            clock.OnSignalTriggered -= HandleSignal; // Unsubscribe from the event
        }
    }

    private void HandleSignal(string signalName)
    {
        Debug.Log($"Signal received in SunPosition: {signalName}");
        // Update the sun's rotation based on the current time
        UpdateSunRotation(clock.currentTime);
    }

    private void Update()
    {
        if (clock != null && sun != null)
        {
            // Smoothly update the sun's rotation based on the current time
            UpdateSunRotation(clock.currentTime);
        }
    }

    private void UpdateSunRotation(float time)
    {
        // Map the time of day to a rotation angle
        float angle = (time / 24f) * 360f;

        // Rotate the sun (directional light)
        if (sun != null)
        {
            sun.localRotation = Quaternion.Euler(angle - 90f, 0f, 0f);
            // Subtract 90 degrees to align the sun correctly (adjust as needed for your scene).
        }
    }
}

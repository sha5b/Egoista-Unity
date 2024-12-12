using UnityEngine;

public class SunPosition : MonoBehaviour
{
    public DayCycleClock clock; // Reference to your DayCycleClock
    public Transform sun; // The directional light to control

    private void OnEnable()
    {
        if (clock != null)
        {
            clock.OnTimeSignal += UpdateSunRotation;
        }
    }

    private void OnDisable()
    {
        if (clock != null)
        {
            clock.OnTimeSignal -= UpdateSunRotation;
        }
    }

    private void UpdateSunRotation(float signalTime)
    {
        // Map the time of day to a rotation angle
        // 0 = Midnight, 6 = Sunrise, 12 = Noon, 18 = Sunset, 24 = Midnight
        float angle = (signalTime / 24f) * 360f;

        // Rotate the sun (directional light)
        if (sun != null)
        {
            sun.localRotation = Quaternion.Euler(angle - 90f, 0f, 0f);
            // Subtract 90 degrees to align the sun correctly (adjust as needed for your scene).
        }
    }

    private void Update()
    {
        if (clock != null && sun != null)
        {
            // Smoothly update the sun's rotation based on the current time
            float angle = (clock.currentTime / 24f) * 360f;
            sun.localRotation = Quaternion.Euler(angle - 90f, 0f, 0f);
        }
    }
}

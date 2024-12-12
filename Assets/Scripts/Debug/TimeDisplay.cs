using UnityEngine;
using UnityEngine.UI;
using TMPro; // Use this if you're using TextMeshPro

public class TimeDisplay : MonoBehaviour
{
    public DayCycleClock clock; // Reference to the DayCycleClock
    public TextMeshProUGUI timeText; // For TextMeshPro (preferred)
    // public Text timeText; // Uncomment if using the standard Text component

    void Update()
    {
        if (clock != null && timeText != null)
        {
            // Get the current formatted time from the clock
            string formattedTime = clock.GetFormattedTime();
            
            // Update the text component
            timeText.text = $"Current Time: {formattedTime}";
        }
    }
}

# Meta Quest 3 Passthrough and LiDAR Visualization

Quick setup guide for visualizing Meta Quest 3's passthrough and LiDAR scanning in Unity.

## Prerequisites
- Unity 2022.3 or newer
- Meta Quest 3 headset
- Meta Quest Link (for PC testing)
- USB-C cable for Quest Link connection

## Project Scripts

### PassthroughLidarManager.cs
```csharp
// Add this script to XR Origin to manage passthrough and scene understanding
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.OculusQuestSupport;
using UnityEngine.XR.Management;
using System.Collections.Generic;

public class PassthroughLidarManager : MonoBehaviour
{
    [Header("Passthrough Settings")]
    [SerializeField]
    private bool enablePassthrough = true;

    [Header("Scene Understanding Settings")]
    [SerializeField]
    private bool enableSceneUnderstanding = true;

    [SerializeField]
    private Material sceneVisualizationMaterial;

    // ... rest of the script implementation
}
```

### SunPosition.cs
This script handles sun positioning in the scene for lighting:
- Calculates sun position based on time of day
- Updates directional light rotation
- Works with DayCycleClock.cs for time management

### DayCycleClock.cs
Manages the day/night cycle:
- Controls time progression
- Syncs with SunPosition.cs
- Provides time-based events

## Unity Setup

1. **XR Configuration**
   - Go to Edit > Project Settings > XR Plug-in Management
   - Enable OpenXR
   - Under OpenXR, check "Meta Quest feature group"
   - In OpenXR tab:
     - Add "Oculus Touch Controller Profile"
     - Enable "Quest Support"
     - Enable "Passthrough Extension Feature"

2. **Scene Setup**
   - Add XR Origin (VR) to your scene
   - Add AR Default Point Cloud as child of XR Origin
   - Ensure hierarchy:
     ```
     XR Origin (VR)
     ├── Camera Offset
     │   └── Main Camera
     ├── Left Controller
     ├── Right Controller
     └── AR Default Point Cloud
     ```
   - Add Directional Light for SunPosition script (if using day/night cycle)

3. **Material Setup**
   - Create new material named "PointCloudMaterial"
   - Set shader to "Universal Render Pipeline/Lit"
   - Configure material:
     - Set Base Map color to cyan (0, 255, 255)
     - Set Metallic to 1
     - Set Smoothness to 0.8
     - Enable Alpha Clipping
     - Set Render Face to "Both"

4. **Component Setup**
   - Add PassthroughLidarManager script to XR Origin
   - Assign PointCloudMaterial to:
     - AR Default Point Cloud's Material field
     - PassthroughLidarManager's Scene Visualization Material field
   - If using day/night cycle:
     - Add SunPosition script to Directional Light
     - Add DayCycleClock to scene manager object

## Script Integration
The scripts work together to provide:
- Real-world environment visualization (PassthroughLidarManager)
- Dynamic lighting based on time (SunPosition + DayCycleClock)
- Debug visualization tools in the Scripts/Debug folder

## Testing with Quest Link
1. Connect Quest 3 to PC via USB
2. Enable Quest Link on headset
3. In Unity Editor, press Play
4. Move around to see:
   - Passthrough view of real environment
   - LiDAR point cloud visualization
   - Dynamic lighting if using day/night cycle

## Building to Device
1. Switch platform to Android in Build Settings
2. Set Minimum API Level to Android 10.0 or higher
3. Build and deploy to Quest 3

## Troubleshooting
- Ensure OpenXR is properly configured for both PC and Android platforms
- Check Quest Link connection if testing through PC
- Verify all materials and scripts are properly assigned
- Make sure Quest 3 has developer mode enabled for direct deployment
- Check Debug folder scripts for additional debugging tools:
  - SignalLogger.cs for logging
  - TimeDisplay.cs for time debugging

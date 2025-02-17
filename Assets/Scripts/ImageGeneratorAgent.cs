using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ImageGeneratorAgent : Agent
{
    [Header("Generation Settings")]
    public int imageSize = 256;
    public float noiseScale = 1.0f;
    public float updateInterval = 0.1f;
    
    [Header("Debug")]
    public bool debugMode = true;
    private float debugTime = 0f;
    private Color debugColor = Color.red;
    
    [Header("Prompt Settings")]
    public string prompt = "space nebula";
    [Range(0, 1)]
    public float redInfluence = 0.5f;
    [Range(0, 1)]
    public float greenInfluence = 0.3f;
    [Range(0, 1)]
    public float blueInfluence = 0.8f;

    [Header("Rendering")]
    public MeshRenderer planeRenderer;
    private Texture2D generatedTexture;
    private float[] noiseValues; 
    private float elapsedTime;

    public override void Initialize()
    {
        if (planeRenderer == null)
        {
            planeRenderer = GetComponent<MeshRenderer>();
            Debug.Log("Found MeshRenderer: " + (planeRenderer != null));
        }

        if (planeRenderer == null)
        {
            Debug.LogError("No MeshRenderer found! Please assign one in the inspector.");
            return;
        }

        // Initialize texture
        generatedTexture = new Texture2D(imageSize, imageSize);
        generatedTexture.filterMode = FilterMode.Bilinear;
        Debug.Log("Created texture with size: " + imageSize);
        
        // Create a new material instance to avoid modifying the shared material
        Material instanceMaterial = new Material(planeRenderer.sharedMaterial);
        planeRenderer.material = instanceMaterial;
        
        // Set texture to material
        planeRenderer.material.mainTexture = generatedTexture;
        Debug.Log("Assigned texture to material");
        
        // Initialize noise array
        noiseValues = new float[imageSize * imageSize];
        
        // Generate initial image
        GenerateNoiseImage();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add current parameters as observations
        sensor.AddObservation(noiseScale);
        sensor.AddObservation(imageSize);
        sensor.AddObservation(transform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Get continuous actions
        float scaleChange = actions.ContinuousActions[0];
        float positionChange = actions.ContinuousActions[1];
        
        // Apply changes
        noiseScale = Mathf.Clamp(noiseScale + scaleChange, 0.1f, 50f);
        transform.position += new Vector3(positionChange, 0, 0) * Time.deltaTime;
        
        // Update image based on interval
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= updateInterval)
        {
            GenerateNoiseImage();
            elapsedTime = 0f;
        }
        
        // Simple reward based on image complexity
        float reward = CalculateImageComplexity();
        AddReward(reward);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal") * 0.1f; // Scale change
        continuousActions[1] = Input.GetAxis("Vertical") * 0.1f;   // Position change
    }

    private void GenerateNoiseImage()
    {
        if (generatedTexture == null || planeRenderer == null)
        {
            Debug.LogError("Missing texture or renderer! Reinitializing...");
            Initialize();
            return;
        }

        float timeValue = Time.time * 0.1f;
        Debug.Log("Generating new image with noise scale: " + noiseScale);

        if (debugMode)
        {
            // In debug mode, create an obvious color-changing pattern
            debugTime += Time.deltaTime;
            if (debugTime > 1f)
            {
                debugTime = 0f;
                debugColor = debugColor == Color.red ? Color.blue : Color.red;
                Debug.Log("Debug: Color changed to " + debugColor);
            }

            // Fill texture with debug pattern
            for (int y = 0; y < imageSize; y++)
            {
                for (int x = 0; x < imageSize; x++)
                {
                    // Create a checkerboard pattern
                    bool isEvenX = (x / 32) % 2 == 0;
                    bool isEvenY = (y / 32) % 2 == 0;
                    Color color = (isEvenX == isEvenY) ? debugColor : Color.white;
                    generatedTexture.SetPixel(x, y, color);
                }
            }
            generatedTexture.Apply();
            return;
        }

        for (int y = 0; y < imageSize; y++)
        {
            for (int x = 0; x < imageSize; x++)
            {
                float xCoord = (float)x / imageSize * noiseScale + timeValue;
                float yCoord = (float)y / imageSize * noiseScale + timeValue;
                
                // Generate different noise patterns for each color channel
                float redNoise = Mathf.PerlinNoise(xCoord * redInfluence, yCoord * redInfluence);
                float greenNoise = Mathf.PerlinNoise(xCoord * greenInfluence + 100, yCoord * greenInfluence + 100);
                float blueNoise = Mathf.PerlinNoise(xCoord * blueInfluence + 200, yCoord * blueInfluence + 200);
                
                // Store the average noise value for complexity calculation
                noiseValues[y * imageSize + x] = (redNoise + greenNoise + blueNoise) / 3f;
                
                // Create a color based on the prompt's "theme"
                Color color = new Color(
                    redNoise * redInfluence,
                    greenNoise * greenInfluence,
                    blueNoise * blueInfluence,
                    1f
                );
                generatedTexture.SetPixel(x, y, color);
            }
        }

        generatedTexture.Apply();
    }

    private float CalculateImageComplexity()
    {
        float complexity = 0f;
        for (int i = 1; i < noiseValues.Length; i++)
        {
            complexity += Mathf.Abs(noiseValues[i] - noiseValues[i - 1]);
        }
        return complexity / noiseValues.Length;
    }

    // Automatically set color influences based on prompt
    private void UpdatePromptInfluences()
    {
        // Simple keyword-based influence setting
        prompt = prompt.ToLower();
        
        if (prompt.Contains("space") || prompt.Contains("nebula"))
        {
            blueInfluence = 0.8f;
            redInfluence = 0.5f;
            greenInfluence = 0.3f;
        }
        else if (prompt.Contains("fire") || prompt.Contains("sunset"))
        {
            redInfluence = 0.9f;
            greenInfluence = 0.3f;
            blueInfluence = 0.2f;
        }
        else if (prompt.Contains("forest") || prompt.Contains("nature"))
        {
            greenInfluence = 0.8f;
            redInfluence = 0.3f;
            blueInfluence = 0.4f;
        }
        else if (prompt.Contains("ocean") || prompt.Contains("water"))
        {
            blueInfluence = 0.9f;
            greenInfluence = 0.6f;
            redInfluence = 0.2f;
        }
        else
        {
            // Default balanced values
            redInfluence = 0.5f;
            greenInfluence = 0.5f;
            blueInfluence = 0.5f;
        }
    }

    private void OnValidate()
    {
        UpdatePromptInfluences();
    }

    void Update()
    {
        // Force update in debug mode
        if (debugMode)
        {
            GenerateNoiseImage();
        }
    }
}

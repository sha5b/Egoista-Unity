using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SandDuneTerrain : MonoBehaviour
{
    [Header("Clock Settings")]
    public DayCycleClock clock; // Reference to your existing clock

    [Header("Terrain Settings")]
    public int width = 10; // Number of quads along the width
    public int height = 10; // Number of quads along the height
    public float quadSize = 1f; // Size of each quad
    public float heightScale = 5f; // Maximum height of dunes
    public float detailScale = 5f; // Noise detail scale
    public float timeScale = 1f; // Speed of dune movement based on the clock's current time

    private MeshFilter meshFilter;
    private Vector3[] baseVertices; // Store original vertices of the plane

    void Start()
    {
        // Generate the terrain plane
        GeneratePlane();
    }

    void Update()
    {
        // Ensure the clock is assigned
        if (clock == null)
        {
            Debug.LogError("SandDuneTerrain: DayCycleClock is not assigned.");
            return;
        }

        // Use the clock's current time to deform the terrain
        float time = clock.currentTime * timeScale;
        DeformTerrain(time);
    }

    void GeneratePlane()
    {
        // Create a new mesh
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Sand Dune";

        // Define vertices
        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
        for (int z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                vertices[z * (width + 1) + x] = new Vector3(x * quadSize, 0, z * quadSize);
            }
        }

        // Define triangles
        int[] triangles = new int[width * height * 6];
        int triangleIndex = 0;
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int topLeft = z * (width + 1) + x;
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + (width + 1);
                int bottomRight = bottomLeft + 1;

                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = topRight;

                triangles[triangleIndex++] = topRight;
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = bottomRight;
            }
        }

        // Define UVs for texture mapping
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                uvs[z * (width + 1) + x] = new Vector2((float)x / width, (float)z / height);
            }
        }

        // Assign mesh data
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        // Assign the mesh to the MeshFilter
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // Store the base vertices
        baseVertices = mesh.vertices;
    }

    void DeformTerrain(float time)
    {
        // Ensure the mesh and base vertices are set
        if (meshFilter == null || baseVertices == null) return;

        // Create a copy of the base vertices
        Vector3[] vertices = new Vector3[baseVertices.Length];

        // Deform vertices using Perlin noise
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = baseVertices[i];
            float xCoord = vertex.x * detailScale + time;
            float zCoord = vertex.z * detailScale + time;
            vertex.y = Mathf.PerlinNoise(xCoord, zCoord) * heightScale;
            vertices[i] = vertex;
        }

        // Update the mesh with deformed vertices
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
    }
}

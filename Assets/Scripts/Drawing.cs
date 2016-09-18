using UnityEngine;
using System.Collections;

public class Drawing : MonoBehaviour
{
    private MeshFilter thisMeshFilter;
    private MeshCollider thisMeshCollider;

    private Vector2[] points;
    private int type;


	void Awake ()
    {
        thisMeshFilter = GetComponent<MeshFilter>();
        thisMeshCollider = GetComponent<MeshCollider>();
        points = new Vector2[0];
	}
	
	void Update ()
    {
       
	}

    public void draw ()
    {
        thisMeshFilter.mesh = createMesh();
        thisMeshCollider.sharedMesh = thisMeshFilter.mesh;

        // Hack to have all drawings facing the camera
        if (thisMeshFilter.mesh.normals[0].z > 0)
        {
            transform.Rotate(new Vector3(0f, 180f, 0f));
        }

        /*
        thisMeshFilter.mesh = createMesh();
        Mesh sourceMesh = new Mesh();
        sourceMesh = thisMeshFilter.mesh;
        Mesh workingMesh = CloneMesh(sourceMesh);
        thisMeshFilter.mesh = workingMesh;

        int iterations = 1;
        for (int i = 0; i < iterations; i++)
        {
            //workingMesh.vertices = SmoothFilter.laplacianFilter(workingMesh.vertices, workingMesh.triangles);
            workingMesh.vertices = SmoothFilter.hcFilter(thisMeshFilter.mesh.vertices, thisMeshFilter.mesh.vertices, thisMeshFilter.mesh.triangles, 0.0f, 0.5f);
        }
        thisMeshCollider.sharedMesh = thisMeshFilter.mesh;
        */
    }

    private static Mesh CloneMesh(Mesh mesh)
    {
        Mesh clone = new Mesh();
        clone.vertices = mesh.vertices;
        clone.normals = mesh.normals;
        clone.tangents = mesh.tangents;
        clone.triangles = mesh.triangles;
        clone.uv = mesh.uv;
        clone.uv2 = mesh.uv2;
        clone.bindposes = mesh.bindposes;
        clone.boneWeights = mesh.boneWeights;
        clone.bounds = mesh.bounds;
        clone.colors = mesh.colors;
        clone.name = mesh.name;
        //TODO : Are we missing anything?
        return clone;
    }

    public void addPoint(float x, float y)
    {
        Vector2[] newPoints = new Vector2[points.Length + 1];
        for (int i = 0; i < points.Length; i++)
        {
            newPoints[i] = points[i];
        }
        newPoints[newPoints.Length - 1] = new Vector2(x, y);
        points = newPoints;
    }

    Mesh createMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "drawing";

        Vector3[] vertices = new Vector3[points.Length];
        //int[] triangles = new int[(points.Length / 2) * 3];
        //Vector2[] uv = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vertices[i] = points[i];

            /*uv = new Vector2[] {
                new Vector2 (0, 0),
                new Vector2 (0, 1),
                new Vector2(1, 1),
                new Vector2 (1, 0)
            };*/
        }

        mesh.vertices = vertices;
        //mesh.uv = uv;
        Triangulator tri = new Triangulator(points);
        mesh.triangles = tri.Triangulate();
        mesh.RecalculateNormals();

        return mesh;
    }

    public Vector2[] getPoints()
    {
        return points;
    }
}

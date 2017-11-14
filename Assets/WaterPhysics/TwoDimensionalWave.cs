using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDimensionalWave : MonoBehaviour
{
    public float heightScale;
    public float pxScale;
    public float pzScale;
    public float speed;

    public GameObject colliderLink;

    MeshFilter meshFil;
    Vector3[] vertices;
    GameObject[] colliders;

    public int zVerts, xVerts;

    // Use this for initialization
    void Start()
    {
        meshFil = GetComponent<MeshFilter>();
        colliders = new GameObject[xVerts+1];
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i] = Instantiate(colliderLink, transform);
        }
        meshBuilder();
        vertices = meshFil.mesh.vertices;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < vertices.Length; i++)
        {
            float x = (vertices[i].x * pxScale) + (Time.time * speed);
            float z = -(vertices[i].z * pzScale) + (Time.time * speed); //Negative to make waves go away from camera
            vertices[i].y = Mathf.PerlinNoise(x, z) * heightScale;

            //Adjust colliders
            if (vertices[i].z == zVerts / 2)
            {
                colliders[i - (zVerts / 2 * (xVerts + 1))].transform.position = transform.TransformPoint(vertices[i]);
            }
        }

        meshFil.mesh.vertices = vertices;
        meshFil.mesh.RecalculateNormals();
        meshFil.mesh.RecalculateBounds();

        //TODO: http://catlikecoding.com/unity/tutorials/procedural-grid/
    }

    void meshBuilder()
    {
        Mesh fMesh = new Mesh();
        fMesh.name = "Generated Mesh";

        //Generate vertices
        Vector3[] vertices = new Vector3[(xVerts + 1) * (zVerts + 1)];
        for (int i = 0, z = 0; z <= zVerts; z++)
        {
            for (int x = 0; x <= xVerts; x++, i++)
            {
                vertices[i] = new Vector3(x, 0, z);
            }
        }
        fMesh.vertices = vertices;

        //Generate triangles
        int[] triangles = new int[xVerts * zVerts * 6];
        for (int ti = 0, vi = 0, y = 0; y < zVerts; y++, vi++)
        {
            for (int x = 0; x < xVerts; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xVerts + 1;
                triangles[ti + 5] = vi + xVerts + 2;
            }
        }
        fMesh.triangles = triangles;

        //Assign meshes
        meshFil.mesh = fMesh;
    }
}

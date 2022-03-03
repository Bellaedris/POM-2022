using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class MeshGenerator
{
    public static MeshData GenerateMesh(float[,] noisemap, float maxHeight, int width, int height, AnimationCurve flattening, int levelOfDetail) {
        int nx = noisemap.GetLength(1);
        int ny = noisemap.GetLength(0);

        int meshSimplificationIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;
       
        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        //calculate vertices
        for (int y = 0; y < ny; y += meshSimplificationIncrement)
        {
            //only write the point if he is not at the end of a line (as it would not have a point to its right)
            for (int x = 0; x < nx; x += meshSimplificationIncrement)
            {
                lock(flattening) {
                    meshData.vertices[vertexIndex] = new Vector3((float) x, flattening.Evaluate(noisemap[x, y]) * maxHeight, (float) y);
                }
                meshData.uvs[vertexIndex] = new Vector2(x / (float) width, y / (float) height);

                if (x < width - 1 && y > 0) {
                    meshData.AddTriangle(vertexIndex, vertexIndex - verticesPerLine + 1, vertexIndex -verticesPerLine);
                    meshData.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex - verticesPerLine + 1);
                }
                vertexIndex++;
            }
        }

        return meshData;
    }

    public static Mesh generateWater(int width, int height) {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        //calculate vertices
        for (int y = 0; y < height; y++)
        {
            //only write the point if he is not at the end of a line (as it would not have a point to its right)
            for (int x = 0; x < width; x++)
            {
                vertices.Add(new Vector3(x, 0, y));
            }
        }

        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x / width, vertices[i].z / height);
        }

        List<int> triangles = new List<int>();
        //skips the first line as it has to points above 
        for (int y = 1; y < height; y++)
        {
            //only write the point if he is not at the end of a line (as it would not have a point to its right)
            for (int x = 0; x < width - 1; x++)
            {
                int current = y * height + x; // the "number" of the current point in a "1D" fashion
                //write current, diagonal, above and current, right, diagonal
                int above = current - width; //the point above the current
                int rDiagonal = above + 1; //the point in the right diagonal of the current
                int right = current + 1;//the point to the right of the current

                triangles.Add(current);
                triangles.Add(rDiagonal);
                triangles.Add(above);

                triangles.Add(current);
                triangles.Add(right);
                triangles.Add(rDiagonal);
            }
        }

        mesh.SetVertices(vertices);
        mesh.triangles = triangles.ToArray();
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        return mesh;
    }

}

public struct MeshData {
    public Vector3[] vertices;
    public Vector2[] uvs;
    public int[] triangles;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight) {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        triangleIndex = 0;
    }

    public void AddTriangle(int a, int b, int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}
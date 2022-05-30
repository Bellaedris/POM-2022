using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class MeshGenerator
{
    public static MeshData GenerateMesh(float[,] noisemap, float[,] sedmap, float maxHeight, int width, int height, AnimationCurve flattening) {
        int nx = noisemap.GetLength(1);
        int ny = noisemap.GetLength(0);
       
        MeshData meshData = new MeshData(nx, ny);
        int vertexIndex = 0;

        //calculate vertices
        for (int y = 0; y < ny; y++)
        {
            //only write the point if he is not at the end of a line (as it would not have a point to its right)
            for (int x = 0; x < nx; x++)
            {
                lock(flattening) {
                    meshData.vertices[vertexIndex] = new Vector3((float) x, (flattening.Evaluate(noisemap[x, y]) + sedmap[x,y]) * maxHeight, (float) y);
                }
                meshData.uvs[vertexIndex] = new Vector2(x / (float) width, y / (float) height);

                if (x < width - 1 && y > 0) {
                    meshData.AddTriangle(vertexIndex, vertexIndex - nx + 1, vertexIndex - nx);
                    meshData.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex - nx + 1);
                }
                vertexIndex++;
            }
        }
        return meshData;
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
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}
using UnityEngine;

public static class Utils
{
    public static float sqrDist(Vector2 v1, Vector2 v2)
    {
        float dx = v1.x - v2.x;
        float dy = v1.y - v2.y;
        return dx * dx + dy * dy;
    }

    public static float[,] ApplyConvolution(float[,] map, int[,] kernel)
    {
        int kernelSize = kernel.GetLength(0);
        int kernelMiddle = kernelSize / 2;

        float[,] cpy = map;

        //get normalization factor
        int normalize = 0;
        for (int i = 0; i < kernelSize; i++)
        {
            for (int j = 0; j < kernelSize; j++)
            {
                if (kernel[i, j] > 0)
                    normalize += kernel[i, j];
            }
        }


        for (int x = kernelMiddle; x < map.GetLength(0) - kernelMiddle; x++)
        {
            for (int y = kernelMiddle; y < map.GetLength(1) - kernelMiddle; y++)
            {
                float val = 0;
                for (int u = -kernelMiddle; u <= kernelMiddle; u++)
                {
                    for (int v = -kernelMiddle; v <= kernelMiddle; v++)
                    {
                        val += kernel[kernelMiddle + u, kernelMiddle + v] * cpy[x + v, y + u];
                    }
                }
                map[x, y] = val / normalize;
            }
        }

        return map;
    }

    public static float InterpolLagrange(float x, float[] xs, float[] ys)
    {
        float res = 0;
        float l;

        for (int j = 0; j < ys.Length; j++)
        {
            l = 1;
            for (int i = 0; i < xs.Length; i++)
            {
                l *= (x - xs[i]) / (xs[j] - xs[i]);
            }
            res += l * ys[j];
        }
        return res;
    }

    public static int[,] gaussianKernel55 = {
        {1, 4, 6, 4, 1},
        {4, 16, 24, 16, 4},
        {6, 24, 36, 24, 6},
        {4, 16, 24, 16, 4},
        {1, 4, 6, 4, 1}
    };

    public static int[,] mean55 = {
        {1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1},
    };
}

using UnityEngine;

public static class Utils
{
    public static float sqrDist(Vector2 v1, Vector2 v2)
    {
        float dx = v1.x - v2.x;
        float dy = v1.y - v2.y;
        return dx * dx + dy * dy;
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
}

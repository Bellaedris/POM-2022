using UnityEngine;

public static class Utils
{
    public static float sqrDist(Vector2 v1, Vector2 v2) {
        float dx = v1.x - v2.x;
        float dy = v1.y - v2.y;
        return dx * dx + dy * dy;
    }
}

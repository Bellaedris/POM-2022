using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a cell able to walk randomly
public class Particle
{
    public Vector3 pos;
    public bool aggregated;
    public int timeAlive; //kill particles that drift away

    public Particle(float x, float y, float z, bool agg = false)
    {
        this.pos = new Vector3(x, y, z);
        this.aggregated = agg;
        this.timeAlive = 0;
    }

    // apply brownian motion and constrain the movements to our heightmap domain
    public void RandomWalk(int width, int height, Vector2 direction, float terrainBias, float depthBias)
    {
        this.pos.x = Mathf.Clamp(this.pos.x + Random.Range(-1f, 1f) + direction.x * terrainBias, 0, width - 1);
        this.pos.y = Mathf.Clamp(this.pos.y + Random.Range(0.01f * depthBias, 0.1f * depthBias), 0, 1);
        this.pos.z = Mathf.Clamp(this.pos.z + Random.Range(-1f, 1f) + direction.y * terrainBias, 0, height - 1);
        this.timeAlive += 1;
    }

    // kills the particle if it goes too deep or above surface, or if it was alive for too long
    public bool Kill(AnimationCurve depthReistance, int MAX_JUMPS)
    {
        return (timeAlive >= MAX_JUMPS || Random.value > depthReistance.Evaluate(pos.y));
    }

    public static Particle SpawnInRange(int width, int height, float[,] hm, float minDepth, float maxDepth) {
        Particle p;
        do {
            p = new Particle(Random.Range(0, width), Random.Range(minDepth, maxDepth), Random.Range(0, height));
        } while(hm[(int)p.pos.x, (int)p.pos.z] >= maxDepth || hm[(int)p.pos.x, (int)p.pos.z] < minDepth);

        return p;
    }

}
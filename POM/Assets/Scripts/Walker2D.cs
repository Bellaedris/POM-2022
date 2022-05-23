using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker
{
    public Vector2 pos;
    public bool aggregated;
    public int timeAlive; //kill particles that drift away

    public Walker(float x, float y, bool agg = false)
    {
        this.pos = new Vector2(x, y);
        this.aggregated = agg;
        this.timeAlive = 0;
    }

    public static Walker SpawnInRange(int width, int height, float[,] hm, float minDepth, float maxDepth)
    {
        Walker w;
        do
        {
            w = new Walker(Random.Range(0, width), Random.Range(0, height));
        } while (hm[(int)w.pos.x, (int)w.pos.y] >= maxDepth || hm[(int)w.pos.x, (int)w.pos.y] < minDepth);
        return w;
    }

    // apply brownian motion and constrain the movements to our heightmap domain
    public void RandomWalk(int width, int height, int step)
    {
        this.pos.x = Mathf.Clamp(this.pos.x + Random.Range(-step, step + 1), 0, width - 1);
        this.pos.y = Mathf.Clamp(this.pos.y + Random.Range(-step, step + 1), 0, height - 1);
        this.timeAlive += 1;
    }

    // checks if 2 points are close enough to be aggregated
    public bool CheckAggregated(Aggregate agg, int cellSize, float[,] heightmap, float shallowWaterLimit)
    {
        for (int i = 0; i < agg.size(); i++)
        {
            // uses sqrdist instead of regular dist to avoid sqrt
            if (Utils.sqrDist(pos, agg.get(i).pos) <= cellSize * cellSize * 4)
            {
                if (Random.value <= heightmap[(int)pos.x, (int)pos.y] && heightmap[(int)pos.x, (int)pos.y] <= shallowWaterLimit)
                {
                    this.aggregated = true;
                    return true;
                }
            }
        }

        return false;
    }
}
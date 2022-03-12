using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DLAGenerator
{

    public const int maxAge = 100;

    #region methods
    public static float[,] GenerateDLA(float[,] heightmap, int particles, int width, int height, int cellSize)
    {
        Aggregate agg = new Aggregate(width / 2, height / 2, cellSize);
        List<Walker> walkers = new List<Walker>(particles);
        for (int i = 1; i < particles; i++)
        {
            walkers.Add(new Walker(Random.Range(0, width), Random.Range(0, height)));
        }

        while (agg.size() < particles)
        {
            Walker walker = new Walker(Random.Range(0, width), Random.Range(0, height)); 
            while(!walker.aggregated) {
                if (walker.CheckAggregated(agg, cellSize, heightmap))
                {
                    agg.AddCell(walker);
                    break;
                }
                walker.RandomWalk(width, height, cellSize);
                if (walker.timeAlive >= maxAge) {
                    Vector2 randCoord;
                    do {
                        randCoord = new Vector2(Random.Range(0, width), Random.Range(0, height));
                    } while(agg.IsInside(randCoord));
                    walkers.Add(new Walker(randCoord.x, randCoord.y));
                    break;
                }
            }
        }
        
        // simply raise the points of the heightmap to 1
        // loops over each walker and sets all points to 1 in its radius
        foreach (Walker w in agg.aggregate)
        {
            heightmap[(int)w.pos.x, (int)w.pos.y] += 0.01f;
            float wHeight = heightmap[(int)w.pos.x, (int)w.pos.y];
            int radius = cellSize / 2;
            //iterate over a subset of the heightmap to update the points in radius of center
            for(int i = (int)w.pos.y - radius; i < (int)w.pos.y + radius; i++) {
                for(int j = (int)w.pos.x - radius; j < (int)w.pos.x + radius; j++) {
                    float oldHeight = heightmap[i, j];
                    Vector2 tmp = new Vector2(j, i);
                    if (Utils.sqrDist(w.pos, tmp) <= radius * radius && 
                        i > 0 && j > 0 && i < width && j < height) {
                        heightmap[i, j] = Mathf.Clamp(wHeight - (((tmp - w.pos).magnitude / cellSize) * ((tmp - w.pos).magnitude / cellSize) + 0.01f), oldHeight, wHeight);
                    }
                    /*if (Utils.sqrDist(w.pos, new Vector2(j, i)) <= radius * radius && 
                        i > 0 && j > 0 && i < width && j < height) {
                        heightmap[j, i] = 1;
                    }*/
                }
            }
        }

        return heightmap;
    }
    #endregion

}



class Walker
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

    public void RandomWalk(int width, int height, float cellSize)
    {
        this.pos.x += Random.value;
        this.pos.y += Random.value;
        this.pos.x = Mathf.Clamp(this.pos.x, 0, width - 1);
        this.pos.y = Mathf.Clamp(this.pos.y, 0, height - 1);
        this.timeAlive += 1;
    }

    public bool CheckAggregated(Aggregate agg, int cellSize, float[,] heightmap)
    {
        for (int i = 0; i < agg.aggregate.Count; i++)
        {
            if (Utils.sqrDist(this.pos, agg.get(i).pos) <= cellSize * cellSize)
            {
                if (Random.value <= heightmap[(int)this.pos.x, (int)this.pos.y])
                {
                    this.aggregated = true;
                    return true;
                }
            }
        }

        return false;
    }
}

class Aggregate
{
    public Vector2 boundsA;
    public Vector2 boundsB;
    public List<Walker> aggregate;
    public int cellSize;

    public Aggregate(int x, int y, int cellSize)
    {
        this.boundsA = new Vector2(float.PositiveInfinity, float.NegativeInfinity);
        this.boundsB = new Vector2(float.NegativeInfinity, float.PositiveInfinity);
        this.aggregate = new List<Walker>();
        this.cellSize = cellSize;
        this.aggregate.Add(new Walker(x, y, true));
    }

    public int size() {
        return aggregate.Count;
    }

    public Walker get(int i) {
        return aggregate[i];
    }

    public void AddCell(Walker cell)
    {
        aggregate.Add(cell);
        UpdateBounds(cell);
    }

    public void UpdateBounds(Walker cell)
    {
        if (cell.pos.x + cellSize < this.boundsA.x)
            this.boundsA.x = cell.pos.x + cellSize;
        if (cell.pos.x + cellSize > this.boundsB.x)
            this.boundsB.x = cell.pos.x + cellSize;
        if (cell.pos.y + cellSize > this.boundsA.y)
            this.boundsA.y = cell.pos.y + cellSize;
        if (cell.pos.y + cellSize < this.boundsB.y)
            this.boundsB.y = cell.pos.y + cellSize;
    }

    public bool IsInside(Vector2 p)
    {
        return (p.x > this.boundsA.x) && (p.x < this.boundsB.x) && (p.y < this.boundsA.y) && (p.y > this.boundsB.y);
    }

}
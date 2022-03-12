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
            walkers.Add(agg.SpawnOutsideBoundingBox(width, height));
        }

        //DLA generation, fire an individual particle until the aggregate has the desired number of particles
        while (agg.size() < particles)
        {
            Walker walker = new Walker(Random.Range(0, width), Random.Range(0, height));
            // randomly move the walker until he's part of the aggregate
            while (!walker.aggregated)
            {
                if (walker.CheckAggregated(agg, cellSize, heightmap))
                {
                    agg.AddCell(walker);
                    break;
                }
                walker.RandomWalk(width, height);
                // kill walkers that have been moving for too long and spawn another one outside the bounding box of the aggregate
                if (walker.timeAlive >= maxAge)
                {
                    walkers.Add(agg.SpawnOutsideBoundingBox(width, height));
                    break;
                }
            }
        }

        // loops over each walker and raise the points in its radius following a specific formula
        foreach (Walker w in agg.aggregate)
        {
            heightmap[(int)w.pos.x, (int)w.pos.y] += 0.05f;
            float wHeight = heightmap[(int)w.pos.x, (int)w.pos.y];
            //iterate over a subset of the heightmap to update the points in radius of center
            for (int i = (int)w.pos.y - cellSize; i < (int)w.pos.y + cellSize; i++)
            {
                for (int j = (int)w.pos.x - cellSize; j < (int)w.pos.x + cellSize; j++)
                {
                    //only update points inside of the heightmap bounds
                    if (i > 0 && j > 0 && i < width && j < height)
                    {
                        float oldHeight = heightmap[j, i];
                        Vector2 tmp = new Vector2(j, i);
                        if (Utils.sqrDist(w.pos, tmp) <= cellSize * cellSize)
                        {
                            // remap height using a 1/x * 1/x + heightIncrease function (should change to a gaussian function at some point)
                            heightmap[j, i] = Mathf.Lerp(oldHeight, wHeight, ((-(tmp - w.pos).magnitude / cellSize) * ((tmp - w.pos).magnitude / cellSize) + 1));
                        }
                    }
                }
            }
        }

        return heightmap;
    }
    #endregion

}

// a cell able to walk randomly
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

    // apply brownian motion and constrain the movements to our heightmap domain
    public void RandomWalk(int width, int height)
    {
        this.pos.x += Random.value;
        this.pos.y += Random.value;
        this.pos.x = Mathf.Clamp(this.pos.x, 0, width - 1);
        this.pos.y = Mathf.Clamp(this.pos.y, 0, height - 1);
        this.timeAlive += 1;
    }

    // checks if 2 points are close enough to be aggregated
    public bool CheckAggregated(Aggregate agg, int cellSize, float[,] heightmap)
    {
        for (int i = 0; i < agg.aggregate.Count; i++)
        {
            // uses sqrdist instead of regular dist to avoid sqrt
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

// an aggregate that contains walkers
public class Aggregate
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

    public int size()
    {
        return aggregate.Count;
    }

    public Walker get(int i)
    {
        return aggregate[i];
    }

    // add a cell to the aggregate and immediately updates the bounding box
    public void AddCell(Walker cell)
    {
        aggregate.Add(cell);
        UpdateBounds(cell);
    }

    // update the bounding box of the aggregate with the newly added cell
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

    // places a walker randomly until it can be placed outside of the aggregate bounding box
    public Walker SpawnOutsideBoundingBox(int width, int height)
    {
        Vector2 randCoord;
        do
        {
            randCoord = new Vector2(Random.Range(0, width), Random.Range(0, height));
        } while (IsInside(randCoord));
        return new Walker(randCoord.x, randCoord.y);
    }

    // checks if a point is inside the aggregate bounding box
    public bool IsInside(Vector2 p)
    {
        return (p.x > this.boundsA.x) && (p.x < this.boundsB.x) && (p.y < this.boundsA.y) && (p.y > this.boundsB.y);
    }

}
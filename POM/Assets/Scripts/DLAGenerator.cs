using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class DLAGenerator
{

    #region variables

    #endregion

    #region methods
    public static float[,] GenerateDLA(float[,] heightmap, int particles, int width, int height, int cellSize) {
        Aggregate agg = new Aggregate(width / 2, height / 2);
        List<Walker> walkers = Enumerable.Repeat(new Walker(Random.Range(0, width), Random.Range(0, height)), particles).ToList();

        //while(agg.aggregate.Count <= particles) {
            // displaces all the walkers a set number of times
            // also removes the walkers that got aggregated
            for (var i = 0; i < walkers.Count; i++) {
                walkers[i].RandomWalk(width, height);
                if (walkers[i].CheckAggregated(agg, cellSize, heightmap)) {
                    agg.AddCell(walkers[i]);
                    walkers.RemoveAt(i);
                }
            }
        //}

        Debug.Log(agg.aggregate.Count);
        Debug.Log(walkers.ToString());
        
        foreach (Walker w in agg.aggregate)
        {
            heightmap[(int)w.pos.x, (int)w.pos.y] = 1;
        }

        return heightmap;
    }
    #endregion

}

struct Walker {
    public Vector2 pos;
    public bool aggregated;

    public Walker(int x, int y, bool agg = false) {
        this.pos = new Vector2(x, y);
        this.aggregated = agg;
    }

    public void RandomWalk(int width, int height) {
        this.pos += new Vector2(Random.value, Random.value).normalized;
        Mathf.Clamp(this.pos.x, 0, width);
        Mathf.Clamp(this.pos.y, 0, height);
    }

    public bool CheckAggregated(Aggregate agg, int cellSize, float[,] heightmap) {
        for (int i = 0; i < agg.aggregate.Count; i++) {
            if (Utils.sqrDist(this.pos, agg.aggregate[i].pos) <= cellSize * cellSize) {
                if (Random.value >= heightmap[(int)this.pos.x, (int)this.pos.y]) {
                    this.aggregated = true;
                    return true;
                }
            }
        }

        return false;
    }
}

struct Aggregate {
    public Vector2 boundsA;
    public Vector2 boundsB;
    public List<Walker> aggregate;

    public Aggregate(int x, int y) {
        this.boundsA = new Vector2(float.PositiveInfinity, float.NegativeInfinity);
        this.boundsB = new Vector2(float.NegativeInfinity, float.PositiveInfinity);
        this.aggregate = new List<Walker>();
        this.aggregate.Add(new Walker(x, y, true));
    }

    public void AddCell(Walker cell) {
        aggregate.Add(cell);
        UpdateBounds(cell);
    }

    public void UpdateBounds(Walker cell) {
        if (cell.pos.x < this.boundsA.x)
            this.boundsA.x = cell.pos.x;
        if (cell.pos.x > this.boundsB.x)
            this.boundsB.x = cell.pos.x;
        if (cell.pos.y > this.boundsA.y)
            this.boundsA.y = cell.pos.y;
        if (cell.pos.y < this.boundsB.y)
            this.boundsB.y = cell.pos.y;
    }

    public bool IsInside(Vector2 p) {
        return (p.x > this.boundsA.x) && (p.x < this.boundsB.x) && (p.y < this.boundsA.y) && (p.y > this.boundsB.y);
    }

}
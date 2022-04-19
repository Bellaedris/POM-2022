using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

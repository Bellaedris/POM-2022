using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SandReef2D
{
    public static float[,] GenerateSandReef2D(float[,] heightmap, int particles, int width, int height, int cellSize, float heightIncrement)
    {
        int maxAge = width;
        int agg = 0;

        //DLA generation, fire an individual particle until the aggregate has the desired number of particles
        while (agg < particles)
        {
            Walker w = Walker.SpawnInRange(width, height, heightmap, 0f, .4f);
            // randomly move the Particle until he's part of the aggregate 
            while (!w.aggregated)
            {
                w.RandomWalk(width, height, cellSize);
                // kill Particles that have been moving for too long or who are not in the ocean anymore (TODO)
                if (w.timeAlive > maxAge)
                {
                    w = Walker.SpawnInRange(width, height, heightmap, 0f, .4f);
                    break;
                }
                //if one point of the particle is above the max threshhold, the sand is out of water and stops moving
                for (int xoff = -cellSize; xoff < cellSize; xoff++)
                {
                    for (int yoff = -cellSize; yoff < cellSize; yoff++)
                    {
                        int newX = (int)w.pos.x + xoff;
                        int newY = (int)w.pos.y + yoff;
                        if (newX >= width || newX < 0 || newY >= width || newY < 0)
                            continue;
                        if (heightmap[newX, newY] >= .4f &&
                            Utils.sqrDist(new Vector2(newX, newY), w.pos) <= cellSize * cellSize
                        )
                        {
                            w.aggregated = true;
                            agg++;
                            break;
                        }
                    }
                }
            }
            //the cell is aggregated, rewrite height
            for (int xoff = -cellSize; xoff < cellSize; xoff++)
            {
                for (int yoff = -cellSize; yoff < cellSize; yoff++)
                {
                    int newX = (int)w.pos.x + xoff;
                    int newY = (int)w.pos.y + yoff;
                    if (newX >= width || newX < 0 || newY >= width || newY < 0)
                            continue;
                    if (Utils.sqrDist(new Vector2(newX, newY), w.pos) <= cellSize * cellSize)
                    {
                        heightmap[newX, newY] += heightIncrement;
                    }
                }
            }

        }
        return heightmap;
    }

}

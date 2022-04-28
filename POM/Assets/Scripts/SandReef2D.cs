using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SandReef2D
{
    public static float[,] GenerateSandReef2D(float[,] heightmap, int particles, int width, int height, int cellSize, float heightIncrement, float shallowWaterLimit)
    {
        int maxAge = width;
        int agg = 0;

        double timer = EditorApplication.timeSinceStartup;

        //DLA generation, fire an individual particle until the aggregate has the desired number of particles
        while (agg < particles)
        {
            Walker w = Walker.SpawnInRange(width, height, heightmap, 0f, shallowWaterLimit);
            // randomly move the Particle until he's part of the aggregate 
            while (!w.aggregated)
            {
                w.RandomWalk(width, height, 1);
                // kill Particles that have been moving for too long or who are not in the ocean anymore (TODO)
                if (w.timeAlive > maxAge)
                {
                    w = Walker.SpawnInRange(width, height, heightmap, 0f, shallowWaterLimit);
                }

                int newX, newY;
                //if one neighbor of the particle is eq or above the max threshhold, the sand is out of water and stops moving
                for (int xoff = -1; xoff <= 1; xoff++)
                {
                    for (int yoff = -1; yoff <= 1; yoff++)
                    {
                        newX = (int)w.pos.x + xoff;
                        newY = (int)w.pos.y + yoff;
                        
                        if (newX >= width || newX < 0 || newY >= width || newY < 0)
                            continue;
                        if (heightmap[newX, newY] >= shallowWaterLimit)
                        {
                            //the cell is aggregated, rewrite height
                            heightmap[(int)w.pos.x, (int)w.pos.y] += heightIncrement;
                            w.aggregated = true;
                            agg++;
                        }
                    }
                }
            }

        }

        Debug.Log("growth in " + (EditorApplication.timeSinceStartup - timer));

        return heightmap;
    }

}

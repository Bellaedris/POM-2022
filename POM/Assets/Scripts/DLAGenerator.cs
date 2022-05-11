using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DLAGenerator
{

    //TODO map the heightmap to an array having the size width/cellSize so the only checks necessary are neighborhood

    #region methods
    public static float[,] GenerateDLA(float[,] heightmap, int particles, int width, int height, int cellSize, float coralsHeight, AnimationCurve distrib)
    {
        int nx = width / cellSize;
        int ny = height / cellSize;

        int[,] mask = new int[nx,ny];
        mask[nx / 2, ny / 2] = 1;

        int maxAge = width;
        Aggregate agg = new Aggregate(nx / 2, ny / 2, cellSize);

        //DLA generation, fire an individual particle until the aggregate has the desired number of particles
        while (agg.size() < particles)
        {
            // spawning anywhere creates big chunks that do not look natural.
            //Walker walker = agg.SpawnOutsideBoundingBox(width, height);
            Walker walker = new Walker(Random.Range(0, nx), Random.Range(0, ny));
            // randomly move the walker until he's part of the aggregate 
            while (!walker.aggregated)
            {
                for(int i = -1; i <= 1; i++) {
                    for(int j = -1; j <= 1; j++) {
                        int newX = (int)walker.pos.x + i;
                        int newY = (int)walker.pos.y + j;
                        if(newX < 0 || newX >= nx || newY < 0 || newY >= ny)
                            continue;
                        if (mask[newX, newY] == 1) {
                            mask[(int)walker.pos.x, (int)walker.pos.y] = 1;
                            walker.pos.x *= cellSize;
                            walker.pos.y *= cellSize;
                            walker.aggregated = true;
                            agg.AddCell(walker);
                        }
                    }
                }
                /*if (walker.CheckAggregated(agg, cellSize, heightmap))
                {
                    agg.AddCell(walker);
                    break;
                }*/
                walker.RandomWalk(nx, ny, cellSize);
                // kill walkers that have been moving for too long and spawn another one outside the bounding box of the aggregate
                if (walker.timeAlive >= maxAge)
                {
                    walker = new Walker(Random.Range(0, nx), Random.Range(0, ny));

                    //walker = agg.SpawnOutsideBoundingBox(width, height);
                    break;
                }
            }
        }

        // CONVOLUTION METHOD SLIGHTLY MODIFIED
        // https://easychair.org/publications/preprint_open/qLmc

        float[,] oldHeights = heightmap;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float conv = 0;
                foreach (Walker w in agg.aggregate)
                {
                    Debug.Log(w.pos);
                    float dist = Vector2.Distance(new Vector2(j, i), w.pos);
                    // new height of the point is: heightmap(point) + desired height * sum of heightmap(point) * exp(-dist(point, point of aggregate)^k)
                    conv += Mathf.Exp(-Mathf.Pow(dist / cellSize, 10f));
                    //conv += distrib.Evaluate(dist / cellSize);
                }

                if (i > 0 && j > 0 && i < width && j < height)
                {
                    heightmap[j, i] = oldHeights[j, i] + coralsHeight * Mathf.Clamp(conv, 0, 1);
                }
            }

        }

        return heightmap;
    }
    #endregion

}
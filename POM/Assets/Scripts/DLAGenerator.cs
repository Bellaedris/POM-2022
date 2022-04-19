using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DLAGenerator
{

    #region methods
    public static float[,] GenerateDLA(float[,] heightmap, int particles, int width, int height, int cellSize, float coralsHeight, AnimationCurve distrib)
    {
        int maxAge = width;
        Aggregate agg = new Aggregate(width / 2, height / 2, cellSize);

        //DLA generation, fire an individual particle until the aggregate has the desired number of particles
        while (agg.size() < particles)
        {
            //TODO fix spawning. Spawning outside of bounding box tends to create long lines
            // spawning anywhere creates big chunks that do not look natural.
            //Walker walker = agg.SpawnOutsideBoundingBox(width, height);
            Walker walker = new Walker(Random.Range(0, width), Random.Range(0, height));
            // randomly move the walker until he's part of the aggregate 
            while (!walker.aggregated)
            {
                if (walker.CheckAggregated(agg, cellSize, heightmap))
                {
                    agg.AddCell(walker);
                    break;
                }
                walker.RandomWalk(width, height, cellSize);
                // kill walkers that have been moving for too long and spawn another one outside the bounding box of the aggregate
                if (walker.timeAlive >= maxAge)
                {
                    Vector2 randPos = new Vector2(Random.Range(0, width), Random.Range(0, height));
                    walker = new Walker(randPos.x, randPos.y);

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoralReefGenerator
{
    public static float[,] GenerateCoralReef(float[,] heightmap, int particles, int width, int height, float heightIncrement, AnimationCurve depthResistance, float shallowWaterLimit, float terrainBias, float depthBias, bool sandCollapse)
    {
        float[,] sedmap = new float[width, height];

        int maxAge = width;
        int agg = 0;

        Vector3 center = new Vector3(width / 2, 0, height / 2);

        //DLA generation, fire an individual particle until the aggregate has the desired number of particles
        while (agg < particles)
        {
            Particle particle = Particle.SpawnInRange(width, height, heightmap, sedmap, 0f, shallowWaterLimit);
            // randomly move the Particle until he's part of the aggregate 
            while (!particle.aggregated)
            {
                //add a bias to the movement, always move toward the center of the isle
                Vector2 direction = (center - particle.pos).normalized;
                particle.RandomWalk(width, height, direction, terrainBias, depthBias);
                float posHeight = heightmap[(int)particle.pos.x, (int)particle.pos.z] + sedmap[(int)particle.pos.x, (int)particle.pos.z];
                // kill Particles that have been moving for too long or who are not in the ocean anymore (TODO)
                if (particle.Kill(depthResistance, maxAge))
                {
                    particle = Particle.SpawnInRange(width, height, heightmap, sedmap, 0f, shallowWaterLimit);
                    break;
                }
                if (particle.pos.y <= posHeight && posHeight <= shallowWaterLimit)
                {
                    sedmap[(int)particle.pos.x, (int)particle.pos.z] += heightIncrement;
                    particle.aggregated = true;
                    agg++;
                }
            }
        }
        if (sandCollapse)
            sedmap = Utils.ApplyConvolution(sedmap, Utils.mean33);

        return sedmap;
    }

    private static float[,] CheckNeighbors(float[,] hm, float[,] sm, int x, int y, float threshold, float heightIncrement)
    {
        //if no sand, don't check as it won't fall
        if (hm[x, y] < threshold)
            return sm;
        //if the height difference is too high with a neighbor, increase the neighbor instead
        float h;
        h = hm[x, y] + sm[x, y];
        /*if (sm[x, y] < threshold)
            return sm;*/
        Coord toProcess = new Coord(x, y);
        if (GetSandfallTarget(hm, sm, x, y, threshold).x != -1)
        {
            toProcess = GetSandfallTarget(hm, sm, toProcess.x, toProcess.y, threshold);
        }

        sm[toProcess.x, toProcess.y] += heightIncrement;

        return sm;
    }

    // checks the neighbors of the cell at x, y and returns one of its neighbors
    // that has a delta of height superior to threshold
    private static Coord GetSandfallTarget(float[,] hm, float[,] sm, int x, int y, float threshold)
    {
        List<Coord> targets = new List<Coord>();

        if (x <= 0 || y <= 0 || x >= hm.GetLength(0) || x >= hm.GetLength(1))
            return new Coord(-1, -1);

        float h = hm[x, y] + sm[x, y];

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (h - hm[x + i, y + j] + sm[x + i, y + j] > threshold && sm[x + i, y + j] > 0)
                    targets.Add(new Coord(x + i, y + j));
            }
        }

        return targets.Count == 0 ? new Coord(-1, -1) : targets[Random.Range(0, targets.Count)];
    }

    struct Coord
    {
        public int x;
        public int y;
        public Coord(int x, int y) { this.x = x; this.y = y; }
    }

}
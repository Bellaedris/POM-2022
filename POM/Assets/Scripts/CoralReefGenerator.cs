using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoralReefGenerator
{
    public static float[,] GenerateCoralReef(float[,] heightmap, int particles, int width, int height, float heightIncrement, AnimationCurve depthResistance, float shallowWaterLimit, float terrainBias, float depthBias)
    {
        float[,] sedmap = new float[width, height];

        int maxAge = width;
        int agg = 0;

        Vector3 center = new Vector3(width / 2, 0, height / 2);

        //DLA generation, fire an individual particle until the aggregate has the desired number of particles
        while (agg < particles)
        {
            Particle particle = Particle.SpawnInRange(width, height, heightmap, 0f, shallowWaterLimit);
            // randomly move the Particle until he's part of the aggregate 
            while (!particle.aggregated)
            {
                //add a bias to the movement, always move toward the center of the isle
                Vector2 direction = (center - particle.pos).normalized;
                particle.RandomWalk(width, height, direction, terrainBias, depthBias);
                // kill Particles that have been moving for too long or who are not in the ocean anymore (TODO)
                if (particle.Kill(depthResistance, maxAge))
                {
                    particle = Particle.SpawnInRange(width, height, heightmap, 0f, shallowWaterLimit);
                    break;
                }
                if (particle.pos.y <= heightmap[(int)particle.pos.x, (int)particle.pos.z] && heightmap[(int)particle.pos.x, (int)particle.pos.z] <= shallowWaterLimit) {
                    sedmap[(int)particle.pos.x, (int)particle.pos.z] += heightIncrement;
                    particle.aggregated = true;
                    agg++;
                }
            }
        }

        //TODO sand pile

        return sedmap;
    }

}
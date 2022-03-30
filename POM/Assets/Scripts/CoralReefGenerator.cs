using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoralReefGenerator
{

    public const int MAX_JUMPS = 500;

    public static float[,] GenerateCoralReef(float[,] heightmap, int particles, int width, int height, float heightIncrement, AnimationCurve depthResistance)
    {
        int maxAge = width;
        int agg = 0;

        //DLA generation, fire an individual particle until the aggregate has the desired number of particles
        while (agg < particles)
        {
            Particle particle = SpawnInOcean(width, height, heightmap);
            // randomly move the Particle until he's part of the aggregate 
            while (!particle.aggregated)
            {
                particle.RandomWalk(width, height);
                // kill Particles that have been moving for too long or who are not in the ocean anymore (TODO)
                if (particle.Kill(depthResistance))
                {
                    particle = SpawnInOcean(width, height, heightmap);

                    break;
                }
                if (particle.pos.y <= heightmap[(int)particle.pos.x, (int)particle.pos.z]) {
                    
                    heightmap[(int)particle.pos.x, (int)particle.pos.z] += .01f;
                    particle.aggregated = true;
                    agg++;
                }
            }
        }

        return heightmap;
    }

    public static Particle SpawnInOcean(int width, int height, float[,] hm) {
        Particle p;
        do {
            p = new Particle(Random.Range(0, width), Random.Range(0f, 1f), Random.Range(0, height));
        } while(hm[(int)p.pos.x, (int)p.pos.z] >= 0.4);

        return p;
    }

    // a cell able to walk randomly
    public class Particle
    {
        public Vector3 pos;
        public bool aggregated;
        public int timeAlive; //kill particles that drift away

        public Particle(float x, float y, float z, bool agg = false)
        {
            this.pos = new Vector3(x, y, z);
            this.aggregated = agg;
            this.timeAlive = 0;
        }

        // apply brownian motion and constrain the movements to our heightmap domain
        public void RandomWalk(int width, int height)
        {
            this.pos.x = Mathf.Clamp(this.pos.x + Random.Range(-1f, 1f), 0, width - 1);
            this.pos.y = Mathf.Clamp(this.pos.y + Random.Range(-0.001f, 0.001f), 0, 1);
            this.pos.z = Mathf.Clamp(this.pos.z + Random.Range(-1f, 1f), 0, height - 1);
            this.timeAlive += 1;
        }

        // kills the particle if it goes too deep or above surface, or if it was alive for too long
        public bool Kill(AnimationCurve depthReistance) {
            if (timeAlive >= MAX_JUMPS)
                return true;
            if (Random.value > depthReistance.Evaluate(pos.y)) {
                return true;
            }
            return false;
        }

    }

}
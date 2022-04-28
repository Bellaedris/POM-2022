using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoise(int width, int height, int octaves, float persistance, float lacunarity, float scale, Vector2 offset, float redistribution, int seed, 
                                        bool islandMode, float waterCoefficient, bool terraces, int terracesStep
    ) {
        float[,] results = new float[width, height]; 

        FastNoiseLite gen = new FastNoiseLite(seed);

        gen.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        gen.SetFractalType(FastNoiseLite.FractalType.FBm);
        gen.SetRotationType3D(FastNoiseLite.RotationType3D.ImproveXYPlanes);
        gen.SetFractalLacunarity(lacunarity);
        gen.SetFractalOctaves(octaves);
        gen.SetFractalGain(persistance);
        

        //seed to have the possibility to recreate a noisemap
        System.Random randomGenerator = new System.Random(seed);
        //random offset
        float offsetX = randomGenerator.Next(-1000, 1000);
        float offsetY = randomGenerator.Next(-1000, 1000);
        Vector2 octaveOffsets = new Vector2(offsetX, offsetY);

        for (float y = 0; y < width; y++) {
            for (float x = 0; x < height; x++) {
                float noiseValue = gen.GetNoise(x * scale + offsetX + offset.x, y * scale + offsetY + offset.y, 0) * 0.5f + 0.5f;
                //redistribution to allow creation of flat areas if need be
                float finalValue = Mathf.Pow(noiseValue, redistribution);
                //create an island shape by lowering noise value the further you are from the middle of the map
                if (islandMode) {
                    float nx = 2 * x / width - 1;
                    float ny = 2 * y / height - 1;
                    float d = 1 - (nx * nx + ny * ny);
                    finalValue = finalValue * Mathf.Lerp(0, 1, d);
                }
                    //finalValue = finalValue - Vector2.Distance(new Vector2(x, y), new Vector2(width / 2, height / 2)) / (width * waterCoefficient);
                    
                if (terraces) 
                    finalValue = Mathf.Round(finalValue * terracesStep) / terracesStep;
                results[(int) x, (int) y] = finalValue;
            }
        }

        return results;
    }

}

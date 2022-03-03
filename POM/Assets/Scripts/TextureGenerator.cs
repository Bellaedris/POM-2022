using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D GenerateColorTexture(float[,] noiseMap, int width, int height, MapGenerator.TerrainType[] biomes) {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[texture.width * texture.height];
        
        for(int y = 0; y < noiseMap.GetLength(0); y++) {
            for(int x = 0; x < noiseMap.GetLength(1); x++) {
                float curHeight = noiseMap[x, y];
                foreach(MapGenerator.TerrainType biome in biomes) {
                    if (curHeight <= biome.heightThreshold) {
                        pixels[y * width + x] = biome.color; 
                        break;
                    }
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        //texture.filterMode = FilterMode.Point; 
        //texture.wrapMode = TextureWrapMode.Clamp;
        return texture;
    }

    public static Texture2D GenerateTexture(float[,] noiseMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[texture.width * texture.height];
        
        for(int y = 0; y < noiseMap.GetLength(0); y++) {
            for(int x = 0; x < noiseMap.GetLength(1); x++) {
                pixels[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]); 
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    public static Texture2D TextureFromColorMap(Color[] colormap, int width, int height) {
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(colormap);
        texture.Apply();
        
        return texture;
    }
}
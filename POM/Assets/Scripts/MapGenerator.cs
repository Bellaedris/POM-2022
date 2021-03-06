using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class MapGenerator : MonoBehaviour
{

    #region variables
    [Header("Generator parameters")]
    public int width = 500;
    public int height = 500;
    [Range(0, 6)]
    public float scale = 1.0f;
    public Vector2 offset;
    public int octaves = 1;
    [Range(0, 1)]
    public float persistance = 1f;
    public float lacunarity = 1f;
    public float redistribution = 1f;
    public float maxHeight = 1;
    public int seed = 1;
    [Tooltip("Proportion of water over land of the island")]
    [Range(0, 1)]
    public float waterCoefficient;
    [Tooltip("Generate island-ish shapes")]
    public bool islandMode;
    [Tooltip("heights will increase by step instead of smoothly")]
    public bool terraces;
    [Tooltip("number of terraces steps")]
    public int terracesSteps = 10;
    [Space(10)]
    [Header("Biomes and colors")]
    public TerrainType[] biomes;
    [Space(10)]
    [Header("Water")]
    [Tooltip("Automatically fill the spaces below the lowest value with water")]
    public bool flattenWaterLevel;
    public AnimationCurve flatWaterlevel;
    [Space(10)]
    
    [Header("Corals")]
    [Tooltip("Number of Cells the DLA should generate")]
    public int DLACells = 50;
    [Tooltip("radius of an aggregated cell")]
    public int cellSize = 1;
    [Tooltip("height a coral should have compared to base terrain")]
    public float coralsHeight = 0.1f;
    [Tooltip("increment of height when sand touches the ground")]
    public float heightIncrement = 0.01f;
    [Tooltip("aspect of the corals")]
    public AnimationCurve coralsAspect;
    [Tooltip("Repartition of the corals depending on the depth")]
    public AnimationCurve depthResistance;
    [Tooltip("Amount of force added to Brownian motion towards center of the island")]
    public float terrainBias;
    [Tooltip("Control how fast the particles sunk under water")]
    public float depthBias;
    [Space(10)]
    [Header("Misc")]
    public bool colorMap;
    public bool generateMesh;
    public bool generateDLA;
    public bool generateReef2D;
    public bool generateReef3D;
    public bool sandCollapse;
    public bool autoUpdate;
    public bool demo;
    public Renderer renderObject;
    public Mesh defaultMesh;


    #endregion

    #region customMethods
    public void GenerateMeshInEditor()
    {
        MapData data = GenerateMap();

        if (generateMesh)
        {
            MeshFilter mesh = renderObject.GetComponent<MeshFilter>();
            MeshData meshData;
            if (flattenWaterLevel)
            {
                meshData = MeshGenerator.GenerateMesh(data.noisemap, data.sedmap, maxHeight, width, height, flatWaterlevel);
                mesh.sharedMesh = meshData.CreateMesh();
            }
            else
                meshData = MeshGenerator.GenerateMesh(data.noisemap, data.sedmap, maxHeight, width, height, AnimationCurve.Linear(0, 0, 1, 1));
            mesh.sharedMesh = meshData.CreateMesh();
        }
        else
        {
            MeshFilter mesh = renderObject.GetComponent<MeshFilter>();
            mesh.sharedMesh = defaultMesh;
        }

        if (colorMap)
        {
            renderObject.sharedMaterial.mainTexture = TextureGenerator.TextureFromColorMap(data.colormap, width, height);
        }
        else
        {
            renderObject.sharedMaterial.mainTexture = TextureGenerator.GenerateTexture(data.noisemap, width, height);
        }

    }

    private MapData GenerateMap()
    {
        float[,] noisemap = NoiseGenerator.GenerateNoise(width, height, octaves, persistance, lacunarity, scale, offset, redistribution, seed,
                                                        islandMode, waterCoefficient, terraces, terracesSteps);
        float[,] sedmap = new float[width, height];

        float shallowLimit = biomes[1].heightThreshold;

        if (generateDLA)
            noisemap = DLAGenerator.GenerateDLA(noisemap, DLACells, width, height, cellSize, coralsHeight, coralsAspect, shallowLimit);
        if (generateReef2D)
            noisemap = SandReef2D.GenerateSandReef2D(noisemap, DLACells, width, height, cellSize, heightIncrement, shallowLimit);
        if (generateReef3D)
            sedmap = CoralReefGenerator.GenerateCoralReef(noisemap, DLACells, width, height, heightIncrement, depthResistance, shallowLimit, terrainBias, depthBias, sandCollapse);

        Color[] colourmap = new Color[width * height];
        for (int y = 0; y < noisemap.GetLength(0); y++)
        {
            for (int x = 0; x < noisemap.GetLength(1); x++)
            {
                float curHeight = noisemap[x, y] + sedmap[x, y];
                foreach (MapGenerator.TerrainType biome in biomes)
                {
                    if (curHeight <= biome.heightThreshold)
                    {
                        colourmap[y * height + x] = biome.color;
                        break;
                    }
                }
            }
        }

        return new MapData(noisemap, colourmap, sedmap);
    }

    #endregion

    #region builtins

    private void Update()
    {

    }

    //checks for incorrect inspector values
    private void OnValidate()
    {
        scale = scale < 0 ? 0 : scale;

        octaves = octaves < 1 ? 1 : octaves;

        lacunarity = lacunarity < 1 ? 1 : lacunarity;

        redistribution = redistribution < 1 ? 1 : redistribution;
    }

    #endregion

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float heightThreshold;
        public Color color;
    }

    public struct MapData
    {
        public float[,] noisemap;
        public float[,] sedmap;
        public Color[] colormap;

        public MapData(float[,] noisemap, Color[] colormap, float[,] sedmap)
        {
            this.noisemap = noisemap;
            this.colormap = colormap;
            this.sedmap = sedmap;
        }
    }
}

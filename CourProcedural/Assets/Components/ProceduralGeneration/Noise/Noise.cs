using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using VTools.Grid;
using static Unity.Burst.Intrinsics.X86.Avx;

[CreateAssetMenu(menuName = "Procedural Generation Method/Noise")]
public class Noise : ProceduralGenerationMethod
{
    [Header ("Global data")]
    [SerializeField] FastNoiseLite.NoiseType noiseType;
    [SerializeField, Range(0.001f, 0.1f)] float frequency = 0.001f;
    [SerializeField, Range(1f, 10f)] float amplitude = 1f;

    [Header("Fractal data")]
    [SerializeField] FastNoiseLite.FractalType fractalType;
    [SerializeField] int octave = 3;
    [SerializeField, Range(0.5f, 1f)] float persistance = 0.5f;
    [SerializeField, Range(2f, 5f)] float lacunarity = 2f;


    [Header ("Height")]
    [SerializeField, Range(-0.5f, 1f)] float rockHeight = -0.5f;
    [SerializeField, Range(-0.5f, 1f)] float grassHeight = -0.5f;
    [SerializeField, Range(-0.5f, 1f)] float sandHeight = -0.5f;

    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        FastNoiseLite noise = new FastNoiseLite(RandomService.Seed);

        InitNoise(noise);

        // Gather noise data
        float[,] noiseData = new float[Grid.Width, Grid.Lenght];

        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Lenght; y++)
            {
                noiseData[x, y] = GetNoiseData(noise, x, y);

                if (noiseData[x, y] >= rockHeight)
                    CreateTile(x, y, ROCK_TILE_NAME);
                else if(noiseData[x, y] >= grassHeight)
                    CreateTile(x, y, GRASS_TILE_NAME);
                else if(noiseData[x, y] >= sandHeight)
                    CreateTile(x, y, SAND_TILE_NAME);
                else
                    CreateTile(x, y, WATER_TILE_NAME);
            }

            await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
        }


    }

    void CreateTile(int x, int y, string type)
    {
        if (Grid.TryGetCellByCoordinates(x, y, out Cell cell))
        {
            AddTileToCell(cell, type, true);
        }
    }

    protected void InitNoise(FastNoiseLite noise)
    {
        noise.SetNoiseType(noiseType);
        noise.SetFrequency(frequency);


        noise.SetFractalType(fractalType);
        noise.SetFractalOctaves(octave);
        noise.SetFractalGain(persistance);
        noise.SetFractalLacunarity(lacunarity);
    }

    protected float GetNoiseData(FastNoiseLite noise, int x, int y)
    {
        var amp = noise.GetNoise(x, y) * amplitude;

        return Mathf.Clamp(amp, -1, 1);
    }

}

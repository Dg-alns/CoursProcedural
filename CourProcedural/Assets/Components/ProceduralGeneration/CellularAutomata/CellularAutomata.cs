using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using VTools.Grid;
using VTools.RandomService;
using VTools.ScriptableObjectDatabase;
using static UnityEngine.Rendering.DebugUI.Table;

[CreateAssetMenu(menuName = "Procedural Generation Method/CellularAutomata")]
public class CellularAutomata : ProceduralGenerationMethod
{
    [SerializeField, Range(1, 100), Tooltip("Chance of water spawn")]
     int noiseDensity = 50;

    [SerializeField, Range(0, 8), Tooltip("Chance of water spawn")]
    int nbGrassAround = 4;

    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {

        CreateNoise();

        List<List<string>> tmpGrid = new();

        for (int i = 0; i < _maxSteps; i++)
        {
            tmpGrid.Clear();

            ChangeAllCell(tmpGrid);

            UpdateGrid(tmpGrid);

            await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
        }







    }

    void CreateNoise()
    {
        for (int i = 0; i < Grid.Width; i++)
        {
            for (int j = 0; j < Grid.Lenght; j++)
            {
                if (Grid.TryGetCellByCoordinates(i, j, out Cell cell))
                {
                    string type = (RandomService.Range(0, 100 + 1) <= noiseDensity) ? WATER_TILE_NAME : GRASS_TILE_NAME;

                    AddTileToCell(cell, type, true);
                }
            }
        }
    }

    void ChangeAllCell(List<List<string>> cells)
    {
        for (int i = 0; i < Grid.Width; i++)
        {
            List<string> row = new();
            for (int j = 0; j < Grid.Lenght; j++)
            {
                if (Grid.TryGetCellByCoordinates(i, j, out Cell cell))
                {
                    row.Add(ChangeCell(cell.Coordinates));
                }
            }

            cells.Add(row);
        }
    }

    void UpdateGrid(List<List<string>> cells)
    {
        for (int i = 0; i < Grid.Width; i++)
        {
            for (int j = 0; j < Grid.Lenght; j++)
            {
                if (Grid.TryGetCellByCoordinates(i, j, out Cell cell))
                {
                    AddTileToCell(cell, cells[i][j], true);
                }
            }
        }
    }

    string ChangeCell(Vector2Int coordinates)
    {
        int nbWater = 0;
        int nbGrass = 0;

        Cell cell;

        for (int i = -1; i < 2; i++)
        {

            DetectTypeCell(coordinates.x + i, coordinates.y + 1, ref nbGrass, ref nbWater);

            DetectTypeCell(coordinates.x + i, coordinates.y - 1, ref nbGrass, ref nbWater);
        }


        DetectTypeCell(coordinates.x - 1, coordinates.y, ref nbGrass, ref nbWater);

        DetectTypeCell(coordinates.x + 1, coordinates.y, ref nbGrass, ref nbWater);

        if (nbGrass >= nbGrassAround)
            return GRASS_TILE_NAME;
        else
            return WATER_TILE_NAME;
    }

    void DetectTypeCell(int x, int y, ref int nbGrass, ref int nbWater)
    {
        if (Grid.TryGetCellByCoordinates(x, y, out Cell cell))
        {
            if (cell.GridObject.Template.Name == GRASS_TILE_NAME)
                nbGrass++;
            else
                nbWater++;
        }
    }




}

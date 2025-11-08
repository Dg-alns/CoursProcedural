using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using VTools.Grid;
using VTools.RandomService;



[CreateAssetMenu(menuName = "Procedural Generation Method/CellularAutomata")]
public class CellularAutomata : ProceduralGenerationMethod
{
    [Header ("Cellular Automata")]

    [SerializeField, Range(1, 100), Tooltip("Chance of water spawn")]
     int noiseDensity = 50;

    [Header("Config of Rules")]

    [SerializeField] RulesConfig rulesConfig;


    [NonSerialized] CellRules rules;

    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        var time = DateTime.Now;

        InitRules();

        Cell[,] cells = new Cell[Grid.Width, Grid.Lenght];
        string[,] types = new string[Grid.Width, Grid.Lenght];

        for (int i = 0; i < Grid.Width; i++)
        {
            for (int j = 0; j < Grid.Lenght; j++) {
                if (Grid.TryGetCellByCoordinates(i, j, out Cell cell))
                    cells[i, j] = cell;
            }
        }


        for (int i = 0; i < Grid.Width; i++)
        {
            for (int j = 0; j < Grid.Lenght; j++)
            {
                CreateNoise(cells[i, j]);
            }
            await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
        }

        for (int i = 0; i < _maxSteps; i++)
        {
            time = DateTime.Now;

            ChangeAllCell(types, cells);

            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Lenght; y++)
                {
                    UpdateGrid(types[x, y], cells[x, y]);
                }

                await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
            }

            Debug.Log($"Generation step {i} completed in {(DateTime.Now - time).TotalSeconds: 0.00} seconds.");

        }
    }

    void InitRules()
    {
        rules = new();

        foreach (CellRulesConfiguration typeRules in rulesConfig.allTypeRules)
        {
            rules.InitType(typeRules.type);
        }

        foreach (CellRulesConfiguration typeRules in rulesConfig.allTypeRules)
        {
            foreach (Rule rule in typeRules.rules)
            {
                switch (rule.type)
                {
                    case RULESTYPE.XAround:
                        rules.AddRule(typeRules.type, CellRules.XContainTypeAround(rule));
                        break;

                    case RULESTYPE.XYAround:
                        rules.AddRule(typeRules.type, CellRules.XYContainTypeAround(rule));
                        break;
                }
            }
        }
    }

    void CreateNoise(Cell cell)
    {
        //string type = (RandomService.Range(0, 100 + 1) <= noiseDensity) ? WATER_TILE_NAME : GRASS_TILE_NAME;

        int nb = RandomService.Range(0, 3);

        string type = nb switch
        {
            0 => WATER_TILE_NAME,
            1 => SAND_TILE_NAME,
            2 => GRASS_TILE_NAME,

            _ => throw new NotImplementedException()
        };

        AddTileToCell(cell, type, true);
           
    }

    void ChangeAllCell(string[,] cellsType, Cell[,] cells)
    {
        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Lenght; y++)
            {
                cellsType[x, y] = ChangeCell(cells[x, y].Coordinates, cells);

            }

        }
    }

    void UpdateGrid(string cellsType, Cell cell)
    {
        if(cell.GridObject.Template.Name != cellsType)
          AddTileToCell(cell, cellsType, true);
        
    }

    string ChangeCell(Vector2Int coordinates, Cell[,] cells)
    {
        rules.ResetAllType();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                DetectTypeCell(cells, coordinates.x + x, coordinates.y + y);
            }
        }

        return rules.ApplyRules(cells[coordinates.x, coordinates.y]);
    }

    void DetectTypeCell(Cell[,] cells, int x, int y)
    {
        if (x < 0 || y < 0 || x >= Grid.Width || y >= Grid.Lenght)
            return;

        Cell cell = cells[x, y];
        if (cell == null)
            return;

        rules.AddTypeCell(cell.GridObject.Template.Name);
    }
}

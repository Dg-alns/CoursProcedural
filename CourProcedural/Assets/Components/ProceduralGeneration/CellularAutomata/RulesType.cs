using System;
using System.Collections.Generic;
using UnityEngine;
using VTools.Grid;

public class RulesType
{
    VTools.Grid.Grid grid;

    Dictionary<string, (int, List<Action<bool>>)> allRules = new();

    public RulesType(VTools.Grid.Grid Grid)
    {
        grid = Grid;
    }



    //void DetectTypeCell(int x, int y, ref int nbGrass, ref int nbWater)
    //{
    //    if (grid.TryGetCellByCoordinates(x, y, out Cell cell))
    //    {

    //        string typeFind = cell.GridObject.Template.Name;

    //        allRules[typeFind].Item1 += 1;

    //        if ( == GRASS_TILE_NAME)
    //            nbGrass++;
    //        else
    //            nbWater++;
    //    }
    //}


}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTools.Grid;

public class CellType
{
    public string type;
    public int nbOffCellAround;
    public bool isTry;

    public CellType(string type)
    {
        this.type = type;
        nbOffCellAround = 0;
        isTry = false;
    }
}

public class RulesTypeCell
{

    static private Dictionary<string, CellType> allCellType = new ()
    {
        {"Grass", new CellType("Grass")},
        {"Water", new CellType("Water")}
    };
     
    static private Dictionary<string, List<Func<CellType, string, string>>> allRules = new ()
    {
        {"Grass", new (){ ContainXTypeAround(4) } },
        {"Water", new (){ } }
    };


    public void ResetAllType()
    {
        Debug.Log("Start reset");

        foreach (CellType item in allCellType.Values)
        {
            item.isTry = false;
            item.nbOffCellAround = 0;
        }
        Debug.Log("Reset all");
    }


    public void AddTypeCell(string type)
    {
        CellType tp = allCellType.Values.FirstOrDefault(t => t.type == type);

        if(tp == null)
            throw new Exception($"Error {type} not in Dict");


        tp.nbOffCellAround++;
    }

    public string ApplyRules(Cell cell)
    {
        string currentType = cell.GridObject.Template.Name;

        CellType tp = GetTypeWithMostCount();

        if (tp == null)
            return currentType;

        Debug.Log($" most cell around are {tp.type} type");

        if (tp == null)
            throw new Exception($"Error {tp.type} not in Dict");


        Debug.Log($" number of rules apply for {tp.type} : {allRules[tp.type].Count}");
        foreach (var rules in allRules[tp.type])
        {
            string newType = rules.Invoke(tp, currentType);

            tp.isTry = true;

            if (newType != currentType)
            {
                return newType;
            }

        }

        return ApplyRules(cell);
    }

    public static Func<CellType, string, string> ContainXTypeAround(int requiredCount)
    {
        return (TargetType, cellType) =>
        {
            return (TargetType.nbOffCellAround >= requiredCount) ? TargetType.type : cellType;
        };
    }

    public CellType GetTypeWithMostCount()
    {
        int max = 0;
        CellType result = null;

        foreach (var cellType in allCellType.Values)
        {
            if(cellType.isTry)
                continue;

            if (cellType.nbOffCellAround > max)
            {
                max = cellType.nbOffCellAround;
                result = cellType;
            }
        }
        return result;
    }
}

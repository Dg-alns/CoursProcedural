using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTools.Grid;

public enum CELLTYPE
{
    Grass,
    Water,
    Sand,


    None
}

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

    static private Dictionary<CELLTYPE, CellType> allCellType = new ()
    {
        {CELLTYPE.Grass, new CellType("Grass")},
        {CELLTYPE.Water, new CellType("Water")}
    };
     
    static private Dictionary<CELLTYPE, List<Func<CellType, string, string>>> allRules = new ()
    {
        {CELLTYPE.Grass, new (){ ContainTypeAround(4) } },
        {CELLTYPE.Water, new (){ } }
    };

    public void AddRule(CELLTYPE type, Func<CellType, string, string> func)
    {
        allRules[type].Add(func);
    }


    public void ResetAllType()
    {
        foreach (CellType item in allCellType.Values)
        {
            item.isTry = false;
            item.nbOffCellAround = 0;
        }
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


        if (tp == null)
            throw new Exception($"Error {tp.type} not in Dict");


        if (allRules[GetType(tp.type)].Count == 0)
        {
            tp.isTry = true;
            return ApplyRules(cell);
        }

        foreach (var rules in allRules[GetType(tp.type)])
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

    CELLTYPE GetType(string type)
    {
        foreach (CELLTYPE item in allCellType.Keys)
        {
            if(item.ToString().Equals(type))
                return item; 
        }

        return CELLTYPE.None;
    }

    public static Func<CellType, string, string> ContainTypeAround(int requiredCount)
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

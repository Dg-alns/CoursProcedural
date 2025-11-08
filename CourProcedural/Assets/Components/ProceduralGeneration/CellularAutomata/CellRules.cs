using System;
using System.Collections.Generic;
using System.Linq;
using VTools.Grid;
using UnityEngine;

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

public class CellRules
{

    static private Dictionary<CellType, List<Func<string, (bool, string)>>> allRules = new();

    public void InitType(CELLTYPE type)
    {
        CellType tp = FindType(type.ToString());

        if (tp == null)
        {
            tp = new(type.ToString());
            allRules[tp] = new();
        }
    }

    public void AddRule(CELLTYPE type, Func<string, (bool, string)> func)
    {
        CellType tp = FindType(type.ToString());

        allRules[tp].Add(func);
    }

    public void ResetAllType()
    {
        foreach (var type in allRules.Keys)
        {
            type.isTry = false;
            type.nbOffCellAround = 0;
        }
    }


    public void AddTypeCell(string type)
    {
        var tp = FindType(type);

        if (tp.type == null)
            throw new Exception($"Error {type} not in Dict");


        tp.nbOffCellAround++;
    }

    public string ApplyRules(Cell cell)
    {
        string currentType = cell.GridObject.Template.Name;

        CellType currentCell = FindType(currentType);


        if (allRules[currentCell].Count == 0)
            return currentType;

        foreach (var rules in allRules[currentCell])
        {
            (bool, string) IsnewType = rules.Invoke(currentCell.type);

            currentCell.isTry = true;

            if (IsnewType.Item1)
            {
                return IsnewType.Item2;
            }

        }

        return currentType;
    }

    static CellType FindType(string type)
    {
        CellType cell = allRules.Keys.FirstOrDefault(t => t.type == type);

        return cell;
    }


    public static Func<string, (bool, string)> XContainTypeAround(Rule rule)
    {
        Constraint constraint = rule.constraint[0];

        CellType cellTypeAround = FindType(constraint.cellAround.ToString());

        if (cellTypeAround == null)
            throw new Exception($"Error {cellTypeAround.type} not in Dict");

        return (currenCellType) =>
        {
            return (cellTypeAround.nbOffCellAround >= constraint.nbAround) ? (true, rule.newCellType.ToString()) : (false, currenCellType);
        };

        
    }

    public static Func<string, (bool, string)> XYContainTypeAround(Rule rule)
    {
        Constraint constraintX = rule.constraint[0];
        Constraint constraintY = rule.constraint[1];

        CellType XcellTypeAround = FindType(constraintX.cellAround.ToString());

        CellType YcellTypeAround = FindType(constraintY.cellAround.ToString());

        if (XcellTypeAround == null)
            throw new Exception($"Error {XcellTypeAround.type} not in Dict");

        if (YcellTypeAround == null)
            throw new Exception($"Error {YcellTypeAround.type} not in Dict");



        return (currenCellType) =>
        {
            bool X = (XcellTypeAround.nbOffCellAround >= constraintX.nbAround);
            bool Y = (YcellTypeAround.nbOffCellAround >= constraintY.nbAround);

            return (X && Y) ? (true, rule.newCellType.ToString()) : (false, currenCellType);
        };
    }
}

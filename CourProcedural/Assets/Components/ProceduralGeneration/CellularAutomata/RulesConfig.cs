using System;
using System.Collections.Generic;
using UnityEngine;
public enum CELLTYPE
{
    Grass,
    Water,
    Sand,


    None
}

public enum RULESTYPE
{
    XAround,
    XYAround,


    None
}


[Serializable]
public class Constraint
{
    public CELLTYPE cellAround;

    [Range(1, 8)] public int nbAround = 4;
}

[Serializable]
public class Rule
{
    public RULESTYPE type;
    public CELLTYPE newCellType;

    public List<Constraint> constraint;
}

[System.Serializable]
public class CellRulesConfiguration
{
    public CELLTYPE type;
    public List<Rule> rules;
}


[CreateAssetMenu(menuName = "Procedural Generation Method/RulesConfiguration")]
public class RulesConfig : ScriptableObject
{
    [SerializeField] public List<CellRulesConfiguration> allTypeRules = new();
}

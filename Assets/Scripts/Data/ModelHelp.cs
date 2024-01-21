using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelHelp : System.Attribute
{
    public bool IsCreated { get; set; }
    public string FieldName { get; set; }
    public string Type { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsCanBeNull { get; set; }

    public ModelHelp(bool isc, string filname , string type , bool isprikey, bool isCanBeNull=false)
    {
        IsCreated = isc;
        FieldName = filname;
        Type = type;
        IsPrimaryKey = isprikey;
        IsCanBeNull = isCanBeNull;
    }
}

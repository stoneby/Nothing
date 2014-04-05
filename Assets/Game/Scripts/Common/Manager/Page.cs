using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
    public int Id;

    public string Name;

    [HideInInspector]
    public string Path;

    public int Level { get; set; }

    public List<Page> Children { get; set; }

    public bool IsRoot
    {
        get { return (Level == 0); }
    }

    // Use this for initialization
    void Start()
    {

    }
}

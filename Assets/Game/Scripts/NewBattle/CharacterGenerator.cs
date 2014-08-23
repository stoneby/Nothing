using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterGenerator : MonoBehaviour
{
    public GameObject Parent;

    public List<Character> CharacterList { get; set; }

    public abstract void Cleanup();

    public abstract void Generate();

    public abstract void Return(GameObject go);
}

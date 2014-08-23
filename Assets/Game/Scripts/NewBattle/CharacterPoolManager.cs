using System.Collections.Generic;
using UnityEngine;

public class CharacterPoolManager : Singleton<CharacterPoolManager>
{
    public List<MyPoolManager> CharacterPoolList;

    private const string BasePath = "Prefabs/NewBattle/Character";

    private bool initialized;

    private void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        if (CharacterPoolList != null && CharacterPoolList.Count != 0)
        {
            return;
        }

        Logger.LogWarning("Dynamic binding mode is on, loading resources in the follow path: " + BasePath);

        var characterList = Resources.LoadAll<GameObject>(BasePath);
        foreach (var characterPrefab in characterList)
        {
            GeneratePoolManager(characterPrefab);
        }
    }

    public void Reset()
    {
        if (!initialized)
        {
            return;
        }

        initialized = false;
    }

    private void GeneratePoolManager(GameObject spawnObject)
    {
        var go = new GameObject(spawnObject.name);
        var pool = go.AddComponent<MyPoolManager>();
        pool.SpawnObject = spawnObject;
        pool.Initialize();
        go.transform.parent = transform;

        CharacterPoolList.Add(pool);
    }

    private void Awake()
    {
        Initialize();
    }
}

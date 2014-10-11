using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterPoolManager : Singleton<CharacterPoolManager>
{
    public List<MyPoolManager> CharacterPoolList;

    private const string BasePath = "AssetBundles/Prefabs/NewBattle/Character";

    private bool initialized;

    public void Initialize()
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
        var characterList = ResoucesManager.Instance.LoadAll<GameObject>(BasePath);
        characterList = characterList.Where(ch => ch.name.StartsWith("0")).ToArray();
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

    public void Cleanup()
    {
        CharacterPoolList.ForEach(pool => pool.Cleanup());
        CharacterPoolList.ForEach(pool => pool.SpawnObject = null);
    }

    /// <summary>
    /// Take pool item from pool index.
    /// </summary>
    /// <param name="index">The index of pool</param>
    public GameObject Take(int index)
    {
        if (index < 0 || index >= CharacterPoolList.Count)
        {
            Logger.LogError("Index is out of range [0, " + CharacterPoolList.Count + ")" + ", but index is: " + index);
            return null;
        }
        var pool = CharacterPoolList[index];
        if (pool.SpawnObject == null)
        {
            // resouce file are 1 based, but our index is 0 based.
            pool.SpawnObject = Resources.Load<GameObject>(string.Format("{0}/{1:000}", BasePath, (index + 1)));
        }
        return pool.Take();
    }

    /// <summary>
    /// Return game object to pool of index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="go"></param>
    public void Return(int index, GameObject go)
    {
        if (index < 0 || index >= CharacterPoolList.Count)
        {
            Logger.LogError("Index is out of range [0, " + CharacterPoolList.Count + ")" + ", but index is: " + index);
            return;
        }
        var pool = CharacterPoolList[index];
        pool.Return(go);
    }

    private void GeneratePoolManager(GameObject spawnObject)
    {
        var go = new GameObject(spawnObject.name);
        var pool = go.AddComponent<MyPoolManager>();
        pool.SpawnObject = spawnObject;
        pool.Capacity = 0;
        pool.Initialize();
        go.transform.parent = transform;

        CharacterPoolList.Add(pool);
    }

    private void Awake()
    {
        ResoucesManager.Instance.OnResourcesDownLoaded += Initialize;
    }
}

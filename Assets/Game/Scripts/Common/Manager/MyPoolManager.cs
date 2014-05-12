using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class MyPoolManager : MonoBehaviour
{
    #region Public Fields

    public int Capcity = DefaultCapcity;
    public GameObject SpawnObject;

    public const int DefaultCapcity = 4;

    #endregion

    #region Public Properties

    public List<GameObject> ObjectList { get; set; }
    public GameObject CurrentObject { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Take one game object from pool.
    /// </summary>
    /// <returns>Pooled game object</returns>
    public GameObject Take()
    {
        var inactiveObjectEnum = ObjectList.Where(item => !item.activeSelf);
        var inactiveObjectList = inactiveObjectEnum as IList<GameObject> ?? inactiveObjectEnum.ToList();
        if (!inactiveObjectList.Any())
        {
            Logger.LogWarning("Take too more than Capacity as " + Capcity + ", make it twice more larger.");
            EnlargeCapacity();
            // return the last one.
            CurrentObject = ObjectList[Capcity - 1];
            return CurrentObject;
        }

        CurrentObject = inactiveObjectList.First();
        return CurrentObject;
    }

    /// <summary>
    /// Return back game object to pool manager.
    /// </summary>
    /// <param name="returnObject">Pooled back game object</param>
    public void Return(GameObject returnObject)
    {
        returnObject.SetActive(false);
        returnObject.transform.parent = gameObject.transform;

        if (CurrentObject == returnObject)
        {
            CurrentObject = null;
        }
    }

    #endregion

    #region Private Methods

    private GameObject Spawn()
    {
        var spawn = Instantiate(SpawnObject) as GameObject;
        spawn.SetActive(false);
        spawn.transform.parent = gameObject.transform;
        return spawn;
    }

    private void EnlargeCapacity()
    {
        FillObjectList();
        Capcity += Capcity;
    }

    private void FillObjectList()
    {
        for (var i = 0; i < Capcity; ++i)
        {
            ObjectList.Add(Spawn());
        }
    }

    #endregion

    #region Mono

    void Awake()
    {
        ObjectList = new List<GameObject>(Capcity);

        FillObjectList();
    }

    #endregion
}

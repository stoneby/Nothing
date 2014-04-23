using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class PoolManager : MonoBehaviour
{
    #region Public Fields

    public int Capcity = DefaultCapcity;
    public GameObject SpawnObject;

    public const int DefaultCapcity = 4;

    #endregion

    #region Public Properties

    public List<GameObject> ObjectList { get; set; }

    #endregion

    #region Public Methods

    public GameObject Take()
    {
        var activeObjectEnum = ObjectList.Where(item => item.activeSelf);
        var activeObjectList = activeObjectEnum as IList<GameObject> ?? activeObjectEnum.ToList();
        if (!activeObjectList.Any())
        {
            Debug.LogWarning("Take too more than Capacity as " + Capcity + ", make it twice more larger.");
            EnlargeCapacity();
            // return the last one.
            return ObjectList[Capcity - 1];
        }
        return activeObjectList.First();
    }

    public void Return(GameObject returnObject)
    {
        returnObject.SetActive(false);
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

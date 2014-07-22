using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class UIHeroCommonSurface : MonoBehaviour 
{
    /// <summary>
    /// The key is the uuid of hero, the value is the hero position index.
    /// </summary>
    private Dictionary<long, int> dropped = new Dictionary<long, int>();
 
    public void StartDrop(long uuid)
    {
        if(dropped.ContainsKey(uuid))
        {
            dropped.Remove(uuid);
        }
        else
        {
            var values = dropped.Values;
        }
    }

}

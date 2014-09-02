using com.kx.sglm.gs.battle.share.data;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character intializer.
/// </summary>
public abstract class CharacterInitializer : MonoBehaviour
{
    public GameObject Face;
    public List<Character> CharacterList { get; set; }
    
    public abstract void Initialize(List<FighterInfo> fighterList);

    public abstract void Cleanup();
}

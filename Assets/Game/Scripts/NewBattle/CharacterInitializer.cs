using com.kx.sglm.gs.battle.share.data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Character intializer.
/// </summary>
public abstract class CharacterInitializer : MonoBehaviour
{
    public GameObject Parent;
    public GameObject Face;
    public List<Character> CharacterList { get; set; }

    public List<int> DepthBaseList { get; set; } 
    
    public abstract void Initialize(List<FighterInfo> fighterList);

    public Character GetCharacter(int index, FighterInfo data, int jobIndex, int depthBase)
    {
        var characterPoolManager = CharacterPoolManager.Instance;
        // convert from 1 based template id to 0 based programe id.
        if (index < 0 || index >= characterPoolManager.CharacterPoolList.Count)
        {
            Logger.LogError("Hero index should be in range [0, " + characterPoolManager.CharacterPoolList.Count + ")." + ", take index 0 as default error recovery.");
            // Fault recover, make unknown index as 0. 
            index = 0;
        }
        var character = characterPoolManager.Take(index).GetComponent<Character>();
        Utils.AddChild(Parent, character.gameObject);
        character.Data = data;
        character.IDIndex = index;
        character.JobIndex = jobIndex;
        character.DepthBase = depthBase;
        // set up depth base.
        character.SetupDepthBase(true);

        var faceObject = NGUITools.AddChild(character.gameObject, Face);
        faceObject.SetActive(true);
        character.FaceObject = faceObject;

        return character;
    }

    public virtual void Cleanup()
    {
        for (var i = 0; i < CharacterList.Count; ++i)
        {
            var character = CharacterList[i];

            Cleanup(character);
        }
        CharacterList.Clear();
    }

    public void Cleanup(Character character)
    {
        // set back depth base.
        character.SetupDepthBase(false);

        // return back to pool.
        var index = character.IDIndex;
        character.CanSelected = true;
        CharacterPoolManager.Instance.Return(index, character.gameObject);

        // destroy face object.
        var faceObject = character.FaceObject;
        character.FaceObject = null;
        Destroy(faceObject);
    }
}

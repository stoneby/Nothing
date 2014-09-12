using com.kx.sglm.gs.battle.share.data;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character intializer.
/// </summary>
public abstract class CharacterInitializer : MonoBehaviour
{
    public GameObject Parent;
    public GameObject Face;
    public List<Character> CharacterList { get; set; }
    
    public abstract void Initialize(List<FighterInfo> fighterList);

    public virtual void Cleanup()
    {
        var characterPoolManager = CharacterPoolManager.Instance;
        CharacterList.ForEach(character =>
        {
            var index = character.IDIndex;
            character.CanSelected = true;
            characterPoolManager.CharacterPoolList[index].Return(character.gameObject);
        });

        CharacterList.ForEach(character =>
        {
            var faceObject = character.FaceObject;
            character.FaceObject = null;
            Destroy(faceObject);
        });
    }
}

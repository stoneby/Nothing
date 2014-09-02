using com.kx.sglm.gs.battle.share.data;
using System.Collections.Generic;
using UnityEngine;

public class MultipleCharacterGenerator : CharacterGenerator
{
    public List<FighterInfo> FighterList { get; set; }

    /// <summary>
    /// Character initializer.
    /// </summary>
    public CharacterInitializer Initializer;

    public override void Cleanup()
    {
        Initializer.Cleanup();

        var characterPoolManager = CharacterPoolManager.Instance;
        CharacterList.ForEach(character =>
        {
            var index = character.IDIndex;
            characterPoolManager.CharacterPoolList[index].Return(character.gameObject);
        });
        CharacterList.Clear();
    }

    public override void Generate()
    {
        if (CharacterList == null)
        {
            CharacterList = new List<Character>();
        }

        Initializer.Initialize(FighterList);
        CharacterList.Clear();
        CharacterList.AddRange(Initializer.CharacterList);

        // transform character to parent game object.
        CharacterList.ForEach(character => Utils.AddChild(Parent, character.gameObject));
    }

    public override void Return(GameObject go)
    {
        var character = go.GetComponent<Character>();
        var characterPoolManager = CharacterPoolManager.Instance;
        characterPoolManager.CharacterPoolList[character.Index].Return(go);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class SingleCharacterGenerator : CharacterGenerator
{
    public MyPoolManager CharacterPool;

    public override void Cleanup()
    {
        CharacterList.ForEach(character => CharacterPool.Return(character.gameObject));
        CharacterList.Clear();
    }

    public override void Generate()
    {
        if (CharacterList == null)
        {
            CharacterList = new List<Character>();
        }

        for (var i = 0; i < Total; ++i)
        {
            var character = CharacterPool.Take().GetComponent<Character>();
            CharacterList.Add(character);
            Utils.AddChild(Parent, character.gameObject);
        }
    }

    public override void Return(GameObject go)
    {
        CharacterPool.Return(go);
    }
}

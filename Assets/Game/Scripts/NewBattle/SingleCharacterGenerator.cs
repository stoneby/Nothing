using UnityEngine;

public class SingleCharacterGenerator : CharacterGenerator
{
    public MyPoolManager CharacterPool;
    public int Total;

    public override void Cleanup()
    {
        CharacterList.ForEach(character => CharacterPool.Return(character.gameObject));
    }

    public override void Generate()
    {
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

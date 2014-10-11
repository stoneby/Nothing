using com.kx.sglm.gs.battle.share.data;
using System.Collections.Generic;
using UnityEngine;

public class MultipleCharacterGenerator : CharacterGenerator
{
    /// <summary>
    /// Depth list to manage the drawcalls of character group.
    /// </summary>
    public List<int> DepthBaseList;

    public List<FighterInfo> FighterList { get; set; }

    /// <summary>
    /// Character initializer.
    /// </summary>
    public CharacterInitializer Initializer;

    public override void Cleanup()
    {
        Initializer.Cleanup();

        CharacterList.Clear();
    }

    public override void Generate()
    {
        if (CharacterList == null)
        {
            CharacterList = new List<Character>();
        }

        if (FighterList.Count > DepthBaseList.Count)
        {
            Logger.LogWarning("DepthBaseList count: " + DepthBaseList.Count + " should be equals to FighterList count: " + FighterList.Count);
            var addCount = FighterList.Count - DepthBaseList.Count;
            // make sure depth base list equals or greater than fighter list count, for error recovering.
            for (var i = 0; i < addCount; ++i)
            {
                DepthBaseList.Add(0);
            }
        }

        Initializer.DepthBaseList = DepthBaseList;
        Initializer.Initialize(FighterList);
        CharacterList.Clear();
        CharacterList.AddRange(Initializer.CharacterList);
    }

    public override void Return(GameObject go)
    {
        var character = go.GetComponent<Character>();
        Initializer.Cleanup(character);
    }
}

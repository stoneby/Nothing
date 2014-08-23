using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MultipleCharacterGenerator : CharacterGenerator
{
    public List<FighterInfo> FighterList { get; set; }

    public override void Cleanup()
    {
        var characterPoolManager = CharacterPoolManager.Instance;
        CharacterList.ForEach(character =>
        {
            var index = character.IDIndex;
            characterPoolManager.CharacterPoolList[index].Return(character.gameObject);
        });
    }

    public override void Generate()
    {
        var characterPoolManager = CharacterPoolManager.Instance;
        FighterList.ForEach(data =>
        {
            var tempid = Int32.Parse(data.getProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE));
            var heroTemplate = HeroModelLocator.Instance.GetHeroByTemplateId(tempid);
            // convert from 1 based template id to 0 based programe id.
            var index = heroTemplate.Icon - 1;
            if (index < 0 || index >= characterPoolManager.CharacterPoolList.Count)
            {
                Logger.LogError("Hero index should be in range [0, " + characterPoolManager.CharacterPoolList.Count + ").");
                return;
            }
            var character = characterPoolManager.CharacterPoolList[index].Take().GetComponent<Character>();
            Utils.AddChild(Parent, character.gameObject);
            character.Data = data;
            character.IDIndex = index;
            character.JobIndex = heroTemplate.Job;
            CharacterList.Add(character);
        });
    }

    public override void Return(GameObject go)
    {
        var character = go.GetComponent<Character>();
        var characterPoolManager = CharacterPoolManager.Instance;
        characterPoolManager.CharacterPoolList[character.Index].Return(go);
    }
}

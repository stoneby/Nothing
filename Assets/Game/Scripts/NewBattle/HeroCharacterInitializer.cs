using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data;
using System;
using System.Collections.Generic;

public class HeroCharacterInitializer : CharacterInitializer
{
    public override void Initialize(List<FighterInfo> fighterList)
    {
        if (CharacterList == null)
        {
            CharacterList = new List<Character>();
        }

        CharacterList.Clear();

        var characterPoolManager = CharacterPoolManager.Instance;
        fighterList.ForEach(data =>
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
            character.Data = data;
            character.IDIndex = index;
            character.JobIndex = heroTemplate.Job;
            NGUITools.AddChild(character.gameObject, Face);
            var characterControll = character.GetComponent<CharacterControl>() ?? character.gameObject.AddComponent<CharacterControl>();
            characterControll.CharacterData = character;
            characterControll.gameObject.SetActive(true);
            CharacterList.Add(character);
        });
    }

    public override void Cleanup()
    {
        CharacterList.ForEach(character =>
        {
            var characterControll = character.GetComponent<CharacterControl>() ?? character.gameObject.AddComponent<CharacterControl>();
            Destroy(characterControll.gameObject);
        });
    }
}

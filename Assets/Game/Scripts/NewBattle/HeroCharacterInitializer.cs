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
                Logger.LogError("Hero index should be in range [0, " + characterPoolManager.CharacterPoolList.Count + ")." + ", take index 0 as default error recovery.");
                // Fault recover, make unknown index as 0. 
                index = 0;
            }
            var character = characterPoolManager.CharacterPoolList[index].Take().GetComponent<Character>();
            Utils.AddChild(Parent, character.gameObject);
            character.Data = data;
            character.IDIndex = index;
            character.JobIndex = heroTemplate.Job;
            var faceObject = NGUITools.AddChild(character.gameObject, Face);
            var characterControll = faceObject.GetComponent<CharacterControl>();
            characterControll.CharacterData = character;
            characterControll.gameObject.SetActive(true);
            character.FaceObject = characterControll.gameObject;
            character.BuffBarController = characterControll.BuffBarController;
            CharacterList.Add(character);
        });
    }
}

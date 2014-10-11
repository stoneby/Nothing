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

        for (var i = 0; i < fighterList.Count; ++i)
        {
            var data = fighterList[i];

            var tempid = Int32.Parse(data.getProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE));
            var heroTemplate = HeroModelLocator.Instance.GetHeroByTemplateId(tempid);

            var index = heroTemplate.Icon - 1;
            var character = GetCharacter(index, data, heroTemplate.Job, DepthBaseList[i]);

            var faceObject = character.FaceObject;
            var characterControll = faceObject.GetComponent<CharacterControl>();
            characterControll.CharacterData = character;
            character.BuffBarController = characterControll.BuffBarController;

            CharacterList.Add(character);
        }
    }
}

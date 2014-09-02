using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data;
using System;
using System.Collections.Generic;

public class MonsterCharacterInitializer : CharacterInitializer
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
            var monsterTemplate = BattleTemplateModelLocator.Instance.GetMonsterTemplate(tempid);
            // convert from 1 based template id to 0 based programe id.
            var index = monsterTemplate.Icon - 1;
            if (index < 0 || index >= characterPoolManager.CharacterPoolList.Count)
            {
                Logger.LogError("Hero index should be in range [0, " + characterPoolManager.CharacterPoolList.Count + ").");
                return;
            }
            var character = characterPoolManager.CharacterPoolList[index].Take().GetComponent<Character>();
            character.Data = data;
            character.IDIndex = index;
            character.JobIndex = monsterTemplate.WeakJob;
            NGUITools.AddChild(character.gameObject, Face);
            var monsterControll = character.GetComponent<EnemyControl>() ?? character.gameObject.AddComponent<EnemyControl>();
            monsterControll.CharacterData = character;
            monsterControll.gameObject.SetActive(true);
            CharacterList.Add(character);
        });
    }

    public override void Cleanup()
    {
        CharacterList.ForEach(character =>
        {
            var monsterControll = character.GetComponent<EnemyControl>() ?? character.gameObject.AddComponent<EnemyControl>();
            Destroy(monsterControll.gameObject);
        });
    }
}

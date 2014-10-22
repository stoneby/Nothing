using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCharacterInitializer : CharacterInitializer
{
    private const float RotateValue = 180;

    private Vector3 defaultEulerAngle;
    private Vector3 defaultScale;

    public override void Initialize(List<FighterInfo> fighterList)
    {
        if (CharacterList == null)
        {
            CharacterList = new List<Character>();
        }
        CharacterList.Clear();

        var characterPoolManager = CharacterPoolManager.Instance;
        var characterTrans = characterPoolManager.Take(0).GetComponent<Character>().AnimatedObject.transform;
        defaultScale = characterTrans.localScale;
        defaultEulerAngle = characterTrans.localEulerAngles;

        for (var i = 0; i < fighterList.Count; ++i)
        {
            var data = fighterList[i];

            var tempid = Int32.Parse(data.getProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE));
            var monsterTemplate = BattleTemplateModelLocator.Instance.GetMonsterTemplate(tempid);

            // convert from 1 based template id to 0 based programe id.
            var index = monsterTemplate.Icon - 1;
            var character = GetCharacter(index, data, monsterTemplate.WeakJob, DepthBaseList[i]);

            var faceObject = character.FaceObject;
            var monsterControll = faceObject.GetComponent<MonsterControl>();
            monsterControll.CharacterData = character;
            character.BuffBarController = monsterControll.BuffBarController;

            // rotate 2d character animation to 180.
            var trans = character.AnimatedObject.transform;
            trans.localEulerAngles = new Vector3(0, RotateValue, 0);
            // scale which is 100 based.
            var scale = monsterTemplate.MonsterScale;
            trans.localScale = defaultScale * scale / 100;

            CharacterList.Add(character);
        }
    }

    public override void Cleanup()
    {
        CharacterList.ForEach(character =>
        {
            var trans = character.AnimatedObject.transform;
            trans.localEulerAngles = defaultEulerAngle;
            trans.localScale = defaultScale;
        });

        base.Cleanup();
    }
}

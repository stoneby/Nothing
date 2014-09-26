using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCharacterInitializer : CharacterInitializer
{
    public float NoOneScale;
    public float NoTwoScale;
    
    private const float RotateValue = 180;
    private const int Three = 3;

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
        var characterTrans = characterPoolManager.CharacterPoolList[0].Take().GetComponent<Character>().AnimatedObject.transform;
        defaultScale = characterTrans.localScale;
        defaultEulerAngle = characterTrans.localEulerAngles;

        var counter = 0;
        fighterList.ForEach(data =>
        {
            var tempid = Int32.Parse(data.getProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE));
            var monsterTemplate = BattleTemplateModelLocator.Instance.GetMonsterTemplate(tempid);
            // convert from 1 based template id to 0 based programe id.
            var index = monsterTemplate.Icon - 1;
            if (index < 0 || index >= characterPoolManager.CharacterPoolList.Count)
            {
                Logger.LogWarning("ERROR---------Monster index should be in range [0, " + characterPoolManager.CharacterPoolList.Count + ")." + ", take index 0 as default error recovery.");
                // Fault recover, make unknown index as 0. 
                index = 0;
            }
            var character = characterPoolManager.CharacterPoolList[index].Take().GetComponent<Character>();
            Utils.AddChild(Parent, character.gameObject);

            // rotate 2d character animation to 180.
            var trans = character.AnimatedObject.transform;
            trans.localEulerAngles = new Vector3(0, RotateValue, 0);
            trans.localScale = defaultScale * (fighterList.Count == Three && (counter == fighterList.Count - 1) ? NoOneScale : NoTwoScale);

            character.Data = data;
            character.IDIndex = index;
            character.JobIndex = monsterTemplate.WeakJob;
            var faceObject = NGUITools.AddChild(character.gameObject, Face);
            var monsterControll = faceObject.GetComponent<MonsterControl>();
            monsterControll.CharacterData = character;
            monsterControll.gameObject.SetActive(true);
            character.FaceObject = monsterControll.gameObject;
            character.BuffBarController = monsterControll.BuffBarController;
            CharacterList.Add(character);

            ++counter;
        });

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

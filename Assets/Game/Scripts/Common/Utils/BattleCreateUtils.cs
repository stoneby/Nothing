using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.enums;
using com.kx.sglm.gs.battle.share.factory.creater;
using com.kx.sglm.gs.hero.properties;
using KXSGCodec;
using Template.Auto.Monster;


internal class BattleCreateUtils
{

    public static void initBattleModeLocator(BattleModelLocator locator, SCBattlePveStartMsg battleStartMsg)
    {
        IBattleTemplateService _service = BattleTemplateModelLocator.Instance;
        var _creater = new BattleSourceTemplateCreater(_service);
        var _source = _creater.createPVESource(battleStartMsg);
        _source.Uuid = battleStartMsg.Uuid;

        //赋值给BattleModeLocator
        locator.BattleType = battleStartMsg.BattleType;
        locator.RaidID = battleStartMsg.RaidID;
        locator.Uuid = battleStartMsg.Uuid;
        locator.Source = _source;
        locator.HeroList = _source.getSideFighters(BattleSideEnum.SIDE_LEFT);
        locator.EnemyList = _source.getSideFighters(BattleSideEnum.SIDEB_RIGHT);

        locator.Source = _source;
        locator.EnemyGroup = _source.MonsterGroup;
    }
}


using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.factory
{

	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;
	using FighterInfo = com.kx.sglm.gs.battle.share.data.FighterInfo;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using IBattleExecuter = com.kx.sglm.gs.battle.share.executer.IBattleExecuter;
	using GreenhandPVEBattleExecuter = com.kx.sglm.gs.battle.share.executer.impl.GreenhandPVEBattleExecuter;
	using BattleFighterCreater = com.kx.sglm.gs.battle.share.factory.creater.BattleFighterCreater;
	using IBattleTemplateService = com.kx.sglm.gs.battle.share.factory.creater.IBattleTemplateService;

	public class GreenhandPVEBattleFactory : AbstractBattleFactory
	{

		public override IBattleExecuter createBattleExecuter(Battle battle, IBattleTemplateService tempService)
		{
			GreenhandPVEBattleExecuter _executer = new GreenhandPVEBattleExecuter(battle);
			Template.Auto.Greenhand.GreenhandTemplate _temp = tempService.BattleGreenhandTemplate;
			_executer.initTemplInfo(_temp);
			return _executer;
		}

		public override HeroTeam createAttackerTeam(Battle battle)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<com.kx.sglm.gs.battle.share.data.FighterInfo> _heroList = com.kx.sglm.gs.battle.share.factory.creater.BattleFighterCreater.getSideFighter(battle, com.kx.sglm.gs.battle.share.enums.BattleSideEnum.SIDE_LEFT);
			List<FighterInfo> _heroList = BattleFighterCreater.getSideFighter(battle, BattleSideEnum.SIDE_LEFT);
			HeroTeam _heroTeam = BattleFighterCreater.createHeroTeam(battle, _heroList, BattleSideEnum.SIDE_LEFT);
			_heroTeam.BattleSide = BattleSideEnum.SIDE_LEFT;
			return _heroTeam;
		}

	}

}
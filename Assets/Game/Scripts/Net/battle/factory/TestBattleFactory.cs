using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.factory
{

	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;
	using FighterInfo = com.kx.sglm.gs.battle.share.data.FighterInfo;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using IBattleExecuter = com.kx.sglm.gs.battle.share.executer.IBattleExecuter;
	using TestPVEBattleExecuter = com.kx.sglm.gs.battle.share.executer.impl.TestPVEBattleExecuter;
	using BattleFighterCreater = com.kx.sglm.gs.battle.share.factory.creater.BattleFighterCreater;

	public class TestBattleFactory : AbstractBattleFactory
	{

		public override IBattleExecuter createBattleExecuter(Battle battle)
		{
			return new TestPVEBattleExecuter(battle);
		}

		public override HeroTeam createAttackerTeam(Battle battle)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<com.kx.sglm.gs.battle.share.data.FighterInfo> _heroList = com.kx.sglm.gs.battle.share.factory.creater.BattleFighterCreater.getSideFighter(battle, com.kx.sglm.gs.battle.share.enums.BattleSideEnum.SIDEA);
			List<FighterInfo> _heroList = BattleFighterCreater.getSideFighter(battle, BattleSideEnum.SIDEA);
			HeroTeam _heroTeam = BattleFighterCreater.createHeroTeam(battle, _heroList, BattleSideEnum.SIDEA);
			_heroTeam.BattleSide = BattleSideEnum.SIDEA;
			return _heroTeam;
		}



	}

}
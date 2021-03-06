using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.factory
{

	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;
	using FighterInfo = com.kx.sglm.gs.battle.share.data.FighterInfo;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using BattleFighterCreater = com.kx.sglm.gs.battle.share.factory.creater.BattleFighterCreater;

	public abstract class AbstractPVEBattleFactory : AbstractBattleFactory
	{


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
using System;

namespace com.kx.sglm.gs.battle.share.singleton
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;
	using HeroArrLogicHelper = com.kx.sglm.gs.battle.share.helper.HeroArrLogicHelper;

	public class NormalHeroRecoverAction : ISingletonBattleAction
	{

		public virtual void onAction(BattleFighter attacker, BattleTeam defencerTeam, BattleFightRecord record)
		{
			BattleRecordHelper.initBattleFight(BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVER, attacker, record);
			float _recover = attacker.Recover;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _curIndex = attacker.getOwnerTeam().getCurFightIndex();
			int _curIndex = attacker.getOwnerTeam().CurFightIndex;
			float _ratio = HeroArrLogicHelper.getAttackRatio(_curIndex);
			_recover *= (_ratio / BattleConstants.BATTLE_RATIO_BASE);
			attacker.getOwnerTeam().changeHp((int)_recover, attacker);
			SingleActionRecord _singleRecord = record.OrCreateAttack;
			_singleRecord.ResultHp = attacker.getOwnerTeam().CurHp;
			Console.WriteLine("Fighter fight recover " + _recover);
		}



	}

}
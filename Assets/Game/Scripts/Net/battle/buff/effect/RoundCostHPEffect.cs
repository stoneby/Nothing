using System;

namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeamInfoRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamInfoRecord;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleLogicHelper = com.kx.sglm.gs.battle.share.helper.BattleLogicHelper;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;

	/// <summary>
	/// 每回合减血的buff
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class RoundCostHPEffect : AbstractBuffEffect
	{

		/// <summary>
		/// 减少的百分比 </summary>
		private int reducePercent;
		/// <summary>
		/// 减少的数值 </summary>
		private int reduceValue;

		public override void onActive(BattleFighter fighter)
		{

		}

		public override void onRemove(BattleFighter fighter)
		{

		}


		public override void onDefence(BattleFighter attacker, BattleFighter owner)
		{

		}


		public override void onAttack(BattleFighter attacker)
		{

		}


		public override bool needShow(BattleFighterBuff buffInst)
		{
			return BuffShowId > 0;
		}

		public override void onTeamBeforeAttack(BattleFighter fighter)
		{
			if (!fighter.ActiveFighter)
			{
				return;
			}
			int _reduceHp = getCostPercentValue(fighter);
			_reduceHp += reduceValue;
			_reduceHp = changeToAliveHp(_reduceHp, fighter);
			BattleTeamInfoRecord _teamRecord = fighter.Battle.Record.OrCreateTeamRecord;
			SingleActionRecord _buffAction = _teamRecord.OrCreateCurBuffAction;
			BattleRecordHelper.initDefencerRecord(fighter, _buffAction);
			BattleLogicHelper.costBaseHp(_reduceHp, fighter, _buffAction);
			_teamRecord.finishCurBuffAction();
		}

		internal abstract int getCostPercentValue(BattleFighter fighter);

		protected internal virtual int changeToAliveHp(int reduceHp, BattleFighter fighter)
		{
			int _maxCostHp = fighter.AliveCostMaxHp;
			return reduceHp > _maxCostHp ? _maxCostHp : reduceHp;
		}



		public virtual int ReducePercent
		{
			get
			{
				return reducePercent;
			}
		}

		public virtual int ReduceValue
		{
			get
			{
				return reduceValue;
			}
		}

		public override void build(params string[] param)
		{
			reducePercent = Convert.ToInt32(param[0]);
			if (param.Length > 1)
			{
				reduceValue = Convert.ToInt32(param[1]);
			}
		}

	}

}
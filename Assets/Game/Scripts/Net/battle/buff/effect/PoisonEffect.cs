using System;

namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeamFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;
	using BattleLogicHelper = com.kx.sglm.gs.battle.share.helper.BattleLogicHelper;

	public abstract class PoisonEffect : AbstractBuffEffect
	{

		private int reduceValue;

		public override FighterStateEnum StateEnum
		{
			get
			{
				return FighterStateEnum.NORMAL_STATE;
			}
		}

		public override void onActive(BattleFighter fighter)
		{

		}

		public override void onRemove(BattleFighter fighter)
		{

		}

		public override void onEffect(BattleFighter fighter)
		{
			if (!fighter.ActiveFighter)
			{
				return;
			}
			int _reduceHp = getCostValue(fighter);
			_reduceHp = changeToAliveHp(_reduceHp, fighter);
			BattleTeamFightRecord _fightRecord = fighter.Battle.Record.OrCreateTeamFighterRecord;
			SingleActionRecord _buffAction = _fightRecord.OrCreateCurBuffAction;
			BattleRecordHelper.initDefencerRecord(fighter, _buffAction);
			BattleLogicHelper.costBaseHp(_reduceHp, fighter, _buffAction);
			_fightRecord.finishCurBuffAction();
		}

		internal abstract int getCostValue(BattleFighter fighter);

		public override void onEvent(InnerBattleEvent @event, BattleFighter fighter)
		{

		}

		protected internal virtual int changeToAliveHp(int reduceHp, BattleFighter fighter)
		{
			int _maxCostHp = fighter.AliveCostMaxHp;
			return reduceHp > _maxCostHp ? _maxCostHp : reduceHp;
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
			reduceValue = Convert.ToInt32(param[0]);
		}

	}

}
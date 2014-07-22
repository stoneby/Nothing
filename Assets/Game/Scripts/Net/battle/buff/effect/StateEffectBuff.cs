using System;

namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;

	public class StateEffectBuff : AbstractBuffEffect
	{

		protected internal FighterStateEnum state;

		public override FighterStateEnum StateEnum
		{
			get
			{
				return state;
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

		}

		public override void onEvent(InnerBattleEvent @event, BattleFighter fighter)
		{

		}

		public override void build(params string[] param)
		{
			int _stateFlag = Convert.ToInt32(param[0]);
			state = FighterStateEnum.getValue(_stateFlag);
		}

	}

}
using System;

namespace com.kx.sglm.gs.battle.share.buff.condition
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	public class HPLessCondition : IBuffCondition
	{

		private int hpRate;

		public override bool canOptionBuff(BattleFighter fighter)
		{
			return fighter.FighterCurHpPercent <= hpRate;
		}

		public override void build(params string[] param)
		{
			this.hpRate = Convert.ToInt32(param[0]);
		}


	}

}
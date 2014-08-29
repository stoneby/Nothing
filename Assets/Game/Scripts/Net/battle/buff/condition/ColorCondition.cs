using System;

namespace com.kx.sglm.gs.battle.share.buff.condition
{

	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleLogicHelper = com.kx.sglm.gs.battle.share.helper.BattleLogicHelper;

	public class ColorCondition : IBuffCondition
	{

		private int colorFlag;

		public override bool canOptionBuff(BattleFighter fighter)
		{
			return BattleLogicHelper.checkFitColor(colorFlag, fighter);
		}

		public override void build(params string[] param)
		{
			colorFlag = MathUtils.changeDecToBinFlag(Convert.ToInt32(param[0]), true);
		}

	}

}
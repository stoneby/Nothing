using System;

namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	public class DamagePropEffectBuff : BasePropEffectBuff
	{


		public override FighterStateEnum StateEnum
		{
			get
			{
				return FighterStateEnum.NORMAL_STATE;
			}
		}

		public override void onActive(BattleFighter fighter)
		{
			fighter.FighterProp.addAttackProp(amendTriple);
		}

		protected internal override int getValue(string param)
		{
			return Convert.ToInt32(param);
		}

	}

}
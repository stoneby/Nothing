namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	public abstract class AbstractNormalPropEffectBuff : BasePropEffectBuff
	{

		public override void onActive(BattleFighter fighter)
		{
			fighter.FighterProp.addBuffProp(amendTriple);
		}


		public override FighterStateEnum StateEnum
		{
			get
			{
				return FighterStateEnum.NORMAL_STATE;
			}
		}

	}

}
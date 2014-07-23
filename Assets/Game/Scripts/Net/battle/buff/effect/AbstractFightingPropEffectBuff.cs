namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	public abstract class AbstractFightingPropEffectBuff : AbstractBuffEffect
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
			// TODO Auto-generated method stub

		}

		public override void onRemove(BattleFighter fighter)
		{
			// TODO Auto-generated method stub

		}

		public override void onEffect(BattleFighter fighter)
		{
			// TODO Auto-generated method stub

		}

		public override void build(params string[] param)
		{
			// TODO Auto-generated method stub

		}

	}

}
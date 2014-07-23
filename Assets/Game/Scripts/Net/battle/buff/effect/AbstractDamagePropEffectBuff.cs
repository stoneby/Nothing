namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	/// <summary>
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class AbstractDamagePropEffectBuff : BasePropEffectBuff
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

	}

}
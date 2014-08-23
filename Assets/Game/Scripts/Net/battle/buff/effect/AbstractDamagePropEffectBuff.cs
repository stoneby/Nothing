namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	/// <summary>
	/// 加上，减伤，与<seealso cref="AbstractNormalPropEffectBuff"/>的属性计算方式不同
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


		public override void effectProp(BattleFighter fighter)
		{
			fighter.FighterProp.addAttackProp(amendTriple);
		}

	}

}
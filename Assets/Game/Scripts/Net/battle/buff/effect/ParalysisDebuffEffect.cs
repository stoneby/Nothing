namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	/// <summary>
	/// Âé±Ô
	/// @author liyuan2
	/// 
	/// </summary>
	public class ParalysisDebuffEffect : StateBuffEffect
	{

		public override FighterStateEnum StateEnum
		{
			get
			{
				return FighterStateEnum.ATTACK_ZERO_FLAG;
			}
		}

	}

}
namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	/// <summary>
	/// ·âÄ§debuff
	/// @author liyuan2
	/// 
	/// </summary>
	public class SealSkillDebuffEffect : StateBuffEffect
	{

		public override FighterStateEnum StateEnum
		{
			get
			{
				return FighterStateEnum.SKILL_DIS_FLAG;
			}
		}

	}

}
namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	public class PetrifiedDebuffEffect : StateBuffEffect
	{

		public override FighterStateEnum StateEnum
		{
			get
			{
				return FighterStateEnum.ATTACK_DIS_FLAG;
			}
		}

	}

}
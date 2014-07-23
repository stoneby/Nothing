namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;
	using BattleLogicHelper = com.kx.sglm.gs.battle.share.helper.BattleLogicHelper;

	/// <summary>
	/// �ж�������Ѫ���ٷֱ��Ϻ�
	/// @author liyuan2
	/// 
	/// </summary>
	public class PoisonPrecentEffect : RoundCostHPEffect
	{

		internal override int getCostPercentValue(BattleFighter fighter)
		{
			return BattleLogicHelper.calcBattleFighterTotalHpPercent(ReducePercent, fighter);
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
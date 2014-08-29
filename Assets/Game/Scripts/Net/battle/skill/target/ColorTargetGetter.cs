using System;

namespace com.kx.sglm.gs.battle.share.skill.target
{

	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleLogicHelper = com.kx.sglm.gs.battle.share.helper.BattleLogicHelper;

	/// <summary>
	/// 通过颜色选择目标
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class ColorTargetGetter : AbstractTeamTargetGetter
	{

		private int colorFlag;

		public override bool isFitFlag(BattleFighter fighter)
		{
			return BattleLogicHelper.checkFitColor(colorFlag, fighter);
		}

		public override void buildByType(params string[] param)
		{
			colorFlag = MathUtils.changeDecToBinFlag(Convert.ToInt32(param[0]), true);
		}



	}

}
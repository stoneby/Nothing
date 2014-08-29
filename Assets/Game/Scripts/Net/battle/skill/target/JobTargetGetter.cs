using System;

namespace com.kx.sglm.gs.battle.share.skill.target
{

	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	/// <summary>
	/// 通过职业选择目标
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class JobTargetGetter : AbstractTeamTargetGetter
	{

		private int jobFlag;

		public override bool isFitFlag(BattleFighter fighter)
		{
			return MathUtils.hasFlagIndex(jobFlag, fighter.Job);
		}

		public override void buildByType(params string[] param)
		{
			jobFlag = MathUtils.changeDecToBinFlag(Convert.ToInt32(param[0]), true);
		}

	}

}
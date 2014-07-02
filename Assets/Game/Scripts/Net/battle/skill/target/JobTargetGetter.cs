using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.target
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;

	/// <summary>
	/// 通过职业选择目标
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class JobTargetGetter : AbstractHeroTeamGetter
	{

		private int jobFlag;

		public override List<BattleFighter> getTarget(BattleFighter attacker, BattleTeam targetTeam)
		{
			List<BattleFighter> _resultFighter = new List<BattleFighter>();
			List<BattleFighter> _allActiveFighter = targetTeam.ActiveFighter;
			foreach (BattleFighter _fighter in _allActiveFighter)
			{
				if (MathUtils.hasFlagIndex(jobFlag, _fighter.Job))
				{
					_resultFighter.Add(_fighter);
				}
			}
			return _resultFighter;
		}

		public override void build(params string[] param)
		{
			jobFlag = MathUtils.changeDecToBinFlag(Convert.ToInt32(param[0]), true);
		}

	}

}
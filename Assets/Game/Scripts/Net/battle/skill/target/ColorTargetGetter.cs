using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.target
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;

	/// <summary>
	/// 通过颜色选择目标
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class ColorTargetGetter : AbstractHeroTeamGetter
	{

		private int colorFlag;

		public override List<BattleFighter> getTarget(BattleFighter attacker, BattleTeam targetTeam)
		{
			List<BattleFighter> _fighterList = new List<BattleFighter>();
			foreach (BattleFighter _fighter in targetTeam.AllBattingFighter)
			{
				if (isFitColor(_fighter, targetTeam))
				{
					_fighterList.Add(_fighter);
				}
			}
			return _fighterList;
		}

		protected internal virtual bool isFitColor(BattleFighter fighter, BattleTeam targetTeam)
		{
			int _colorIndex = targetTeam.getFighterColor(fighter.Index);
			return MathUtils.hasFlagIndex(colorFlag, _colorIndex);
		}

		public override void build(params string[] param)
		{
			colorFlag = MathUtils.changeDecToBinFlag(Convert.ToInt32(param[0]), true);

		}

	}

}
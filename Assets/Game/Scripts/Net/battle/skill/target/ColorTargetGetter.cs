using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.target
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;

	public class ColorTargetGetter : AbstractHeroTeamGetter
	{

		private int colorFlag;

		public override List<BattleFighter> getTarget(BattleFighter attacker, BattleTeam targetTeam)
		{
			List<BattleFighter> _fighterList = new List<BattleFighter>();
			foreach (BattleFighter _fighter in targetTeam.ActiveFighter)
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

		public override void build(params int[] param)
		{
			int _baseValue = param[0];
			for (int _i = 0; _i < BattleConstants.INT_SIZE; _i++)
			{
				bool _flag = _baseValue % 10 > 0;
				_baseValue /= 10;
				if (_flag)
				{
					//+1是因为颜色是�? 开始的
					colorFlag = MathUtils.optionOrFlag(colorFlag, _i + 1);
				}
			}

		}

	}

}
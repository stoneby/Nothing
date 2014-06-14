using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.target
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;

	public class RandomTargetGetter : ISkillTargetGetter
	{

		public override List<BattleFighter> getTarget(BattleFighter attacker, BattleTeam targetTeam)
		{
			List<BattleFighter> _curFighterList = new List<BattleFighter>();
			List<BattleFighter> _fighters = targetTeam.ActiveFighter;
			int _index = MathUtils.random(0, _fighters.Count - 1);
			BattleFighter _fighter = _fighters[_index];
			_curFighterList.Add(_fighter);
			return _curFighterList;
		}

		public override void build(params int[] param)
		{

		}

	}

}
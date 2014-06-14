using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.target
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;

	public class IndexTargetGetter : ISkillTargetGetter
	{

		private int index;

		public override List<BattleFighter> getTarget(BattleFighter attacker, BattleTeam defencerTeam)
		{
			List<BattleFighter> _curFighterList = new List<BattleFighter>();
			BattleFighter _fighter = defencerTeam.getActor(index);
			_curFighterList.Add(_fighter);
			return _curFighterList;
		}

		public override void build(params int[] param)
		{
			// TODO Auto-generated method stub

		}



	}

}
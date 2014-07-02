using System;

namespace com.kx.sglm.gs.battle.share.skill.aicondition
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	public class AIRoundCondition : IAICondition
	{

		private int round;

		public override bool canOption(BattleFighter attacker)
		{
			Battle _battle = attacker.Battle;
			int _curRound = _battle.CurSceneRound;
			return round <= _curRound;
		}

		public override void build(params string[] param)
		{
			round = Convert.ToInt32(param[0]);
		}

	}

}
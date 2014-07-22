using System;

namespace com.kx.sglm.gs.battle.share.skill.aicondition
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;

	/// <summary>
	/// dead monster more than condition size
	/// @author liyuan2
	/// 
	/// </summary>
	public class AIDeadMonsterCondition : IAICondition
	{

		private int deadCount;

		public override bool canOption(BattleFighter attacker)
		{
			BattleTeam _monsterTeam = attacker.getOwnerTeam();
			int _curDeadCount = _monsterTeam.allActorSize() - _monsterTeam.AllAliveFighter.Count;
			return deadCount <= _curDeadCount;
		}

		public override void build(params string[] param)
		{
			this.deadCount = Convert.ToInt32(param[0]);
		}


	}

}
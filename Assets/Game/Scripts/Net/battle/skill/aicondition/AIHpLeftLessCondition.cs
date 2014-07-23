using System;

namespace com.kx.sglm.gs.battle.share.skill.aicondition
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	/// <summary>
	/// left hp percent left than condition value
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class AIHpLeftLessCondition : IAICondition
	{

		private int leftHpPercent;

		public override bool canOption(BattleFighter attacker)
		{
			if (attacker.Hero)
			{
				Logger.Log(string.Format("#AIHpLeftLessCondition.canOption, attacker is hero, uuid = {0:D}", attacker.Battle.BattleSource.Uuid));
				return false;
			}
			return leftHpPercent >= attacker.FighterCurHpPercent;
		}

		public override void build(params string[] param)
		{
			this.leftHpPercent = Convert.ToInt32(param[0]);
		}

	}

}
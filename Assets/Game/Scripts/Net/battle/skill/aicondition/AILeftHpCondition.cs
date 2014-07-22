using System;

namespace com.kx.sglm.gs.battle.share.skill.aicondition
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	/// <summary>
	/// left hp percent left than condition value
	/// @author liyuan2
	/// 
	/// </summary>
	public class AILeftHpCondition : IAICondition
	{

		private int leftHpPercent;

		public override bool canOption(BattleFighter attacker)
		{
			float _leftHp = attacker.CurHp;
			float _totalHp = attacker.FighterTotalHp;
			float _percent = _leftHp / _totalHp;
			return leftHpPercent >= (int)(_percent * BattleConstants.BATTLE_RATIO_BASE);
		}

		public override void build(params string[] param)
		{
			this.leftHpPercent = Convert.ToInt32(param[0]);
		}

	}

}
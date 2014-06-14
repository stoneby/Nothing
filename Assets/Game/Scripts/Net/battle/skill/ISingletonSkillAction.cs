using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;

	public interface ISingletonSkillAction
	{

		void onAction(BattleFighter attacker, BattleFightRecord record);

		bool canOption(BattleFighter attacker);

	}

	public static class ISingletonSkillAction_Fields
	{
		public static readonly List<ISkillCondition> emptyList = new List<ISkillCondition>();
	}

}
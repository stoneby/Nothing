using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.ai
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using com.kx.sglm.gs.battle.share.model;

	public class SkillAIHolder : ComparableListHolder<SkillAI>
	{

		public SkillAIHolder(int priority) : base(priority)
		{
			Asc = false;
		}

		public virtual List<SkillRoulettePair> getCanOptionSkill(BattleFighter fighter)
		{
			List<SkillRoulettePair> _resultList = new List<SkillRoulettePair>();
			List<SkillAI> _aiList = getCanOpSkillAi(fighter);
			foreach (SkillAI _ai in _aiList)
			{
				_resultList.Add(_ai.getSkillInfo());
			}
			return _resultList;
		}


		protected internal virtual List<SkillAI> getCanOpSkillAi(BattleFighter attacker)
		{
			List<SkillAI> _aiList = new List<SkillAI>();
			foreach (SkillAI _ai in HoldList)
			{
				if (_ai.Condition.canOption(attacker))
				{
					_aiList.Add(_ai);
				}
			}
			return _aiList;
		}



	}

}
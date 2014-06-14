using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.model
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BaseHeroBattleSkillAction = com.kx.sglm.gs.battle.share.skill.action.BaseHeroBattleSkillAction;

	public class SkillActionHolder
	{

		private int triggerType;
		private List<BaseHeroBattleSkillAction> skillActionList;

		public SkillActionHolder(int triggerType)
		{
			skillActionList = new List<BaseHeroBattleSkillAction>();
			this.triggerType = triggerType;
		}

		public virtual void addSkillAction(BaseHeroBattleSkillAction baseAction)
		{
			this.skillActionList.Add(baseAction);
		}

		public virtual List<BaseHeroBattleSkillAction> getActAction(BattleFighter attacker)
		{
			List<BaseHeroBattleSkillAction> _toActList = new List<BaseHeroBattleSkillAction>();
			foreach (BaseHeroBattleSkillAction _action in skillActionList)
			{
				if (_action.canOption(attacker))
				{
					_toActList.Add(_action);
				}
			}
			return _toActList;
		}


		public virtual bool hasSkillAction()
		{
			return skillActionList.Count == 0;
		}

		public virtual int TriggerType
		{
			get
			{
				return triggerType;
			}
		}



	}

}
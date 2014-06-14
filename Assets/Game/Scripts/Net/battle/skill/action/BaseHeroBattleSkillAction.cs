using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.action
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	/// <summary>
	/// 英雄技�?
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BaseHeroBattleSkillAction : AbstractSingletonBattleAction
	{

		/// <summary>
		/// 触发节点位置ID </summary>
		private int triggerId;

		/// <summary>
		/// 消耗MP </summary>
		private int costMp;

		/// <summary>
		/// 公用概率 </summary>
		private List<ISkillCondition> effectCondition;


		public BaseHeroBattleSkillAction()
		{
			effectCondition = new List<ISkillCondition>();
		}

		public virtual int TriggerId
		{
			get
			{
				return triggerId;
			}
			set
			{
				this.triggerId = value;
			}
		}


		public virtual int CostMp
		{
			get
			{
				return costMp;
			}
			set
			{
				this.costMp = value;
			}
		}


		/// 
		/// <param name="attacker">
		/// @return </param>
		public override bool canOption(BattleFighter attacker)
		{
			bool _option = true;
			foreach (ISkillCondition _condition in effectCondition)
			{
				if (!_condition.canOptionSkill(attacker))
				{
					_option = false;
					break;
				}
			}
			return _option;
		}

		public virtual void addCondition(List<ISkillCondition> conditionList)
		{
			this.effectCondition.AddRange(conditionList);
		}

		//TODO: 用ID判断有点糙，以后�?
		public virtual bool NormalAction
		{
			get
			{
				return SkillId == 0;
			}
		}
	}

}
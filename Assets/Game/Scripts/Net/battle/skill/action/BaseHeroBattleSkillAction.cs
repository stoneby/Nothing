using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.action
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	/// <summary>
	/// Ӣ�ۼ���
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BaseHeroBattleSkillAction : AbstractSingletonBattleAction
	{

		/// <summary>
		/// �����ڵ�λ��ID </summary>
		private int triggerId;

		/// <summary>
		/// ����MP </summary>
		private int costMp;

		/// <summary>
		/// ���ø��� </summary>
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
			if (isFourceTrigger(attacker))
			{
				return _option;
			}
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


		/// <summary>
		/// ǿ�ƴ���flag��Ŀǰֻ��һ�֣� </summary>
		protected internal virtual bool isFourceTrigger(BattleFighter attacker)
		{
			return TriggerId == BattleEventConstants.BATTLE_FIGHTER_ATTACK && attacker.hasState(FighterStateEnum.SP_MAX_STATE);
		}
		public virtual void addCondition(List<ISkillCondition> conditionList)
		{
			this.effectCondition.AddRange(conditionList);
		}

		//TODO: ��ID�ж��е�ڣ��Ժ��
		public virtual bool NormalAction
		{
			get
			{
				return SkillId == 0;
			}
		}
	}

}
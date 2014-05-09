using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data.record
{

	/// <summary>
	/// 单个攻击者的动作，可能包含多个子动作<br>
	/// 暂时不支持攻击者的攻击前动�?
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleFightRecord : AbstractBaseRecord
	{

		/// <summary>
		/// 目标Index </summary>
		private int sourceIndex;

		/// <summary>
		/// 目标阵营 </summary>
		private int sourceSide;

		/// <summary>
		/// 攻击动作 </summary>
		private SingleActionRecord attackAction;

		/// <summary>
		/// 动作列表 </summary>
		private List<SingleActionRecord> actionList;

		/// <summary>
		/// 逻辑用curAction </summary>
		private SingleActionRecord curDefencerAction;

		public BattleFightRecord()
		{
			actionList = new List<SingleActionRecord>();
		}

		public virtual SingleActionRecord OrCreateAttack
		{
			get
			{
				if (attackAction == null)
				{
					attackAction = new SingleActionRecord();
				}
				return attackAction;
			}
		}

		public virtual SingleActionRecord OrCreateDefence
		{
			get
			{
				if (curDefencerAction == null)
				{
					curDefencerAction = new SingleActionRecord();
				}
				return curDefencerAction;
			}
		}

		public virtual void finishCurDefecner()
		{
			actionList.Add(curDefencerAction);
			curDefencerAction = null;
		}

		public virtual int SourceIndex
		{
			get
			{
				return sourceIndex;
			}
			set
			{
				this.sourceIndex = value;
			}
		}


		public virtual int SourceSide
		{
			get
			{
				return sourceSide;
			}
			set
			{
				this.sourceSide = value;
			}
		}


		public virtual SingleActionRecord getAttackAction()
		{
			return attackAction;
		}

		public virtual void setAttackAction(SingleActionRecord attackAction)
		{
			this.attackAction = attackAction;
		}

		public virtual List<SingleActionRecord> ActionList
		{
			set
			{
				this.actionList = value;
			}
			get
			{
				return actionList;
			}
		}


		public virtual void addActionList(SingleActionRecord action)
		{
			this.actionList.Add(action);
		}

	}

}
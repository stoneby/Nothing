using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.data.record
{

	/// <summary>
	/// 单个攻击者的动作，可能包含多个子动作
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleFightRecord : AbstractBaseRecord
	{

		/// <summary>
		/// 目标Index
		/// </summary>
		private int targetIndex;
		/// <summary>
		/// 目标阵营
		/// </summary>
		private int targetSide;
		/// <summary>
		/// 动作列表
		/// </summary>
		private List<SingleActionRecord> actionList;

		public virtual int TargetIndex
		{
			get
			{
				return targetIndex;
			}
			set
			{
				this.targetIndex = value;
			}
		}


		public virtual int TargetSide
		{
			get
			{
				return targetSide;
			}
			set
			{
				this.targetSide = value;
			}
		}


		public virtual List<SingleActionRecord> ActionList
		{
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
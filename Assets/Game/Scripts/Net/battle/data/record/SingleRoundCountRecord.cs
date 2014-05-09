using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data.record
{

	/// <summary>
	/// 单个回合计数
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class SingleRoundCountRecord : AbstractBaseRecord
	{

		/// <summary>
		/// 阵营
		/// </summary>
		private int side;
		/// <summary>
		/// 位置
		/// </summary>
		private int index;
		/// <summary>
		/// 剩余回合
		/// </summary>
		private int leftRound;
		/// <summary>
		/// 回合类型(buff、技�?
		/// </summary>
		private int roundCountType;
		/// <summary>
		/// 动作列表
		/// </summary>
		private List<SingleActionRecord> actionList;

		public virtual int Side
		{
			get
			{
				return side;
			}
			set
			{
				this.side = value;
			}
		}


		public virtual int Index
		{
			get
			{
				return index;
			}
			set
			{
				this.index = value;
			}
		}


		public virtual int LeftRound
		{
			get
			{
				return leftRound;
			}
			set
			{
				this.leftRound = value;
			}
		}


		public virtual int RoundCountType
		{
			get
			{
				return roundCountType;
			}
			set
			{
				this.roundCountType = value;
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
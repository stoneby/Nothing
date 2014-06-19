using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data.record
{

	/// <summary>
	/// �����غϼ���
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class SingleRoundCountRecord : AbstractBaseRecord
	{

		/// <summary>
		/// ��Ӫ
		/// </summary>
		private int side;
		/// <summary>
		/// λ��
		/// </summary>
		private int index;
		/// <summary>
		/// ʣ��غ�
		/// </summary>
		private int leftRound;
		/// <summary>
		/// �غ�����(buff������)
		/// </summary>
		private int roundCountType;
		/// <summary>
		/// �����б�
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
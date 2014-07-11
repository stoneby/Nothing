using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data.record
{

	/// <summary>
	/// ���������ߵĶ��������ܰ�������Ӷ���<br>
	/// ��ʱ��֧�ֹ����ߵĹ���ǰ����
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleFightRecord : AbstractBaseRecord
	{

		/// <summary>
		/// Ŀ��Index </summary>
		private int sourceIndex;

		/// <summary>
		/// Ŀ����Ӫ </summary>
		private int sourceSide;

		/// <summary>
		/// �������� </summary>
		private SingleActionRecord attackAction;

		/// <summary>
		/// �����б� </summary>
		private List<SingleActionRecord> actionList;

		/// <summary>
		/// �߼���curAction </summary>
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

		public virtual bool Empty
		{
			get
			{
				return attackAction == null && actionList.Count == 0;
			}
		}

	}

}
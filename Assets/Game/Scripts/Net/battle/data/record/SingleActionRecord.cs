using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data.record
{

	/// <summary>
	/// ������������һ���ุܶ�������õ���
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class SingleActionRecord : AbstractBaseRecord
	{

		/// <summary>
		/// �������ͣ�0������1�ظ���2����, 3SP����, 4תɫ </summary>
		private int actType;

		/// <summary>
		/// ��ӪIndex </summary>
		private int sideIndex;

		/// <summary>
		/// ����λ�ã���λ��ɣ�ʮλ��sideID����λ�Ƕ�����Index </summary>
		private int index;

		/// <summary>
		/// ״̬����
		/// </summary>
		private List<FighterStateRecord> stateUpdateList;

		public SingleActionRecord()
		{
			stateUpdateList = new List<FighterStateRecord>();
		}

		public virtual int ActType
		{
			get
			{
				return actType;
			}
			set
			{
				this.actType = value;
			}
		}


		public virtual int SideIndex
		{
			set
			{
				this.sideIndex = value;
			}
			get
			{
				return sideIndex;
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


		public virtual int ResultHp
		{
			get
			{
				return getIntProp(BattleRecordConstants.SINGLE_ACTION_PROP_HP);
			}
			set
			{
				addProp(BattleRecordConstants.SINGLE_ACTION_PROP_HP, value);
			}
		}


		public virtual int TeamIndex
		{
			get
			{
				return index % 10;
			}
		}

		public virtual int TargetIndexInTeam
		{
			get
			{
				return index / 10;
			}
		}

		public virtual void addState(sbyte key, int round)
		{
			FighterStateRecord _state = new FighterStateRecord();
			_state.State = key;
			_state.LeftRound = (sbyte)round;
			stateUpdateList.Add(_state);
		}

		public virtual List<FighterStateRecord> StateUpdateList
		{
			get
			{
				return stateUpdateList;
			}
		}


	    public override string ToString()
	    {
	        return
	            string.Format("Index: " + index + ", Act type: " + actType + ", side index: " + sideIndex);
	    }
	}

}
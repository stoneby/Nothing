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

		private SingleFighterRecord fighterInfo;

		public SingleActionRecord()
		{
			fighterInfo = new SingleFighterRecord();
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
				this.fighterInfo.Side = value;
			}
			get
			{
				return fighterInfo.Side;
			}
		}


		public virtual int StateFlag
		{
			set
			{
				this.fighterInfo.StateFlag = value;
			}
			get
			{
				return fighterInfo.StateFlag;
			}
		}



		public virtual int Index
		{
			get
			{
				return fighterInfo.Index;
			}
			set
			{
				this.fighterInfo.Index = value;
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



		public virtual void addState(int buffId, int showId, sbyte state, int round)
		{
			fighterInfo.addState(buffId, showId, state, round);
		}

		public virtual void clearState()
		{
			this.fighterInfo.clearState();
		}

		public virtual List<FighterStateRecord> StateUpdateList
		{
			get
			{
				return fighterInfo.StateUpdateList;
			}
		}

		public virtual SingleFighterRecord FighterInfo
		{
			get
			{
				return fighterInfo;
			}
		}

	}

}
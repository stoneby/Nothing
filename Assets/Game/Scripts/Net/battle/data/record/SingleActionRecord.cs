using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data.record
{

	/// <summary>
	/// 单个动作，是一个很多父动作公用的类
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class SingleActionRecord : AbstractBaseRecord
	{

		/// <summary>
		/// 动作类型：0攻击，1回复，2防御, 3SP攻击, 4转色 </summary>
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



	//	public void addState (int buffId, int showId, byte state, int round, Map<Integer, Integer> paramMap) {
	//		fighterInfo.addState(buffId, showId, state, round, paramMap);
	//	}

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
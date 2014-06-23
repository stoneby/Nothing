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

		/// <summary>
		/// 阵营Index </summary>
		private int sideIndex;

		/// <summary>
		/// 动作位置，两位组成，十位是sideID，个位是队伍内Index </summary>
		private int index;

		/// <summary>
		/// 状态更新
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
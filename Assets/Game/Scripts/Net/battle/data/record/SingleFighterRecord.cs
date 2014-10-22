using System.Collections.Generic;
using System.Text;

namespace com.kx.sglm.gs.battle.share.data.record
{


	public class SingleFighterRecord : AbstractBaseRecord
	{

		private int index;
		private int side;
		private int stateFlag;
		/// <summary>
		/// ×´Ì¬¸üÐÂ
		/// </summary>
		protected internal List<FighterStateRecord> stateUpdateList;

		public SingleFighterRecord()
		{
			stateUpdateList = new List<FighterStateRecord>();
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


		public virtual int StateFlag
		{
			get
			{
				return stateFlag;
			}
			set
			{
				this.stateFlag = value;
			}
		}


		public virtual string toRecordString()
		{
			StringBuilder _sb = new StringBuilder();
			_sb.Append(index).Append(" ");
			_sb.Append(side).Append(" ");
			_sb.Append(stateFlag).Append(" ;");
			foreach (FighterStateRecord _stateRecord in stateUpdateList)
			{
				_sb.Append(_stateRecord.toRecordString());
				_sb.Append(" ;");
			}
			return _sb.ToString();
		}


		public virtual void addState(int buffId, int showId, sbyte state, int round, Dictionary<int, int> paramMap)
		{
			FighterStateRecord _state = new FighterStateRecord();
			_state.State = state;
			_state.BuffId = buffId;
			_state.ShowId = showId;
			_state.LeftRound = (sbyte)round;
			_state.ParamMap = paramMap;
			stateUpdateList.Add(_state);
		}

		public virtual void clearState()
		{
			this.stateUpdateList.Clear();
		}

		public virtual List<FighterStateRecord> StateUpdateList
		{
			get
			{
				return stateUpdateList;
			}
		}



	}

}
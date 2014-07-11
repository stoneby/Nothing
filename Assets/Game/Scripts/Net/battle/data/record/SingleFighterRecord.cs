using System.Collections.Generic;

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



		public virtual void addState(int buffId, int showId, sbyte state, int round)
		{
			FighterStateRecord _state = new FighterStateRecord();
			_state.State = state;
			_state.BuffId = buffId;
			_state.ShowId = showId;
			_state.LeftRound = (sbyte)round;
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
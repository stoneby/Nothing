using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.actor.impl
{


	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;

	public class BattleFighterState
	{

		private int buffId;
		private FighterStateEnum state;
		private int showId;
		private int round;
		private Dictionary<int, int> paramMap;

		public BattleFighterState(int buffId, FighterStateEnum state, int showId, int round)
		{
			this.state = state;
			this.buffId = buffId;
			this.showId = showId;
			this.round = round;
			this.paramMap = new Dictionary<int, int>();
		}

		public virtual int BuffId
		{
			get
			{
				return buffId;
			}
		}

		public virtual int Round
		{
			get
			{
				return round;
			}
		}

		public virtual FighterStateEnum State
		{
			get
			{
				return state;
			}
		}

		public virtual int ShowId
		{
			get
			{
				return showId;
			}
		}

		public virtual sbyte Index
		{
			get
			{
				return (sbyte)State.Index;
			}
		}

		public virtual int getParam(int key, int defaultValue)
		{
			int _param = defaultValue;
			if (paramMap.ContainsKey(key))
			{
				_param = paramMap[key];
			}
			return _param;
		}

		public virtual Dictionary<int, int> ParamMap
		{
			get
			{
				return paramMap;
			}
			set
			{
				this.paramMap = new Dictionary<int, int>(value);
			}
		}


	}

}
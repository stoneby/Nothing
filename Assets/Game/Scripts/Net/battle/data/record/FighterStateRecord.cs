using System.Collections.Generic;
using System.Text;

namespace com.kx.sglm.gs.battle.share.data.record
{


	/// <summary>
	/// 武将单个状态<br>
	/// 若<seealso cref="#leftRound"/>为0，则直接移除这个状态
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class FighterStateRecord
	{

		private int buffId;
		private sbyte state;
		private int showId;
		private sbyte leftRound;
		private Dictionary<int, int> paramMap;

		public virtual int BuffId
		{
			get
			{
				return buffId;
			}
			set
			{
				this.buffId = value;
			}
		}


		public virtual int ShowId
		{
			get
			{
				return showId;
			}
			set
			{
				this.showId = value;
			}
		}


		public virtual sbyte State
		{
			get
			{
				return state;
			}
			set
			{
				this.state = value;
			}
		}


		public virtual sbyte LeftRound
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


		public virtual Dictionary<int, int> ParamMap
		{
			set
			{
				this.paramMap = new Dictionary<int, int>(value);
			}
		}

		//TODO: 这里在三个类重复了3遍，以后要优化
		public virtual int getParam(int key, int defaultValue)
		{
			int _param = defaultValue;
			if (paramMap.ContainsKey(key))
			{
				_param = paramMap[key];
			}
			return _param;
		}

		public virtual string toRecordString()
		{
			StringBuilder _sb = new StringBuilder();
			_sb.Append(buffId).Append(",");
			_sb.Append(state).Append(",");
			_sb.Append(leftRound).Append(",");
			return _sb.ToString();

		}

	}

}
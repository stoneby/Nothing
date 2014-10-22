using System.Collections.Generic;
using System.Text;

namespace com.kx.sglm.gs.battle.share.data.record
{


	/// <summary>
	/// �佫����״̬<br>
	/// ��<seealso cref="#leftRound"/>Ϊ0����ֱ���Ƴ����״̬
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

		//TODO: �������������ظ���3�飬�Ժ�Ҫ�Ż�
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
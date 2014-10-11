using System;
using System.Collections.Generic;
using System.Text;

namespace com.kx.sglm.gs.battle.share.data.store
{


	/// <summary>
	/// Êý¾ÝMap
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleStoreMap
	{

		private Dictionary<int, string> dataMap;

		public BattleStoreMap()
		{
			dataMap = new Dictionary<int, string>();
		}

		public virtual bool Empty
		{
			get
			{
				return dataMap.Count == 0;
			}
		}

		protected internal virtual void addValue(int key, string value)
		{
			dataMap[key] = value;
		}

		protected internal virtual void addIntValue(int key, int value)
		{
			addValue(key, Convert.ToString(value));
		}

		protected internal virtual void addLongValue(int key, long value)
		{
			addValue(key, Convert.ToString(value));
		}

		public virtual int getIntValue(int key)
		{
			return Convert.ToInt32(getValue(key, "0"));
		}

		public virtual long getLongValue(int key)
		{
			return Convert.ToInt64(getValue(key, "0"));
		}

		public virtual string getValue(int key, string defaultValue)
		{
			return dataMap.ContainsKey(key) ? dataMap[key] : defaultValue;
		}

		public virtual string toStoreStr()
		{
			StringBuilder _sb = new StringBuilder();
			foreach (KeyValuePair<int, string> _entry in dataMap)
			{
				_sb.Append(_entry.Key).Append(",");
				_sb.Append(_entry.Value).Append(";");
			}
			return _sb.ToString();
		}

		public virtual void fromStoreStr(string value)
		{
			if (value == null || value.Length == 0)
			{
				return;
			}
			string[] _values = value.Split(BattleStoreConstants.BATTLE_STORE_DATA_SPLIT, true);
			if (_values.Length == 0)
			{
				return;
			}
			foreach (string _kv in _values)
			{
				string[] _kvPair = _kv.Split(BattleStoreConstants.BATTLE_STORE_KEY_SPLIT, true);
				if (_kvPair.Length != 2)
				{
					//TODO: loggers.error
					continue;
				}
				dataMap[Convert.ToInt32(_kvPair[0])] = _kvPair[1];
			}
		}

	}

}
using System;
using System.Collections.Generic;
using System.Text;

namespace com.kx.sglm.gs.battle.share.data.store
{


	using IBattleCompatibleUtils = com.kx.sglm.gs.battle.share.utils.IBattleCompatibleUtils;

	/// <summary>
	/// Êý¾ÝMap
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleStoreMap
	{

		private Dictionary<int, string> dataMap;
		private IBattleCompatibleUtils compatibleUtils;

		public BattleStoreMap(IBattleCompatibleUtils compatibleUtils)
		{
			dataMap = new Dictionary<int, string>();
			this.compatibleUtils = compatibleUtils;
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
				_sb.Append(_entry.Key).Append(BattleStoreConstants.BATTLE_STORE_KEY_SPLIT);
				_sb.Append(_entry.Value).Append(BattleStoreConstants.BATTLE_STORE_DATA_SPLIT);
			}
			return _sb.ToString();
		}

		public virtual void fromStoreStr(string value)
		{
			if (value == null || value.Length == 0)
			{
				return;
			}
			string[] _values = compatibleUtils.splitString(value, BattleStoreConstants.BATTLE_STORE_DATA_SPLIT);
			if (_values.Length == 0)
			{
				return;
			}
			foreach (string _kv in _values)
			{
				string[] _kvPair = compatibleUtils.splitString(_kv, BattleStoreConstants.BATTLE_STORE_KEY_SPLIT);
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
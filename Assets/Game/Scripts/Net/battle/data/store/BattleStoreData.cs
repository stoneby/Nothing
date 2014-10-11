using System;
using System.Collections.Generic;
using System.Text;

namespace com.kx.sglm.gs.battle.share.data.store
{

	using MathUtils = com.kx.sglm.core.util.MathUtils;

	/// <summary>
	/// 战斗内存储
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleStoreData
	{

		private BattleStoreMap[] datas;

		public BattleStoreData()
		{
			datas = new BattleStoreMap[BattleStoreConstants.BATTLE_STORE_DATA_SIZE];
			for (int _i = 0; _i < BattleStoreConstants.BATTLE_STORE_DATA_SIZE; _i++)
			{
				datas[_i] = new BattleStoreMap();
			}
		}

		public virtual bool Empty
		{
			get
			{
				bool _empty = true;
				for (int _i = 0; _i < BattleStoreConstants.BATTLE_STORE_DATA_SIZE; _i++)
				{
					if (!datas[_i].Empty)
					{
						_empty = false;
						break;
					}
				}
				return _empty;
			}
		}

		public virtual int CurMaxFight
		{
			get
			{
				return getIntValue(BattleStoreConstants.BATTLE_RESULT_STORE_DATA, BattleStoreConstants.BATTLE_STORE_MAX_FIGHT);
			}
		}

		public virtual int CurHeroMp
		{
			get
			{
				return getIntValue(BattleStoreConstants.BATTLE_RESULT_STORE_DATA, BattleStoreConstants.BATTLE_STORE_CUR_HERO_MP);
			}
			set
			{
				addIntValue(BattleStoreConstants.BATTLE_PROCESS_STORE_DATA, BattleStoreConstants.BATTLE_STORE_CUR_HERO_MP, value);
			}
		}

		public virtual int CurSceneIndex
		{
			get
			{
				return getIntValue(BattleStoreConstants.BATTLE_PROCESS_STORE_DATA, BattleStoreConstants.BATTLE_STORE_CUR_SCENE_INDEX);
			}
			set
			{
				addIntValue(BattleStoreConstants.BATTLE_PROCESS_STORE_DATA, BattleStoreConstants.BATTLE_STORE_CUR_SCENE_INDEX, value);
			}
		}

		public virtual int CurHeroHp
		{
			get
			{
				return getIntValue(BattleStoreConstants.BATTLE_PROCESS_STORE_DATA, BattleStoreConstants.BATTLE_STORE_CUR_HERO_HP);
			}
			set
			{
				addIntValue(BattleStoreConstants.BATTLE_PROCESS_STORE_DATA, BattleStoreConstants.BATTLE_STORE_CUR_HERO_HP, value);
			}
		}

		public virtual int CurMax
		{
			set
			{
				addIntValue(BattleStoreConstants.BATTLE_RESULT_STORE_DATA, BattleStoreConstants.BATTLE_STORE_MAX_FIGHT, value);
			}
		}




		protected internal virtual void addValue(int type, int key, string value)
		{
			if (type >= datas.Length)
			{
				return;
			}
			datas[type].addValue(key, value);
		}

		protected internal virtual void addIntValue(int type, int key, int value)
		{
			addValue(type, key, Convert.ToString(value));
		}

		protected internal virtual void addLongValue(int type, int key, long value)
		{
			addValue(type, key, Convert.ToString(value));
		}

		public virtual int getIntValue(int type, int key)
		{
			return Convert.ToInt32(getValue(type, key, "0"));
		}

		public virtual long getLongValue(int type, int key)
		{
			return Convert.ToInt64(getValue(type, key, "0"));
		}

		public virtual string getValue(int type, int key, string defaultValue)
		{
			BattleStoreMap _dataMap = getStoreMap(type);
			if (_dataMap == null)
			{
				return defaultValue;
			}
			return _dataMap.getValue(key, defaultValue);
		}

		public virtual void saveSpString(List<int> spIndexList)
		{
			long _value = 0;
			foreach (int _index in spIndexList)
			{
				_value = MathUtils.optionOrFlagLong(_value, _index);
			}
			addLongValue(BattleStoreConstants.BATTLE_PROCESS_STORE_DATA, BattleStoreConstants.BATTLE_STORE_CUR_SP_INDEX_LIST, _value);
		}

		public virtual List<int> loadSpIndexList()
		{
			BattleStoreMap _map = getStoreMap(BattleStoreConstants.BATTLE_PROCESS_STORE_DATA);
			long _baseValue = _map.getLongValue(BattleStoreConstants.BATTLE_STORE_CUR_SP_INDEX_LIST);
			List<int> _indexList = MathUtils.getFlagIndexFromLong(_baseValue);
			return _indexList;
		}

		public virtual string toStoreStr()
		{
			StringBuilder _sb = new StringBuilder();
			for (int _i = 0; _i < BattleStoreConstants.BATTLE_STORE_DATA_SIZE; _i++)
			{
				_sb.Append(datas[_i].toStoreStr());
				// 最后一个后面没有
				if (_i < BattleStoreConstants.BATTLE_STORE_DATA_SIZE - 1)
				{
					_sb.Append(BattleStoreConstants.BATTLE_STORE_TYPE_SPLIT);
				}
			}
			return _sb.ToString();
		}

		internal virtual BattleStoreMap getStoreMap(int type)
		{
			BattleStoreMap _map = null;
			if (datas.Length > type)
			{
				_map = datas[type];
			}
			return _map;
		}

		public virtual string toStoreStr(int type)
		{
			BattleStoreMap _dataMap = getStoreMap(type);
			StringBuilder _sb = new StringBuilder();
			if (_dataMap != null)
			{
				_sb.Append(_dataMap.toStoreStr());
			}
			return _sb.ToString();
		}

		public virtual void fromStoreStr(string value)
		{
			string[] _datas = value.Split(BattleStoreConstants.BATTLE_STORE_TYPE_SPLIT, true);
			datas = new BattleStoreMap[_datas.Length];
			for (int _i = 0; _i < _datas.Length; _i++)
			{
				BattleStoreMap _mapData = new BattleStoreMap();
				_mapData.fromStoreStr(_datas[_i]);
				datas[_i] = _mapData;
			}
		}

	}

}
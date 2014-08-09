using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data
{


	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using BattleType = com.kx.sglm.gs.battle.share.enums.BattleType;
	using FighterType = com.kx.sglm.gs.battle.share.enums.FighterType;
	using PropertyRawSet = com.kx.sglm.gs.battle.share.utils.PropertyRawSet;

	/// <summary>
	/// ���Թ���һ������ս����Ԫ�أ����� 1.��ͻ���ͨѶ������ 2.��������ϴ洢ս����Ϣ<br>
	/// ս�������������ݲ��ᱻ�޸�
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleSource
	{

		/// <summary>
		/// ս����UUID </summary>
		protected internal long uuid;

		/// <summary>
		/// ȫ����Fighter������������м�����Ӫ��ͨ��<seealso cref="FighterInfo #battleSide"/>������Ӫ </summary>

		protected internal List<FighterInfo> fighterProp;

		/// <summary>
		/// ս������ </summary>
		protected internal BattleType battleType;

		/// <summary>
		/// ս���������� </summary>
		protected internal PropertyRawSet props;

		/// <summary>
		/// ս����ʼʱ�� </summary>
		protected internal long startTime;

		protected internal int spMaxBuffId;

		protected internal int raidStageId;

		protected internal List<int> monsterGroup;


		public BattleSource(BattleType battleType)
		{
			this.battleType = battleType;
			this.fighterProp = new List<FighterInfo>();
			this.props = new PropertyRawSet();
		}

		public virtual long Uuid
		{
			set
			{
				this.uuid = value;
			}
			get
			{
				return uuid;
			}
		}


		public virtual BattleType BattleType
		{
			get
			{
				return battleType;
			}
		}

		public virtual object getValue(int key)
		{
			return props.get(key);
		}

		public virtual string getStringValue(int key)
		{
			return props.getString(key, "");
		}

		public virtual int getIntValue(int key)
		{
			return props.getInt(key, 0);
		}

		public virtual int RaidStageId
		{
			get
			{
				return raidStageId;
			}
			set
			{
				this.raidStageId = value;
			}
		}


		public virtual List<FighterInfo> getTypeFighter(FighterType fighterType)
		{
			List<FighterInfo> _typeFighters = new List<FighterInfo>();
			foreach (FighterInfo _prop in FighterProp)
			{
				if (_prop.fighterType == fighterType)
				{
					_typeFighters.Add(_prop);
				}
			}
			return _typeFighters;
		}

		public virtual List<FighterInfo> getSideFighters(BattleSideEnum battelSide)
		{
			List<FighterInfo> _sideFighter = new List<FighterInfo>();
			foreach (FighterInfo _prop in FighterProp)
			{
				if (_prop.BattleSide == battelSide)
				{
					_sideFighter.Add(_prop);
				}
			}
			return _sideFighter;
		}

		public virtual List<FighterInfo> FighterProp
		{
			get
			{
				return fighterProp;
			}
			set
			{
				this.fighterProp = value;
			}
		}

		public virtual List<int> getFighterPropList(int key, bool notZero)
		{
			List<int> _propList = new List<int>();
			foreach (FighterInfo _info in FighterProp)
			{
				int _value = _info.getIntProp(key);
				if (notZero && _value == 0)
				{
					continue;
				}
				_propList.Add(_value);
			}
			return _propList;
		}


		public virtual void addFighterProp(List<FighterInfo> fighterList)
		{
			this.fighterProp.AddRange(fighterList);
		}

		public virtual void addProp(int key, object value)
		{
			this.props.set(key, value);
		}

		public virtual int SpMaxBuffId
		{
			get
			{
				return spMaxBuffId;
			}
			set
			{
				this.spMaxBuffId = value;
			}
		}


		public virtual List<int> MonsterGroup
		{
			get
			{
				return monsterGroup;
			}
			set
			{
				this.monsterGroup = value;
			}
		}




	}

}
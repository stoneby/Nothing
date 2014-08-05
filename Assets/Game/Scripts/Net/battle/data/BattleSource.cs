using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data
{


	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using BattleType = com.kx.sglm.gs.battle.share.enums.BattleType;
	using PropertyRawSet = com.kx.sglm.gs.battle.share.utils.PropertyRawSet;
	using BattleBuffMsgData = KXSGCodec.BattleBuffMsgData;
	using BattleHeroSkillMsgAction = KXSGCodec.BattleHeroSkillMsgAction;
	using BattleMonsterAIMsgAction = KXSGCodec.BattleMonsterAIMsgAction;
	using BattleMonsterSkillMsgAction = KXSGCodec.BattleMonsterSkillMsgAction;

	/// <summary>
	/// 可以构建一个完整战斗的元素，用于 1.与客户端通讯构建， 2.在玩家身上存储战斗信息<br>
	/// 战斗中这个类的内容不会被修改
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleSource
	{

		/// <summary>
		/// 战斗的UUID </summary>
		protected internal long uuid;

		/// <summary>
		/// 全部的Fighter都在这里，不管有几个阵营，通过<seealso cref="FighterInfo #battleSide"/>区分阵营 </summary>
		protected internal List<FighterInfo> fighterProp;

		/// <summary>
		/// 战斗类型 </summary>
		protected internal BattleType battleType;

		/// <summary>
		/// 战斗其他属性 </summary>
		protected internal PropertyRawSet props;

		/// <summary>
		/// 战斗开始时间 </summary>
		protected internal long startTime;

		protected internal int spMaxBuffId;

		/// <summary>
		/// 英雄技能列表 </summary>
		protected internal List<BattleHeroSkillMsgAction> heroSkillList;

		/// <summary>
		/// 怪物技能列表 </summary>
		protected internal List<BattleMonsterSkillMsgAction> monsterSkillList;

		/// <summary>
		/// 怪物AI列表 </summary>
		protected internal List<BattleMonsterAIMsgAction> monsterAIList;

		protected internal List<BattleBuffMsgData> buffList;

		public BattleSource(BattleType battleType)
		{
			this.battleType = battleType;
			this.fighterProp = new List<FighterInfo>();
			this.props = new PropertyRawSet();
			this.heroSkillList = new List<BattleHeroSkillMsgAction>();
			this.monsterSkillList = new List<BattleMonsterSkillMsgAction>();
			this.monsterAIList = new List<BattleMonsterAIMsgAction>();
			this.buffList = new List<BattleBuffMsgData>();
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


		public virtual void addProp(int key, object value)
		{
			this.props.set(key, value);
		}

		public virtual List<BattleHeroSkillMsgAction> HeroSkillList
		{
			get
			{
				return heroSkillList;
			}
			set
			{
				this.heroSkillList = value;
			}
		}


		public virtual List<BattleMonsterSkillMsgAction> MonsterSkillList
		{
			get
			{
				return monsterSkillList;
			}
			set
			{
				this.monsterSkillList = value;
			}
		}


		public virtual List<BattleMonsterAIMsgAction> MonsterAIList
		{
			get
			{
				return monsterAIList;
			}
			set
			{
				this.monsterAIList = value;
			}
		}


		public virtual List<BattleBuffMsgData> BuffList
		{
			get
			{
				return buffList;
			}
			set
			{
				this.buffList = value;
			}
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






	}

}
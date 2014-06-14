using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data
{


	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using FighterType = com.kx.sglm.gs.battle.share.enums.FighterType;
	using FighterAProperty = com.kx.sglm.gs.battle.share.utils.FighterAProperty;
	using PropertyRawSet = com.kx.sglm.gs.battle.share.utils.PropertyRawSet;

	/// <summary>
	/// 战斗相关属�?
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class FighterInfo
	{

		protected internal int index;
		protected internal BattleSideEnum battleSide;
		protected internal FighterType fighterType;
		protected internal FighterAProperty battleProperty;
		protected internal PropertyRawSet properties;
		protected internal int activeSkillId;
		protected internal List<int> skillIdList;

		public FighterInfo()
		{
			properties = new PropertyRawSet();
			skillIdList = new List<int>();
		}

		public virtual void addSkillId(int skillId)
		{
			skillIdList.Add(skillId);
		}

		public virtual void addNormalProp(int key, object value)
		{
			properties.set(key, value);
		}

		public virtual string getProp(int key)
		{
			return properties.getString(key, "");
		}

		public virtual int getIntProp(int key)
		{
			return properties.getInt(key, 0);
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


		public virtual FighterType FighterType
		{
			set
			{
				this.fighterType = value;
			}
		}

		public virtual FighterAProperty BattleProperty
		{
			set
			{
				this.battleProperty = value;
			}
			get
			{
				return battleProperty;
			}
		}


		public virtual BattleSideEnum BattleSide
		{
			get
			{
				return battleSide;
			}
			set
			{
				this.battleSide = value;
			}
		}


		public virtual List<int> SkillIdList
		{
			get
			{
				return skillIdList;
			}
			set
			{
				this.skillIdList = value;
			}
		}

		public virtual int ActiveSkillId
		{
			get
			{
				return activeSkillId;
			}
			set
			{
				this.activeSkillId = value;
			}
		}




		public virtual int AiId
		{
			get
			{
				return properties.getInt(BattleKeyConstants.BATTLE_PROP_MONSTER_AI_ID, 0);
			}
		}

	}

}
namespace com.kx.sglm.gs.battle.data
{

	using BattleSideEnum = com.kx.sglm.gs.battle.enums.BattleSideEnum;
	using FighterType = com.kx.sglm.gs.battle.enums.FighterType;
	using FighterAProperty = com.kx.sglm.gs.battle.utils.FighterAProperty;
	using PropertyRawSet = com.kx.sglm.gs.battle.utils.PropertyRawSet;

	/// <summary>
	/// 战斗相关属性
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

		public FighterInfo()
		{
			properties = new PropertyRawSet();
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

		public virtual void addFighterProp(int key, object value)
		{
			properties.set(key, value);
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


	}

}
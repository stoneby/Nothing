using System.Collections.Generic;
using System.Text;

namespace com.kx.sglm.gs.battle.share.actor.impl
{


	using RoleAProperty = com.kx.sglm.gs.hero.properties.RoleAProperty;
	using AmendTriple = com.kx.sglm.gs.role.properties.amend.model.AmendTriple;

	/// <summary>
	/// property for battle fighter, base prop is final prop, buff prop is 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleFighterPropty : IFighterOwner
	{

		private RoleAProperty baseProp;
		private RoleAProperty buffProp;
		private RoleAProperty mutiProp;
		private RoleAProperty battleProp;
		private BattleFighter owner;




		public BattleFighterPropty(BattleFighter fighter)
		{
			this.owner = fighter;
			this.baseProp = new RoleAProperty();
			this.buffProp = new RoleAProperty();
			this.mutiProp = new RoleAProperty();
			this.battleProp = new RoleAProperty();
			initBaseProp();
		}

		protected internal virtual void initBaseProp()
		{
			Dictionary<int, int> battleProperties = Owner.BaseProp.BattleProperty;
			foreach (KeyValuePair<int, int> _prop in battleProperties)
			{
				baseProp.set(_prop.Key, (float)_prop.Value);
			}
			resetBuffProp();
		}


		public virtual void addBuffProp(AmendTriple amendTriple)
		{
			amendTriple.amend(baseProp, buffProp);
		}

		public virtual void addAttackProp(AmendTriple amendTriple)
		{
			RoleAProperty _baseProp = new RoleAProperty();
			amendTriple.amend(mutiProp, _baseProp);
			mutiProp.set(amendTriple.AmendIndex, _baseProp.get(amendTriple.AmendIndex));
		}

		public virtual void addBuffProp(int key, float value)
		{
			buffProp.add(key, value);
		}

		public virtual void resetBuffProp()
		{
			this.buffProp.clear();
			this.mutiProp.clear();
			this.mutiProp.add(RoleAProperty.DECRDAMAGE, BattleConstants.BATTLE_RATIO_BASE);
			this.mutiProp.add(RoleAProperty.INCRDAMAGE, BattleConstants.BATTLE_RATIO_BASE);
			//Logger.Log("clear all buff prop");
		}



		public virtual void recalcBattleProp()
		{
			battleProp.clear();
			battleProp.add(baseProp);
			battleProp.add(buffProp);
			//special property, set value directly
			for (int _i = 0; _i < BattleConstants.BATTLE_MUTI_PROP_ARR.Length; _i++)
			{
				int _index = BattleConstants.BATTLE_MUTI_PROP_ARR[_i];
				battleProp.set(_index, mutiProp.get(_index));
			}
			//Logger.Log(string.Format("#recalcBattleProp.cur prop = {0}", toPropStr(battleProp)));
		}

		protected internal virtual string toPropStr(RoleAProperty prop)
		{
			StringBuilder _sb = new StringBuilder();
			for (int _i = 0; _i < RoleAProperty._SIZE; _i++)
			{
				_sb.Append(_i).Append("=").Append(prop.get(_i)).Append(",");
			}
			return _sb.ToString();
		}

		public virtual RoleAProperty BattleProp
		{
			get
			{
				return battleProp;
			}
		}

		public virtual RoleAProperty BaseProp
		{
			get
			{
				return baseProp;
			}
		}

		public virtual BattleFighter Owner
		{
			get
			{
				return owner;
			}
		}






	}

}
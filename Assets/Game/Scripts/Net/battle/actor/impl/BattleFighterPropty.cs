using System.Collections.Generic;

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
			mutiProp.add(RoleAProperty.DECRDAMAGE, BattleConstants.BATTLE_RATIO_BASE);
			mutiProp.add(RoleAProperty.INCRDAMAGE, BattleConstants.BATTLE_RATIO_BASE);
		}

		public virtual void addBuffProp(AmendTriple amendTriple)
		{
			amendTriple.amend(baseProp, buffProp);
		}

		public virtual void addAttackProp(AmendTriple amendTriple)
		{
			RoleAProperty _prop = new RoleAProperty();
			_prop.copyFrom(mutiProp);
			amendTriple.amend(mutiProp, _prop);
			mutiProp.copyFrom(_prop);
		}

		public virtual void addBuffProp(int key, float value)
		{
			buffProp.add(key, value);
		}

		public virtual void resetBuffProp()
		{
			this.buffProp.clear();

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
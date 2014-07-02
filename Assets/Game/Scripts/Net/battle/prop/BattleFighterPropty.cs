using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.prop
{


	using IFighterOwner = com.kx.sglm.gs.battle.share.actor.IFighterOwner;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
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
		private RoleAProperty battleProp;
		private BattleFighter owner;





		public BattleFighterPropty(BattleFighter fighter)
		{
			this.owner = fighter;
			this.baseProp = new RoleAProperty();
			this.buffProp = new RoleAProperty();
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
		}

		public virtual void addBuffProp(AmendTriple amendTriple)
		{
			amendTriple.amend(baseProp, buffProp);
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
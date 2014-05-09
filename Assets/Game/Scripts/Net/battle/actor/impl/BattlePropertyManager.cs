namespace com.kx.sglm.gs.battle.share.actor.impl
{

	using FighterAProperty = com.kx.sglm.gs.battle.share.utils.FighterAProperty;

	public class BattlePropertyManager
	{


		private FighterAProperty baseProperty;

		private FighterAProperty fighterProperty;


		public virtual FighterAProperty BaseProperty
		{
			set
			{
				this.baseProperty = value;
			}
		}


		public virtual void resetFighterProp()
		{

		}


		public virtual float TotalHp
		{
			get
			{
				return fighterProperty.get(FighterAProperty.HP);
    
			}
		}


	}

}
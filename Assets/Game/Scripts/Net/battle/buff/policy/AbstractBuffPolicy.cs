namespace com.kx.sglm.gs.battle.share.buff.policy
{


	public abstract class AbstractBuffPolicy : IBuffAddPolicy
	{

		public virtual void optionBuff(BattleBuffManager manager, IBuffAction buffAction)
		{
			BattleFighterBuff _buff = optionBuffByType(manager, buffAction);
			manager.putBuff(_buff);
		}

		internal abstract BattleFighterBuff optionBuffByType(BattleBuffManager manager, IBuffAction buffAction);

	}

}
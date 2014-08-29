namespace com.kx.sglm.gs.battle.share.buff.policy
{


	public abstract class AbstractBuffPolicy : IBuffAddPolicy
	{

		public virtual void optionBuff(BattleBuffManager manager, BuffInfo info)
		{
			BattleFighterBuff _buff = optionBuffByType(manager, info);
			manager.putBuff(_buff);
		}

		internal abstract BattleFighterBuff optionBuffByType(BattleBuffManager manager, BuffInfo info);

	}

}
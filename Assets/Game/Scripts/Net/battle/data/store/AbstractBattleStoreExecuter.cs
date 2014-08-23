namespace com.kx.sglm.gs.battle.share.data.store
{

	public abstract class AbstractBattleStoreExecuter : IBattleStoreExecuter
	{
		public abstract void saveToData();

		protected internal BattleStoreData data;


		public AbstractBattleStoreExecuter(BattleStoreData data)
		{
			this.data = data;
		}

	}

}
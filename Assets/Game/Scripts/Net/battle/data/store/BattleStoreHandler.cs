namespace com.kx.sglm.gs.battle.share.data.store
{

	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;

	public class BattleStoreHandler
	{

		protected internal BattleStoreData data;
		protected internal AttackStoreExecuter attackExecuter;
		protected internal SceneEndExecuter sceneEndExecuter;

		public BattleStoreHandler(BattleStoreData data)
		{
			this.data = data;
			attackExecuter = new AttackStoreExecuter(data);
			sceneEndExecuter = new SceneEndExecuter(data);

		}

		public virtual void handleStartAttack(int totalCount)
		{
			attackExecuter.handleStartAttack(totalCount);
		}

		public virtual void handleAttack(int attackValue)
		{
			attackExecuter.handleAttackInfo(attackValue);
		}

		public virtual void handleSceneEnd(int sceneIndex, HeroTeam heroTeam)
		{
			sceneEndExecuter.handleSceneFinish(sceneIndex, heroTeam);
		}

		public virtual BattleStoreData Data
		{
			get
			{
				return data;
			}
		}

	}

}
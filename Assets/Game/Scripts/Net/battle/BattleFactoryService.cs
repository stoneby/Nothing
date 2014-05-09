namespace com.kx.sglm.gs.battle.share
{

	using TestBattleFactory = com.kx.sglm.gs.battle.share.factory.TestBattleFactory;


	public class BattleFactoryService
	{

		private TestBattleFactory testFactory;


		public BattleFactoryService()
		{
			testFactory = new TestBattleFactory();
		}


		public virtual TestBattleFactory TestFactory
		{
			get
			{
				return testFactory;
			}
		}
	}

}
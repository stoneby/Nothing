namespace com.kx.sglm.gs.battle
{

	using TestBattleFactory = com.kx.sglm.gs.battle.factory.TestBattleFactory;


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
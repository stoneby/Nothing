namespace com.kx.sglm.gs.battle.share.factory
{

	using IBattleExecuter = com.kx.sglm.gs.battle.share.executer.IBattleExecuter;
	using TestPVEBattleExecuter = com.kx.sglm.gs.battle.share.executer.impl.TestPVEBattleExecuter;
	using IBattleTemplateService = com.kx.sglm.gs.battle.share.factory.creater.IBattleTemplateService;

	public class TestBattleFactory : AbstractPVEBattleFactory
	{

		public override IBattleExecuter createBattleExecuter(Battle battle, IBattleTemplateService tempService)
		{
			return new TestPVEBattleExecuter(battle);
		}



	}

}
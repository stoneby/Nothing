namespace com.kx.sglm.gs.battle.share.factory
{

	using BattleSource = com.kx.sglm.gs.battle.share.data.BattleSource;
	using IBattleTemplateService = com.kx.sglm.gs.battle.share.factory.creater.IBattleTemplateService;

	public interface IBattleFactory
	{

		Battle createBattle(BattleSource source, IBattleTemplateService tempService);
	}

}
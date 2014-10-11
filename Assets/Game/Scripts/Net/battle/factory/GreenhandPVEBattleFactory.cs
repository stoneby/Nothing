namespace com.kx.sglm.gs.battle.share.factory
{

	using GreenhandColorCreater = com.kx.sglm.gs.battle.share.color.GreenhandColorCreater;
	using IBattleExecuter = com.kx.sglm.gs.battle.share.executer.IBattleExecuter;
	using GreenhandPVEBattleExecuter = com.kx.sglm.gs.battle.share.executer.impl.GreenhandPVEBattleExecuter;
	using IBattleTemplateService = com.kx.sglm.gs.battle.share.factory.creater.IBattleTemplateService;

	public class GreenhandPVEBattleFactory : AbstractPVEBattleFactory
	{

		public override IBattleExecuter createBattleExecuter(Battle battle, IBattleTemplateService tempService)
		{
			GreenhandColorCreater _colorCreater = new GreenhandColorCreater(battle);
			GreenhandPVEBattleExecuter _executer = new GreenhandPVEBattleExecuter(battle, _colorCreater);
			Template.Auto.Greenhand.GreenhandTemplate _temp = tempService.BattleGreenhandTemplate;
			_executer.initTemplInfo(_temp);
			return _executer;
		}

	}

}
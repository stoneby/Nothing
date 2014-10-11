namespace com.kx.sglm.gs.battle.share.factory
{

	using RaidColorCreater = com.kx.sglm.gs.battle.share.color.RaidColorCreater;
	using IBattleExecuter = com.kx.sglm.gs.battle.share.executer.IBattleExecuter;
	using RaidPVEBattleExecuter = com.kx.sglm.gs.battle.share.executer.impl.RaidPVEBattleExecuter;
	using IBattleTemplateService = com.kx.sglm.gs.battle.share.factory.creater.IBattleTemplateService;

	public class RaidPVEBattleFactory : AbstractPVEBattleFactory
	{

		public override IBattleExecuter createBattleExecuter(Battle battle, IBattleTemplateService tempService)
		{
			RaidColorCreater _creater = createRaidColorCreater(battle, tempService);
			return new RaidPVEBattleExecuter(battle, _creater);
		}

		internal virtual RaidColorCreater createRaidColorCreater(Battle battle, IBattleTemplateService tempService)
		{
			int _stageId = battle.BattleSource.RaidStageId;
			Template.Auto.Raid.RaidStageTemplate _temp = tempService.getRaidStageTemp(_stageId);
			RaidColorCreater _creater = new RaidColorCreater(battle);
			_creater.build(_temp.ColorRate);
			return _creater;
		}

	}

}
namespace com.kx.sglm.gs.battle.share.factory
{

	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;
	using BattleSource = com.kx.sglm.gs.battle.share.data.BattleSource;
	using BattleType = com.kx.sglm.gs.battle.share.enums.BattleType;
	using IBattleExecuter = com.kx.sglm.gs.battle.share.executer.IBattleExecuter;
	using IBattleTemplateService = com.kx.sglm.gs.battle.share.factory.creater.IBattleTemplateService;

	public abstract class AbstractBattleFactory : IBattleFactory
	{


		public virtual Battle createBattle(BattleSource source, IBattleTemplateService tempService)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.share.enums.BattleType _type = source.getBattleType();
			BattleType _type = source.BattleType;

			Battle _battle = new Battle(_type, source);

			IBattleExecuter _excuter = createBattleExecuter(_battle, tempService);

			_battle.BattleExcuter = _excuter;

			HeroTeam _attacker = createAttackerTeam(_battle);

			_excuter.AttackerTeam = _attacker;

			_excuter.initDefencerTeam();

			_excuter.initDataOnCreate();

			return _battle;
		}



		public abstract IBattleExecuter createBattleExecuter(Battle battle, IBattleTemplateService tempService);

		public abstract HeroTeam createAttackerTeam(Battle battle);




	}

}
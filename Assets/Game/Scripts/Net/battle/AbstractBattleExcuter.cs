using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.executer
{

	using HeroTeam = com.kx.sglm.gs.battle.actor.impl.HeroTeam;

	public abstract class AbstractBattleExcuter : IBattleExecuter
	{
		public abstract bool BattleDead {get;}
		public abstract IList<com.kx.sglm.gs.battle.enums.HeroColor> createColorList(int createCount);
		public abstract com.kx.sglm.gs.battle.enums.BattleType BattleType {get;}
		public abstract com.kx.sglm.gs.battle.logic.loop.BattleScene createNextBattleScene();
		public abstract bool hasNextBattleScene();
		public abstract void onBattleFinish();
		public abstract void onBattleRoundStart(com.kx.sglm.gs.battle.logic.loop.BattleRound battleRound, com.kx.sglm.gs.battle.actor.impl.BattleTeam attackTeam);
		public abstract bool needHungUp(com.kx.sglm.gs.battle.logic.loop.BattleRound round, com.kx.sglm.gs.battle.actor.impl.BattleTeam attackTeam);
		public abstract void onBattleSceneFinish(com.kx.sglm.gs.battle.logic.loop.BattleScene battleScene);
		public abstract void onBattleRoundFinish(com.kx.sglm.gs.battle.logic.loop.BattleRound battleRound);
		public abstract void onBattleTeamShotFinish(com.kx.sglm.gs.battle.logic.loop.BattleTeamShot battleTeamShot);
		public abstract void onBattleTeamShotStart(com.kx.sglm.gs.battle.logic.loop.BattleTeamShot battleTeamShot);
		public abstract void beforeBattleStart(com.kx.sglm.gs.battle.logic.loop.BattleScene battleScene, com.kx.sglm.gs.battle.data.record.BattleRoundCountRecord record);
		public abstract void initDataOnCreate();
		public abstract void initDefencerTeam();
		public abstract com.kx.sglm.gs.battle.actor.impl.BattleArmy createSceneBattleArmy();

		private Battle battle;

		private HeroTeam attackerTeam_Renamed;

		public AbstractBattleExcuter(Battle battle)
		{
			this.battle = battle;
		}


		public virtual HeroTeam AttackerTeam
		{
			set
			{
				this.attackerTeam_Renamed = value;
			}
		}


		public virtual HeroTeam attackerTeam()
		{
			return attackerTeam_Renamed;
		}


		public virtual Battle Battle
		{
			get
			{
				return battle;
			}
		}

	}

}
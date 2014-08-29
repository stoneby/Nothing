namespace com.kx.sglm.gs.battle.share.buff
{

	using IFighterOwner = com.kx.sglm.gs.battle.share.actor.IFighterOwner;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using TeamShotStartEvent = com.kx.sglm.gs.battle.share.@event.impl.TeamShotStartEvent;

	public interface IBattleBuffManager : IRoundCounter, IFighterOwner
	{

		void onAttack(BattleFightRecord fightRecord);

		void addBuff(BuffInfo buffInfo);

		void onRoundFinish(BattleRoundCountRecord roundRecord);

		void onTeamShotStart(TeamShotStartEvent @event);

		void activeAllBuff(int buffFlag);

		void clearAllBuff();

	}

}
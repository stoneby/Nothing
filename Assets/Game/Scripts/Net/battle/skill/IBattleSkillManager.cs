namespace com.kx.sglm.gs.battle.share.skill
{

	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using ISingletonBattleAction = com.kx.sglm.gs.battle.share.singleton.ISingletonBattleAction;

	public interface IBattleSkillManager : IRoundCounter
	{


		ISingletonBattleAction FightAction {get;}

		bool canAttack();

		void beforeAttack(BattleFightRecord record);

		void afterAttack(BattleFightRecord record);

	}

}
namespace com.kx.sglm.gs.battle.skill
{

	using BattleFightRecord = com.kx.sglm.gs.battle.data.record.BattleFightRecord;
	using AbstractSingletonAttackAction = com.kx.sglm.gs.battle.singleton.AbstractSingletonAttackAction;

	public interface IBattleSkillManager : IRoundCounter
	{


		AbstractSingletonAttackAction FightAction {get;}

		bool canAttack();

		void beforeAttack(BattleFightRecord record);

		void afterAttack(BattleFightRecord record);

	}

}
namespace com.kx.sglm.gs.battle.skill
{

	using BattleFighter = com.kx.sglm.gs.battle.actor.impl.BattleFighter;
	using BattleFightRecord = com.kx.sglm.gs.battle.data.record.BattleFightRecord;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.data.record.BattleRoundCountRecord;
	using AbstractSingletonAttackAction = com.kx.sglm.gs.battle.singleton.AbstractSingletonAttackAction;
	using NormalHeroAttackAction = com.kx.sglm.gs.battle.singleton.NormalHeroAttackAction;

	public class HeroSkillManager : AbstractSkillManager
	{

		private NormalHeroAttackAction heroAction;

		public HeroSkillManager(BattleFighter fighter) : base(fighter)
		{
			heroAction = new NormalHeroAttackAction();
			//TODO: 以后重构
		}

		public virtual void countDownRound()
		{
			// TODO no logic here

		}

		public override AbstractSingletonAttackAction FightAction
		{
			get
			{
				return heroAction;
			}
		}

		public override bool canAttack()
		{
			return true;
		}

		public override void beforeAttack(BattleFightRecord record)
		{
			// TODO Auto-generated method stub

		}

		public override void afterAttack(BattleFightRecord record)
		{
			// TODO Auto-generated method stub

		}

		public override void countDownRound(BattleRoundCountRecord roundRecord)
		{
			// TODO Auto-generated method stub

		}

		public override void beforeBattleStart(BattleRoundCountRecord roundRecord)
		{
			// TODO Auto-generated method stub

		}

	}

}
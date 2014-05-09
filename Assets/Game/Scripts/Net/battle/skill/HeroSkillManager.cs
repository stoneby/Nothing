namespace com.kx.sglm.gs.battle.share.skill
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;
	using ISingletonBattleAction = com.kx.sglm.gs.battle.share.singleton.ISingletonBattleAction;
	using NormalHeroAttackAction = com.kx.sglm.gs.battle.share.singleton.NormalHeroAttackAction;
	using NormalHeroRecoverAction = com.kx.sglm.gs.battle.share.singleton.NormalHeroRecoverAction;

	public class HeroSkillManager : AbstractSkillManager
	{

		private NormalHeroAttackAction heroAction;
		private NormalHeroRecoverAction recoverAction;
		private HeroTeam ownerTeam;

		public HeroSkillManager(BattleFighter fighter, HeroTeam ownerTeam) : base(fighter)
		{
			//TODO: 这里之后是单例的Action
			heroAction = new NormalHeroAttackAction();
			recoverAction = new NormalHeroRecoverAction();
			this.ownerTeam = ownerTeam;
			//TODO: 以后重构
		}

		public virtual void countDownRound()
		{
			// TODO no logic here

		}

		public override ISingletonBattleAction FightAction
		{
			get
			{
				HeroColor _color = ownerTeam.CurFightColor;
				ISingletonBattleAction _curAction = null;
				if (_color.Recover)
				{
					_curAction = recoverAction;
				}
				else
				{
					_curAction = heroAction;
				}
				return _curAction;
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
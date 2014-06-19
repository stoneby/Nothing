namespace com.kx.sglm.gs.battle.share.skill.manager
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;

	/// <summary>
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class AbstractBattleSkillManager : IBattleSkillManager
	{
		public abstract void onHandleEvent(com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord record);
		public abstract void onAttack(BattleFightRecord fightRecord);
		public abstract void onTeamShotStart(com.kx.sglm.gs.battle.share.@event.impl.TeamShotStartEvent @event);
		public abstract void onSceneStart(com.kx.sglm.gs.battle.share.@event.impl.SceneStartEvent @event);
		public abstract void afterAttack(BattleFightRecord record);
		public abstract void beforeAttack(BattleFightRecord record);


		private BattleFighter fighter;

		public AbstractBattleSkillManager(BattleFighter fighter)
		{
			this.fighter = fighter;
		}

		public abstract void init();

		/* java to c#语法需要
		 * @see com.kx.sglm.gs.battle.skill.IBattleSkillManager#canAttack()
		 */
		public virtual bool canAttack()
		{
			return false;
		}

		/* java to c#语法需要
		 * @see com.kx.sglm.gs.battle.skill.IBattleSkillManager#getFightAction()
		 */
		public virtual ISingletonSkillAction AttackAction
		{
			get
			{
				return null;
			}
		}


		/* java to c#语法需要
		 * @see com.kx.sglm.gs.battle.IRoundCounter#countDownRound(com.kx.sglm.gs.battle.data.record.BattleRoundCountRecord)
		 */
		public virtual void countDownRound(BattleRoundCountRecord roundRecord)
		{

		}

		/* java to c#语法需要
		 * @see com.kx.sglm.gs.battle.IRoundCounter#beforeBattleStart(com.kx.sglm.gs.battle.data.record.BattleRoundCountRecord)
		 */
		public virtual void beforeBattleStart(BattleRoundCountRecord roundRecord)
		{

		}

		protected internal virtual void action(ISingletonSkillAction skillAction, BattleFightRecord record)
		{
			skillAction.onAction(Fighter, record);
		}

		/* java to c#语法需要
		 * @see com.kx.sglm.gs.battle.share.skill.IBattleSkillManager#onActiveOption()
		 */
		public virtual void onActiveOption()
		{
			// TODO Auto-generated method stub

		}


		public virtual BattleFighter Fighter
		{
			get
			{
				return fighter;
			}
		}


		/* 没有Override——java to c#语法需要
		 * @see com.kx.sglm.gs.battle.IBattle#getBattle()
		 */
		public virtual Battle Battle
		{
			get
			{
				return fighter.Battle;
			}
		}

		public virtual BattleSkillActionService SkillService
		{
			get
			{
				return BattleSkillActionService.Service;
			}
		}


	}

}
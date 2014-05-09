namespace com.kx.sglm.gs.battle.share.skill
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using ISingletonBattleAction = com.kx.sglm.gs.battle.share.singleton.ISingletonBattleAction;

	/// <summary>
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class AbstractSkillManager : IBattleSkillManager
	{
		public abstract void afterAttack(com.kx.sglm.gs.battle.share.data.record.BattleFightRecord record);
		public abstract void beforeAttack(com.kx.sglm.gs.battle.share.data.record.BattleFightRecord record);


		private BattleFighter fighter;

		public AbstractSkillManager(BattleFighter fighter)
		{
			this.fighter = fighter;
		}

		/* java to c#语法需�?
		 * @see com.kx.sglm.gs.battle.skill.IBattleSkillManager#canAttack()
		 */
		public virtual bool canAttack()
		{
			return false;
		}

		/* java to c#语法需�?
		 * @see com.kx.sglm.gs.battle.skill.IBattleSkillManager#getFightAction()
		 */
		public virtual ISingletonBattleAction FightAction
		{
			get
			{
				return null;
			}
		}


		/* java to c#语法需�?
		 * @see com.kx.sglm.gs.battle.IRoundCounter#countDownRound(com.kx.sglm.gs.battle.data.record.BattleRoundCountRecord)
		 */
		public virtual void countDownRound(BattleRoundCountRecord roundRecord)
		{

		}

		/* java to c#语法需�?
		 * @see com.kx.sglm.gs.battle.IRoundCounter#beforeBattleStart(com.kx.sglm.gs.battle.data.record.BattleRoundCountRecord)
		 */
		public virtual void beforeBattleStart(BattleRoundCountRecord roundRecord)
		{

		}

		public virtual BattleFighter Fighter
		{
			get
			{
				return fighter;
			}
		}


		/* 没有Override——java to c#语法需�?
		 * @see com.kx.sglm.gs.battle.IBattle#getBattle()
		 */
		public virtual Battle Battle
		{
			get
			{
				return fighter.Battle;
			}
		}


	}

}
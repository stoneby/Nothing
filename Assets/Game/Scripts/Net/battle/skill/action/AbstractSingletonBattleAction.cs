using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.action
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	/// <summary>
	/// 技能的顶层抽象�?
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class AbstractSingletonBattleAction : ISingletonSkillAction
	{
		public abstract bool canOption(BattleFighter attacker);

		/// <summary>
		/// 技能ID </summary>
		private int skillId;

		/// <summary>
		/// 目标获取器，根据Index不同分为对敌还是对己 </summary>
		private TargetGetterHolder[] targetGetterHolderArr;

		/// <summary>
		/// 技能动�? </summary>
		private List<ISkillEffect> allEffect;

		public AbstractSingletonBattleAction()
		{
			allEffect = new List<ISkillEffect>();
			initTargetGetter();
		}

		public virtual void initTargetGetter()
		{
			targetGetterHolderArr = new TargetGetterHolder[BattleSideEnum.size()];
			targetGetterHolderArr[BattleConstants.TARGET_GETTER_FOR_ENEMY_INDEX] = new TargetGetterHolder(true);
			targetGetterHolderArr[BattleConstants.TARGET_GETTER_FOR_FRIEND_INDEX] = new TargetGetterHolder(false);
		}

		public virtual void onAction(BattleFighter attacker, BattleFightRecord record)
		{
			// TODO: 加入后续逻辑
			optionAction(attacker, record);
		}

		internal virtual void optionAction(BattleFighter attacker, BattleFightRecord record)
		{
			// 创建一个供不同动作之间数据沟通的记录
			SkillDataHolder _holder = createDataHolder(record);
			// 动作分两�?
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<com.kx.sglm.gs.battle.share.actor.impl.BattleFighter> _enemyFighterList = calcTargetList(attacker, true);
			List<BattleFighter> _enemyFighterList = calcTargetList(attacker, true);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<com.kx.sglm.gs.battle.share.actor.impl.BattleFighter> _friendFighterList = calcTargetList(attacker, false);
			List<BattleFighter> _friendFighterList = calcTargetList(attacker, false);
			foreach (ISkillEffect _effect in allEffect)
			{
				List<BattleFighter> _fighterList = _effect.EnemyEffect ? _enemyFighterList : _friendFighterList;
				_effect.onAction(attacker, _fighterList, _holder);
				// TODO: 区分不同的effect
				optionAfterAction(attacker, _fighterList, record);
			}
		}

		public virtual int SkillId
		{
			get
			{
				return skillId;
			}
			set
			{
				this.skillId = value;
			}
		}

		/// <summary>
		/// 攻击后操�?
		/// </summary>
		/// <param name="attacker"> </param>
		/// <param name="defencerList"> </param>
		/// <param name="record"> </param>
		public virtual void optionAfterAction(BattleFighter attacker, List<BattleFighter> defencerList, BattleFightRecord record)
		{
			attacker.afterAttack(record);
			foreach (BattleFighter _defencer in defencerList)
			{
				_defencer.afterDefence(record);
			}
		}

		protected internal virtual List<BattleFighter> calcTargetList(BattleFighter attacker, bool enemyEffect)
		{
			TargetGetterHolder _holder = getTargetHolder(enemyEffect);
			return _holder.calcTargetList(attacker);
		}

		public virtual SkillDataHolder createDataHolder(BattleFightRecord record)
		{
			return new SkillDataHolder(record);
		}


		public virtual void addEnemyTargetGetter(List<ISkillTargetGetter> enemyTargetGetterList)
		{
			this.getTargetHolder(true).addAll(enemyTargetGetterList);
		}

		public virtual void addFriendTargetGetter(List<ISkillTargetGetter> friendTargetGetterList)
		{
			this.getTargetHolder(false).addAll(friendTargetGetterList);
		}

		protected internal virtual TargetGetterHolder getTargetHolder(bool enemy)
		{
			int _index = enemy ? BattleConstants.TARGET_GETTER_FOR_ENEMY_INDEX : BattleConstants.TARGET_GETTER_FOR_FRIEND_INDEX;
			return this.targetGetterHolderArr[_index];
		}

		public virtual void addEffectList(List<ISkillEffect> effectList)
		{
			this.allEffect.AddRange(effectList);
		}

	}

}
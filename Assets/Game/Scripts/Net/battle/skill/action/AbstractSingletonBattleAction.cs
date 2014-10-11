using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.action
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	/// <summary>
	/// 技能的顶层抽象类
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
		/// 技能动作，一个技能可以包括多个动作 </summary>
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

		/// <summary>
		/// 核心方法，执行技能动作
		/// </summary>
		/// <param name="attacker"> </param>
		/// <param name="record"> </param>
		internal virtual void optionAction(BattleFighter attacker, BattleFightRecord record)
		{
			// 创建一个供不同动作之间数据沟通的记录
			SkillDataHolder _holder = new SkillDataHolder();
			// 动作分两步
			// 准备数据
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<com.kx.sglm.gs.battle.share.actor.impl.BattleFighter> _enemyList = calcTargetList(attacker, true);
			List<BattleFighter> _enemyList = calcTargetList(attacker, true);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<com.kx.sglm.gs.battle.share.actor.impl.BattleFighter> _friendList = calcTargetList(attacker, false);
			List<BattleFighter> _friendList = calcTargetList(attacker, false);
			List<ISkillEffect> _activeEffects = calcActiveEffectByRate();

			_holder.Record = record;
			_holder.Attacker = attacker;
			_holder.EnemyList = _enemyList;
			_holder.FriendList = _friendList;
			_holder.ActiveEffect = _activeEffects;

			// 执行动作内容
			optionEffectAction(_holder);
			// 执行动作后操作
			optionAfterAction(_holder);

		}

		protected internal virtual bool ratioEffect(ISkillEffect effect)
		{
			return MathUtils.randomRate(effect.Ratio, BattleConstants.BATTLE_RATIO_BASE);
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

		internal virtual List<ISkillEffect> calcActiveEffectByRate()
		{
			List<ISkillEffect> _activeEffects = new List<ISkillEffect>();
			foreach (ISkillEffect _effect in allEffect)
			{
				if (!ratioEffect(_effect))
				{
					continue;
				}
				_activeEffects.Add(_effect);
			}
			return _activeEffects;
		}

		/// <summary>
		/// 技能操作
		/// </summary>
		/// <param name="holder"> </param>
		internal virtual void optionEffectAction(SkillDataHolder holder)
		{
			foreach (ISkillEffect _effect in holder.ActiveEffect)
			{
				if (!ratioEffect(_effect))
				{
					continue;
				}
				List<BattleFighter> _fighterList = holder.getTargets(_effect);
				_effect.onAction(holder.Attacker, _fighterList, holder);
			}
		}

		/// <summary>
		/// 技能释放后操作
		/// </summary>
		/// <param name="holder"> 包含当次技能的一些实时数据 </param>
		internal virtual void optionAfterAction(SkillDataHolder holder)
		{
			BattleFightRecord _record = holder.Record;
			holder.Attacker.attackerAfterSkillAction(_record);
			foreach (ISkillEffect _effect in holder.ActiveEffect)
			{
				List<BattleFighter> _fighterList = holder.getTargets(_effect);
				_effect.defencerAfterEffect(holder.Attacker, _fighterList, _record);
			}

		}

		protected internal virtual List<BattleFighter> calcTargetList(BattleFighter attacker, bool enemyEffect)
		{
			TargetGetterHolder _holder = getTargetHolder(enemyEffect);
			return _holder.calcTargetList(attacker);
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
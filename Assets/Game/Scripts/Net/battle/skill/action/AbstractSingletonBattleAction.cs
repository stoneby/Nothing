using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.action
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	/// <summary>
	/// ���ܵĶ��������
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class AbstractSingletonBattleAction : ISingletonSkillAction
	{
		public abstract bool canOption(BattleFighter attacker);

		/// <summary>
		/// ����ID </summary>
		private int skillId;

		/// <summary>
		/// Ŀ���ȡ��������Index��ͬ��Ϊ�Եл��ǶԼ� </summary>
		private TargetGetterHolder[] targetGetterHolderArr;

		/// <summary>
		/// ���ܶ�����һ�����ܿ��԰���������� </summary>
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
			// TODO: ��������߼�
			optionAction(attacker, record);
		}

		/// <summary>
		/// ���ķ�����ִ�м��ܶ���
		/// </summary>
		/// <param name="attacker"> </param>
		/// <param name="record"> </param>
		internal virtual void optionAction(BattleFighter attacker, BattleFightRecord record)
		{
			// ����һ������ͬ����֮�����ݹ�ͨ�ļ�¼
			SkillDataHolder _holder = new SkillDataHolder();
			// ����������
			// ׼������
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

			// ִ�ж�������
			optionEffectAction(_holder);
			// ִ�ж��������
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
		/// ���ܲ���
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
		/// �����ͷź����
		/// </summary>
		/// <param name="holder"> �������μ��ܵ�һЩʵʱ���� </param>
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
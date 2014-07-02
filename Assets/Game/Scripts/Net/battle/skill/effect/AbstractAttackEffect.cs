using System;

namespace com.kx.sglm.gs.battle.share.skill.effect
{

	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	public abstract class AbstractAttackEffect : AbstractSkillEffect
	{

		protected internal float attackValMuti;
		protected internal float hitCount;

		public AbstractAttackEffect() : base(true)
		{
			attackValMuti = BattleConstants.FIGHTER_FIGHT_DEFAULT_RATIO;
			hitCount = BattleConstants.FIGHTER_FIGHT_DEFAULT_COUNT;
		}

		protected internal override void initRecord(BattleFighter attacker, BattleFightRecord fightRecord)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.share.actor.impl.BattleTeam _attackerTeam = attacker.getOwnerTeam();
			BattleTeam _attackerTeam = attacker.getOwnerTeam();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _targetIndex = _attackerTeam.getIntProp((com.kx.sglm.gs.battle.share.BattleKeyConstants.BATTLE_KEY_HERO_TEAM_TARGET));
			int _targetIndex = _attackerTeam.getIntProp((BattleKeyConstants.BATTLE_KEY_HERO_TEAM_TARGET));
			BattleRecordHelper.initBattleFight(AttackType, attacker, fightRecord);
			SingleActionRecord _attackAction = fightRecord.OrCreateAttack;
			_attackAction.addProp(BattleRecordConstants.SINGLE_ACTION_PROP_ATTACK_TARGET, _targetIndex);
		}

		protected internal override void onSingleAction(BattleFighter attacker, BattleFighter defencer, SkillDataHolder resultData)
		{
			onAttack(attacker, defencer, resultData.Record);
		}

		/// <summary>
		/// S1=（武将战斗中攻击力*（1+无双技能加成倍数）-怪物防御力）*伤害加成倍数*弱点加成倍数*带盾减伤倍数*（1-减伤比）*攻击次数
		/// </summary>
		/// <param name="attacker"> </param>
		/// <param name="defencer"> </param>
		/// <param name="fightRecord"> </param>
		public virtual void onAttack(BattleFighter attacker, BattleFighter defencer, BattleFightRecord fightRecord)
		{
			// TODO: afterAttack在其他effect也可能会涉及
			SingleActionRecord _record = initDefenceRecord(defencer, fightRecord);

			float _attack = getAttack(attacker);
			float _spValMuti = this.attackValMuti;
			float _defence = defencer.Defence;
			float _indexValMuti = calcIndexRatio(attacker);
			float _weakRatio = calcJobWeakValue(attacker, defencer);
			float _damageFree = calcDamagefree(defencer.DamageFree);

			_attack = calcSingleDamage(_attack, _defence, _indexValMuti, _spValMuti, _weakRatio, _damageFree);

			float _costHpFloat = _attack * this.hitCount;
			// 四舍五入
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _costHp = com.kx.sglm.core.util.MathUtils.float2Int(_costHpFloat);
			int _costHp = MathUtils.float2Int(_costHpFloat);

			costHp(_costHp, defencer, _record);

			recordDamageInfo((int) _attack, _record);

			fightRecord.finishCurDefecner();

			Console.WriteLine("Fighter fight costHp " + _costHp);
		}

		protected internal virtual float calcSingleDamage(float attack, float defence, float indexValMuti, float spValuMuti, float weakRatio, float damageFree)
		{
			float _damage = attack * spValuMuti;
			_damage -= defence;
			_damage *= weakRatio;
			_damage *= indexValMuti;
			_damage *= damageFree;
			return _damage;
		}

		protected internal virtual void recordDamageInfo(int damage, SingleActionRecord record)
		{
			record.addProp(BattleRecordConstants.BATTLE_HERO_PROP_HIT_COUNT, (int) hitCount);
			record.addProp(BattleRecordConstants.BATTLE_HERO_PROP_HIT_SINGLE_DAMAGE, (int) damage);
		}

		protected internal virtual float calcJobWeakValue(BattleFighter attacker, BattleFighter defencer)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _jobWeak = defencer.getFighterOtherProp(com.kx.sglm.gs.battle.share.BattleKeyConstants.BATTLE_KEY_MONSTER_WEEK_JOB);
			int _jobWeak = defencer.getFighterOtherProp(BattleKeyConstants.BATTLE_KEY_MONSTER_WEEK_JOB);
			float _weakRatio = BattleConstants.FIGHTER_FIGHT_DEFAULT_RATIO;
			if (_jobWeak == attacker.Job)
			{
				_weakRatio = BattleConstants.MONSTER_WEAK_RATIO;
			}
			return _weakRatio;
		}

		protected internal virtual float calcIndexRatio(BattleFighter attacker)
		{
			return getIndexAttackRatio(attacker) / BattleConstants.BATTLE_RATIO_BASE;
		}

		protected internal virtual SingleActionRecord initDefenceRecord(BattleFighter defencer, BattleFightRecord fightRecord)
		{
			SingleActionRecord _singleRecord = fightRecord.OrCreateDefence;
			BattleRecordHelper.initDefencerRecord(defencer, _singleRecord);
			return _singleRecord;
		}

		protected internal virtual float calcDamagefree(float damageFree)
		{
			return ((BattleConstants.BATTLE_RATIO_BASE - damageFree) / BattleConstants.BATTLE_RATIO_BASE);
		}

		public virtual float getAttack(BattleFighter attacker)
		{
			float _baseAttack = attacker.AttackVal;
			float _attackRatil = getIndexAttackRatio(attacker);
			return _baseAttack * (_attackRatil / BattleConstants.BATTLE_RATIO_BASE);
		}

		protected internal abstract int AttackType {get;}

		// public abstract float calcDamage(float attack, BattleFighter attacker, BattleFighter defencer, SingleActionRecord record);

		public abstract int getIndexAttackRatio(BattleFighter attacker);

		public abstract void costHp(int costHp, BattleFighter defencer, SingleActionRecord record);

	}

}
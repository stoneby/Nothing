using System;

namespace com.kx.sglm.gs.battle.share.skill.effect
{

	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleLogicHelper = com.kx.sglm.gs.battle.share.helper.BattleLogicHelper;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;
	using HeroArrLogicHelper = com.kx.sglm.gs.battle.share.helper.HeroArrLogicHelper;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;
	using RoleAProperty = com.kx.sglm.gs.hero.properties.RoleAProperty;

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
			float _spValMuti = this.attackValMuti / BattleConstants.BATTLE_RATIO_BASE;
			float _defence = defencer.Defence;
			float _indexValMuti = calcIndexRatio(attacker);
			float _weakRatio = calcJobWeakValue(attacker, defencer);
			float _damageMuti = calcDamageModify(attacker.DamageMuti);
			float _damageFree = calcDamageModify(defencer.DamageFree);

			_attack = calcSingleDamage(_attack, _defence, _indexValMuti, _spValMuti, _weakRatio, _damageMuti, _damageFree);

			_attack = calcAttackerState(attacker, _attack);

			_attack = calcDefencerState(defencer, _attack);

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


		protected internal virtual void modifyAttackResult(float result, BattleFighter attacker, BattleFighter defencer)
		{
			RoleAProperty _prop = new RoleAProperty();
			_prop.set(RoleAProperty.ATK, result);

		}

		/// <param name="defencer"> </param>
		protected internal virtual float calcDefencerState(BattleFighter defencer, float damage)
		{
			float _resultDamage = damage;
			if (defencer.hasState(BattleConstants.MONSTER_SHIELD_FLAG))
			{
				_resultDamage = (BattleConstants.MONSTER_SHIELD_DAMAGE_REDUCE / BattleConstants.BATTLE_RATIO_BASE) * damage;
				Logger.Log(string.Format("shield is working, index = {0:D}, damage = {1:F}, result = {2:F}", defencer.Index, damage, _resultDamage));
			}
			else
			{
				Logger.Log(string.Format("shield not working, index = {0:D}, damage = {1:F}, result = {2:F}", defencer.Index, damage, _resultDamage));
			}
			return _resultDamage;
		}

		protected internal virtual float calcAttackerState(BattleFighter attacker, float damage)
		{
			float _resultDamage = damage;
			if (attacker.hasState(BattleConstants.ATTACK_ZERO_FLAG))
			{
				_resultDamage = 0;
			}
			return _resultDamage;
		}

		protected internal virtual float calcSingleDamage(float attack, float defence, float indexValMuti, float spValuMuti, float weakRatio, float damageMuti, float damageFree)
		{
			float _damage = attack * spValuMuti;
			_damage *= indexValMuti;
			_damage -= defence;
			if (_damage < 0)
			{
				_damage = BattleConstants.MIN_ATTACK;
			}
			else
			{
				_damage *= weakRatio;
				_damage *= damageMuti;
				_damage *= damageFree;
			}
			return _damage;
		}

		protected internal virtual void recordDamageInfo(int damage, SingleActionRecord record)
		{
			record.addProp(BattleRecordConstants.BATTLE_HERO_PROP_HIT_COUNT, (int) hitCount);
			record.addProp(BattleRecordConstants.BATTLE_HERO_PROP_HIT_SINGLE_DAMAGE, (int) damage);
		}

		protected internal virtual void updateShieldState(BattleFighter defencer, SingleActionRecord record)
		{
			if (defencer.hasState(BattleConstants.MONSTER_SHIELD_FLAG))
			{
				defencer.updateStateRecord(record.FighterInfo);
			}
		}

		protected internal virtual float calcJobWeakValue(BattleFighter attacker, BattleFighter defencer)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _jobWeak = defencer.getFighterOtherProp(com.kx.sglm.gs.battle.share.BattleKeyConstants.BATTLE_KEY_MONSTER_WEEK_JOB);
			int _jobWeak = defencer.getFighterOtherProp(BattleKeyConstants.BATTLE_KEY_MONSTER_WEEK_JOB);
			float _weakRatio = BattleConstants.FIGHTER_FIGHT_DEFAULT_RATIO / BattleConstants.BATTLE_RATIO_BASE;
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


		protected internal virtual float calcDamageModify(int damageModify)
		{
			return damageModify / BattleConstants.BATTLE_RATIO_BASE;
		}

		public virtual float getAttack(BattleFighter attacker)
		{
			float _baseAttack = attacker.AttackVal;
			float _attackRatil = getIndexAttackRatio(attacker);
			return _baseAttack * (_attackRatil / BattleConstants.BATTLE_RATIO_BASE);
		}

		protected internal abstract int AttackType {get;}

		public virtual int getIndexAttackRatio(BattleFighter attacker)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _curIndex = attacker.getOwnerTeam().getAttackRatioIndex(attacker);
			int _curIndex = attacker.getOwnerTeam().getAttackRatioIndex(attacker);
			return HeroArrLogicHelper.getAttackRatio(_curIndex);
		}

		public virtual void costHp(int costHp, BattleFighter defencer, SingleActionRecord record)
		{
			BattleLogicHelper.costBaseHp(costHp, defencer, record);
		}

	}

}
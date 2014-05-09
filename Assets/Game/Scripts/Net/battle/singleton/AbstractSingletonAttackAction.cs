using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.singleton
{

	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;

	/// 
	/// <summary>
	/// 单例的抽象攻击动�?
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class AbstractSingletonAttackAction : ISingletonBattleAction
	{

		public virtual void onAttack(BattleFighter attacker, BattleFighter defencer, BattleFightRecord fightRecord)
		{

			SingleActionRecord _record = initDefenceRecord(defencer, fightRecord);

			float _attack = getAttack(attacker);
			float _defence = defencer.Defence;

			_attack -= _defence;

			_attack = calcDamage(_attack, attacker, defencer);

			// 四舍五入
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _costHp = com.kx.sglm.core.util.MathUtils.float2Int(_attack);
			int _costHp = MathUtils.float2Int(_attack);

			costHp(_costHp, defencer, _record);

			fightRecord.finishCurDefecner();

			Console.WriteLine("Fighter fight costHp " + _costHp);
		}

		protected internal virtual void initAttackRecord(BattleFighter attacker, BattleFightRecord fightRecord)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.share.actor.impl.BattleTeam _attackerTeam = attacker.getOwnerTeam();
			BattleTeam _attackerTeam = attacker.getOwnerTeam();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _targetIndex = _attackerTeam.getIntProp((com.kx.sglm.gs.battle.share.BattleKeyConstants.BATTLE_KEY_HERO_TEAM_TARGET));
			int _targetIndex = _attackerTeam.getIntProp((BattleKeyConstants.BATTLE_KEY_HERO_TEAM_TARGET));
			BattleRecordHelper.initBattleFight(BattleRecordConstants.SINGLE_ACTION_TYPE_ATTACK, attacker, fightRecord);
			SingleActionRecord _attackAction = fightRecord.OrCreateAttack;
			_attackAction.addProp(BattleRecordConstants.SINGLE_ACTION_PROP_ATTACK_TARGET, _targetIndex);
		}

		protected internal virtual SingleActionRecord initDefenceRecord(BattleFighter defencer, BattleFightRecord fightRecord)
		{
			SingleActionRecord _singleRecord = fightRecord.OrCreateDefence;
			BattleRecordHelper.initSingleRecord(defencer, _singleRecord);
			_singleRecord.ActType = BattleRecordConstants.SINGLE_ACTION_TYPE_DEFENCE;
			return _singleRecord;
		}

		public virtual void onAction(BattleFighter attacker, BattleTeam defencerTeam, BattleFightRecord record)
		{
			// 初始化攻击战�?
			initAttackRecord(attacker, record);
			optionBeforeAction(attacker, defencerTeam, record);
			IList<BattleFighter> _defencerList = getDefencerList(attacker, defencerTeam);
			foreach (BattleFighter _defencer in _defencerList)
			{
				onAttack(attacker, _defencer, record);
			}
			optionAfterAction(attacker, _defencerList, record);
		}

		public virtual float getAttack(BattleFighter attacker)
		{
			float _baseAttack = attacker.AttackVal;
			float _attackRatil = getAttackRatio(attacker);
			return _baseAttack * (_attackRatil / BattleConstants.BATTLE_RATIO_BASE);
		}

		protected internal virtual float calcDamagefree(float damageFree, float attack)
		{
			return attack * ((BattleConstants.BATTLE_RATIO_BASE - damageFree) / BattleConstants.BATTLE_RATIO_BASE);
		}

		/// <summary>
		/// 攻击后操�?
		/// </summary>
		/// <param name="attacker"> </param>
		/// <param name="defencerList"> </param>
		/// <param name="record"> </param>
		public virtual void optionAfterAction(BattleFighter attacker, IList<BattleFighter> defencerList, BattleFightRecord record)
		{
			attacker.afterAttack(record);
			foreach (BattleFighter _defencer in defencerList)
			{
				_defencer.afterDefence(record);
			}
		}

		/// <summary>
		/// 攻击后操�?
		/// </summary>
		/// <param name="attacker"> </param>
		/// <param name="defencerTeam"> </param>
		/// <param name="record"> </param>
		public virtual void optionBeforeAction(BattleFighter attacker, BattleTeam defencerTeam, BattleFightRecord record)
		{
			// TODO Auto-generated method stub

		}

		public abstract float calcDamage(float attack, BattleFighter attacker, BattleFighter defencer);

		public abstract void onAddBuff(BattleFighter attacker, BattleTeam defencerTeam);

		public abstract int getAttackRatio(BattleFighter attacker);

		public abstract void costHp(int costHp, BattleFighter defencer, SingleActionRecord record);

		public abstract IList<BattleFighter> getDefencerList(BattleFighter attacker, BattleTeam defencerTeam);

	}

}
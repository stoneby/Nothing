using System;

namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;
	using BattleMonsterSkillManager = com.kx.sglm.gs.battle.share.skill.manager.BattleMonsterSkillManager;

	public class MonsterShield : AbstractBuffEffect
	{

		/// <summary>
		/// ª§∂‹À≥–Ú£¨∞¥—’…´Index≈≈–Ú </summary>
		private int shieldOrder;
		private int breakShieldCD;

		public override void onActive(BattleFighter fighter)
		{
			BattleFighterBuff _buff = fighter.BuffManager.getBattleBuff(this);
			_buff.setParam(BattleKeyConstants.BATTLE_BUFF_CUR_SHIELD_ORDER, shieldOrder);
			Logger.Log(string.Format("shield active, index = {0:D}", fighter.Index));
		}

		public override void onRemove(BattleFighter fighter)
		{
			fighter.addBuff(Id);
			Logger.Log(string.Format("shield remove, index = {0:D}", fighter.Index));
		}

		public override void onTeamBeforeAttack(BattleFighter fighter)
		{

		}

		public override void onAttack(BattleFighter attacker)
		{

		}

		public override void onDefence(BattleFighter attacker, BattleFighter owner)
		{
			if (!attacker.Hero)
			{
				return;
			}
			BattleFighterBuff _buff = owner.BuffManager.getBattleBuff(this);
			if (_buff == null)
			{
				// TODO: loggers.error
				return;
			}

			int _shieldOrder = getCurShieldOrder(_buff);
			if (isShieldOff(_shieldOrder))
			{
				Logger.Log(string.Format("shield off, index = {0:D}", owner.Index));
				return;
			}

			int _resultShield = hitShield(attacker, _shieldOrder);

			Logger.Log(string.Format("shield refresh, index = {0:D}, value = {1:D}", owner.Index, _resultShield));

			_buff.setParam(BattleKeyConstants.BATTLE_BUFF_CUR_SHIELD_ORDER, _resultShield);

			checkShieldBreak(owner, _buff, _resultShield);
		}

		protected internal virtual int hitShield(BattleFighter attacker, int shieldOrder)
		{
			HeroColor _curColor = attacker.getOwnerTeam().CurFightColor;
			int _curShieldOrder = shieldOrder % 10;
			int _resultValue = shieldOrder;
			if (_curColor.Index == _curShieldOrder)
			{
				_resultValue = shieldOrder / 10;
			}
			return _resultValue;
		}

		protected internal virtual void checkShieldBreak(BattleFighter owner, BattleFighterBuff buff, int curShieldOrder)
		{
			if (isShieldOff(curShieldOrder))
			{
				Logger.Log(string.Format("shield break, index = {0:D}", owner.Index));
				buff.ExtraRound = BreakShieldCd;
				SheildLeftRound = owner;
				owner.removeStateFlag(Id);
			}
		}

		private BattleFighter SheildLeftRound
		{
			set
			{
				if (value.Hero)
				{
					return;
				}
				BattleMonsterSkillManager _manager = (BattleMonsterSkillManager)value.SkillManager;
				_manager.setSheildLeftRound();
			}
		}

		public virtual int BreakShieldCd
		{
			get
			{
				return breakShieldCD;
			}
		}

		protected internal virtual int getCurShieldOrder(BattleFighterBuff buffInst)
		{
			return buffInst.getParam(BattleKeyConstants.BATTLE_BUFF_CUR_SHIELD_ORDER, 0);
		}

		protected internal virtual bool isShieldOff(int curShieldOrder)
		{
			return BattleConstants.MONSTER_SHIELD_NIL_VALUE == curShieldOrder;
		}

		public override void build(params string[] param)
		{
			this.shieldOrder = Convert.ToInt32(param[0]);
			this.breakShieldCD = Convert.ToInt32(param[1]);
		}

		public override FighterStateEnum StateEnum
		{
			get
			{
				return FighterStateEnum.MONSTER_SHIELD;
			}
		}

		public override bool needShow(BattleFighterBuff buffInst)
		{
			return !isShieldOff(getCurShieldOrder(buffInst));
		}

	}

}
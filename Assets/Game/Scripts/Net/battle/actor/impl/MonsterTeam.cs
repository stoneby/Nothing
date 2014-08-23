using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.actor.impl
{

	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using FighterType = com.kx.sglm.gs.battle.share.enums.FighterType;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;
	using BattleTeamShot = com.kx.sglm.gs.battle.share.logic.loop.BattleTeamShot;

	public class MonsterTeam : BattleTeam
	{

		public MonsterTeam(Battle battle, BattleSideEnum side) : base(battle, side)
		{
		}

		public override bool hasFightFighter()
		{
			return battlingActorSize() > curFightIndex;
		}

		public override BattleFighter CurFighter
		{
			get
			{
				return getActor(curFightIndex);
			}
		}

		public override void doResetReset()
		{
			curFightIndex = 0;
		}


		public override bool AllFighterDead
		{
			get
			{
				bool _isDead = true;
				foreach (BattleFighter _fighter in actorList)
				{
					_fighter.tryDead();
					if (_fighter.hasHp())
					{
						_isDead = false; // 这里不能Break，因为需要将所有fighter尝试死亡
					}
				}
				return _isDead;
			}
		}

		public override void onDead()
		{
			// TODO Auto-generated method stub

		}

		public override bool handleBattleFightInfo(int fightIndex, int[] battleIndexes)
		{
			Logger.Log("#MonsterTeam.handleBattleFight: error.action.in.battle, monster.not.action");
			return false;
		}

		public override void onTeamShotFinish(BattleTeamShot teamShot)
		{
			// TODO Auto-generated method stub

		}

		public override void onActorDead()
		{
			// TODO Auto-generated method stub

		}

		public override FighterType FighterType
		{
			get
			{
				return FighterType.MONSTER;
			}
		}

		public override bool hasFighterIndex(int fighterIndex)
		{
			bool _rightIndex = battlingActorSize() > fighterIndex;
			if (_rightIndex)
			{
				BattleFighter _fighter = getActor(fighterIndex);
				// TODO：可能以后会有其他情况不能被攻击，这里暂时写生存
				_rightIndex = _fighter.hasHp();
			}
			return _rightIndex;
		}

		public override void changeHp(int costHp, BattleFighter defencer)
		{
			// DO Nothing
		}

		public override BattleFighter getFighterByIndex(int fighterIndex)
		{
			return getActor(fighterIndex);
		}

		public override int CurHp
		{
			get
			{
				return 0;
			}
		}

		public override List<BattleFighter> AllBattingFighter
		{
			get
			{
				return AllAliveFighter;
			}
		}

		public override void changeFightColor(int fighterIndex, HeroColor color, SingleActionRecord actionRecord)
		{
			// do nothing here
		}

		public override int TotalHp
		{
			get
			{
				return 0;
			}
		}

		public override int CurMp
		{
			get
			{
				return 0;
			}
		}

		public override int getFighterColor(int fighterIndex)
		{
			// no color here
			return 0;
		}

		public override bool isActiveFighter(BattleFighter fighter)
		{
			return !fighter.Dead;
		}

		public override List<BattleFighter> CurTeamShotFighters
		{
			get
			{
				return AllBattingFighter;
			}
		}

		public override int getFighterCurHp(BattleFighter fighter)
		{
			return fighter.CurHp;
		}

		public override int getFighterTotalHp(BattleFighter fighter)
		{
			return fighter.TotalHp;
		}

		public override void costFighterHp(int costHp, BattleFighter fighter)
		{
			fighter.changeCurHp(-costHp);
		}

		public override HeroColor CurFightColor
		{
			get
			{
				return HeroColor.NIL;
			}
		}

		public override int getAttackRatioIndex(BattleFighter fighter)
		{
			return 0;
		}

	}

}
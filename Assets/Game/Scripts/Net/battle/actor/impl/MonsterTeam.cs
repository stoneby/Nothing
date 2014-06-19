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

		public override bool hasHp()
		{
			return !deadth;
		}

		public override void tryDead()
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
			Deadth = _isDead;

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


		public override List<BattleFighter> ActiveFighter
		{
			get
			{
				List<BattleFighter> _fighterList = new List<BattleFighter>();
				foreach (BattleFighter _fighter in ActorList)
				{
					if (_fighter.hasHp())
					{
						_fighterList.Add(_fighter);
					}
				}
				return _fighterList;
			}
		}

		public override void changeFightColor(int fighterIndex, HeroColor color, SingleActionRecord actionRecord)
		{
			//do nothing here
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
			// TODO Auto-generated method stub
			return 0;
		}

	}

}
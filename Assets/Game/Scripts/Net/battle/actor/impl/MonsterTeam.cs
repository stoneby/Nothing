using System;

namespace com.kx.sglm.gs.battle.share.actor.impl
{

	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using FighterType = com.kx.sglm.gs.battle.share.enums.FighterType;
	using BattleTeamShot = com.kx.sglm.gs.battle.share.logic.loop.BattleTeamShot;

	public class MonsterTeam : BattleTeam
	{

		public MonsterTeam(Battle battle, BattleSideEnum side) : base(battle, side)
		{
		}

		public override bool hasFightFighter()
		{
			return actorSize() > curFightIndex;
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

		public override bool Alive
		{
			get
			{
				return !deadth;
			}
		}

		public override void tryDead()
		{
			bool _isDead = true;
			foreach (BattleFighter _fighter in actorList)
			{
				_fighter.tryDead();
				if (_fighter.Alive)
				{
					_isDead = false; //这里不能Break，因为需要将所有fighter尝试死亡
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
			Console.WriteLine("#MonsterTeam.handleBattleFight: error.action.in.battle, monster.not.action");
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
			bool _rightIndex = actorSize() > fighterIndex;
			if (_rightIndex)
			{
				BattleFighter _fighter = getActor(fighterIndex);
				//TODO：可能以后会有其他情况不能被攻击，这里暂时写生存
				_rightIndex = _fighter.Alive;
			}
			return _rightIndex;
		}

		public override void changeHp(int costHp, BattleFighter defencer)
		{
			//DO Nothing
		}

		public override BattleFighter getFighterByIndex(int fighterIndex)
		{
			return getActor(fighterIndex);
		}

		public override int CurHp
		{
			get
			{
				// TODO Auto-generated method stub
				return 0;
			}
		}


	}

}
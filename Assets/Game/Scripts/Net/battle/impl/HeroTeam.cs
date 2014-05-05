using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.actor.impl
{


	using BattleSideEnum = com.kx.sglm.gs.battle.enums.BattleSideEnum;
	using FighterType = com.kx.sglm.gs.battle.enums.FighterType;
	using HeroColor = com.kx.sglm.gs.battle.enums.HeroColor;
	using PointDirection = com.kx.sglm.gs.battle.enums.PointDirection;
	using HeroArrLogicHelper = com.kx.sglm.gs.battle.helper.HeroArrLogicHelper;
	using BattleTeamShot = com.kx.sglm.gs.battle.logic.loop.BattleTeamShot;
	using ArrayUtils = com.kx.sglm.gs.battle.utils.ArrayUtils;

	/// <summary>
	/// 玩家的英雄队伍
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class HeroTeam : BattleTeam
	{

		/// <summary>
		/// 总血量，因为英雄队伍是没有单独血条的 </summary>
		protected internal int totalHp;

		/// <summary>
		/// 当前的血量 </summary>
		protected internal int curHp;

		/// <summary>
		/// 最大气力值 </summary>
		protected internal int totalSp;

		/// <summary>
		/// 当前在战斗内的所有武将点 </summary>
		protected internal HeroPoint[] battlingHeroArr;

		/// <summary>
		/// 等待列表中的武将点 </summary>
		protected internal LinkedList<HeroPoint> waitingHeroList;

		/// <summary>
		/// 即将出手的武将序列 </summary>
		protected internal int[] curActionArr;

		public HeroTeam(Battle battle, BattleSideEnum side) : base(battle, side)
		{
			this.waitingHeroList = new LinkedList<HeroPoint>();
			clearPoint();
		}

		public override int CurHp
		{
			get
			{
				return curHp;
			}
		}


		public virtual void initHero()
		{
			initHeroPoint();
			initTeamHP();
		}

		protected internal virtual void initHeroPoint()
		{
			waitingHeroList.Clear();
			joinWatingHeroList(actorList);
		}

		protected internal virtual void initTeamHP()
		{
			foreach (BattleFighter _fighter in actorList)
			{
				totalHp += _fighter.CurHp;
			}
			curHp = totalHp;
			totalSp = BattleConstants.TEST_TOTAL_SP;
		}

		public virtual bool isRightActionArr(int[] actionArray)
		{
			return HeroArrLogicHelper.isRightActionArr(actionArray, battlingHeroArr);
		}


		public virtual void emptyFightHeroArr()
		{
			int _fightCount = curActionArr.Length;
			for (int _i = 0; _i < _fightCount; _i++)
			{
				setBattlingHeroPoint(curActionArr[_i], HeroPoint.emptyPoint);
			}
		}

		public virtual void fillHeroArrayInside()
		{
			for (int _i = 0; _i < BattleConstants.HERO_BATTLE_ARR_LENGTH; _i++)
			{
				if (getHeroPoint(_i).Empty)
				{
					fillSinglePointInside(_i);
				}
			}
		}

		protected internal virtual void fillSinglePointInside(int emptyPointIndex)
		{
			int _toFillIndex = emptyPointIndex;
			bool _needSwitch = true;
			do
			{
				_toFillIndex = PointDirection.LEFT.getChangedIndex(_toFillIndex);
				if (!HeroArrLogicHelper.isRightPointIndex(_toFillIndex))
				{
					_needSwitch = false;
					break;
				}
			} while (getHeroPoint(_toFillIndex).Empty);
			if (_needSwitch)
			{
				ArrayUtils.switchArray(battlingHeroArr, emptyPointIndex, _toFillIndex);
			}
		}

		public virtual IList<BattleFighter> fillArrayFromWaitingHero()
		{
			IList<BattleFighter> _fighterArr = new List<BattleFighter>();
			for (int _i = 0; _i < BattleConstants.HERO_BATTLE_ARR_LENGTH; _i++)
			{
				if (getHeroPoint(_i).Empty)
				{
					HeroPoint _point = pollWatingHero();
					_fighterArr.Add(_point.Fighter);
					setBattlingHeroPoint(_i, _point);
				}
			}
			return _fighterArr;
		}

		public virtual void createNextWatingHero()
		{
			List<BattleFighter> _fighterList = new List<BattleFighter>();
			for (int _i = 0; _i < curActionArr.Length; _i++)
			{
				_fighterList.Add(getFighterFromCurActArr(_i));
			}
			joinWatingHeroList(_fighterList);
		}

		protected internal virtual void joinWatingHeroList(IList<BattleFighter> fighterList)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _fighterSize = fighterList.size();
			int _fighterSize = fighterList.Count;
			IList<HeroColor> _colorList = BattleExcuter.createColorList(_fighterSize);
			for (int _i = 0; _i < _fighterSize; _i++)
			{
				BattleFighter _fighter = fighterList[_i];
				HeroPoint _point = createHeroPoint(_fighter, _colorList[_i]);
				waitingHeroList.AddLast(_point);
			}
		}

		public virtual HeroPoint createHeroPoint(BattleFighter battleFighter, HeroColor color)
		{
			return new HeroPoint(battleFighter, color);
		}

		public virtual HeroPoint getHeroPoint(int index)
		{
			return battlingHeroArr[index];
		}

		public virtual HeroPoint pollWatingHero()
		{
			HeroPoint _point = waitingHeroList.First.Value;
			waitingHeroList.RemoveFirst();
			return _point;
		}

		public virtual void setBattlingHeroPoint(int index, HeroPoint point)
		{
			if (!ArrayUtils.isRightArrayIndex(index, battlingHeroArr))
			{
				return;
			}
			battlingHeroArr[index] = point;
			point.InBattle = true;
		}

		public override int actorSize()
		{
			return battlingHeroArr.Length;
		}

		public override bool hasFightFighter()
		{
			return CurActionLength > curFightIndex;
		}

		public override BattleFighter CurFighter
		{
			get
			{
				return getFighterFromCurActArr(curFightIndex);
			}
		}

		protected internal virtual int getRealIndex(int arrIndex)
		{
			return curActionArr[arrIndex];
		}

		public override void doResetReset()
		{
			curFightIndex = 0;
			foreach (BattleFighter _fighter in ActorList)
			{
				_fighter.resetOnNewAction();
			}
		}

		public override void tryDead()
		{
			if (CurHp <= 0)
			{
				Deadth = true;
			}
		}

		public override void onDead()
		{

		}

		public override bool handleBattleFightInfo(int targetIndex, int[] battleIndexes)
		{
			if (!isRightFightInfo(targetIndex, battleIndexes))
			{
				return false;
			}
			CurActionArr = battleIndexes;
			addProp(BattleKeyConstants.BATTLE_KEY_HERO_TEAM_TARGET, targetIndex);
			return true;
		}

		protected internal virtual bool isRightFightInfo(int targetIndex, int[] battleIndexes)
		{
			if (!isRightActionArr(battleIndexes))
			{
				// TODO: add log
				return false;
			}
			if (!battle.BattleType.canSelectTarget())
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BattleTeam _oppoTeam = getOppositeTeam();
			BattleTeam _oppoTeam = OppositeTeam;
			if (!_oppoTeam.hasFighterIndex(targetIndex))
			{
				return false;
			}
			return true;
		}

		public virtual void clearPoint()
		{
			this.battlingHeroArr = new HeroPoint[BattleConstants.HERO_BATTLE_ARR_LENGTH];
			for (int _i = 0; _i < BattleConstants.HERO_BATTLE_ARR_LENGTH; _i++)
			{
				setBattlingHeroPoint(_i, HeroPoint.emptyPoint);
			}
		}

		public virtual int[] CurActionArr
		{
			set
			{
				this.curActionArr = value;
			}
			get
			{
				return curActionArr;
			}
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
				return FighterType.HERO;
			}
		}

		public override bool hasFighterIndex(int fighterIndex)
		{
			return fighterIndex < battlingHeroArr.Length;
		}

		public virtual HeroPoint[] BattlingHeroArr
		{
			get
			{
				return battlingHeroArr;
			}
		}

		public virtual LinkedList<HeroPoint> WaitingHeroList
		{
			get
			{
				return waitingHeroList;
			}
		}


		public virtual int CurActionLength
		{
			get
			{
				return CurActionArr.Length;
			}
		}

		public virtual int TotalHp
		{
			get
			{
				return totalHp;
			}
		}

		public override void costHp(int costHp, BattleFighter defencer)
		{
			curHp -= costHp;
			if (curHp <= 0)
			{
				curHp = 0;
			}
		}

		public virtual BattleFighter getFighterFromCurActArr(int fighterIndex)
		{
			int _realIndex = getRealIndex(fighterIndex);
			return getFighterByIndex(_realIndex);
		}

		public override BattleFighter getFighterByIndex(int fighterIndex)
		{
			return battlingHeroArr[fighterIndex].Fighter;
		}

		public virtual int TotalSp
		{
			get
			{
				return totalSp;
			}
		}

	}

}
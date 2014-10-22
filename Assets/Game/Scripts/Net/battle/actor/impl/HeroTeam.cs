using System.Collections.Generic;
using System.Text;

namespace com.kx.sglm.gs.battle.share.actor.impl
{


	using BattleIndexRecord = com.kx.sglm.gs.battle.share.data.record.BattleIndexRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using BattleTeamFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using FighterType = com.kx.sglm.gs.battle.share.enums.FighterType;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;
	using PointDirection = com.kx.sglm.gs.battle.share.enums.PointDirection;
	using HeroArrLogicHelper = com.kx.sglm.gs.battle.share.helper.HeroArrLogicHelper;
	using BattleTeamShot = com.kx.sglm.gs.battle.share.logic.loop.BattleTeamShot;
	using ArrayUtils = com.kx.sglm.gs.battle.share.utils.ArrayUtils;

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
		/// 当前气力值 </summary>
		protected internal int curMp;

		/// <summary>
		/// 最大气力值 </summary>
		protected internal int totalMp;

		/// <summary>
		/// 当前在战斗内的所有武将点 </summary>
		protected internal HeroPoint[] battlingHeroArr;

		/// <summary>
		/// 等待列表中的武将点 </summary>
		protected internal LinkedList<HeroPoint> waitingHeroList;

		protected internal HeroPoint[] indexedHeroPoint;

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
			initTeamProp();
		}

		protected internal virtual void initHeroPoint()
		{
			waitingHeroList.Clear();
			indexedHeroPoint = new HeroPoint[actorList.Count];
			joinWatingHeroList(actorList);
		}

		protected internal virtual void initTeamProp()
		{
			foreach (BattleFighter _fighter in actorList)
			{
				totalHp += _fighter.TotalHp;
				totalMp += _fighter.TotalMp;
			}
			curHp = totalHp;
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

		public virtual List<BattleFighter> fillArrayFromWaitingHero()
		{
			List<BattleFighter> _fighterArr = new List<BattleFighter>();
			for (int _i = 0; _i < BattleConstants.HERO_BATTLE_ARR_LENGTH; _i++)
			{
				if (getHeroPoint(_i).Empty)
				{
					HeroPoint _point = pollWatingHero();
					randomCreateSpMax(_point);
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

		protected internal virtual void randomCreateSpMax(HeroPoint heroPoint)
		{
			if (heroPoint.Color.Recover)
			{
				return;
			}
			heroPoint.Fighter.randomSpMax();
		}

		protected internal virtual void joinWatingHeroList(List<BattleFighter> fighterList)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _fighterSize = fighterList.size();
			int _fighterSize = fighterList.Count;
			List<HeroColor> _colorList = BattleExcuter.createColorList(_fighterSize);
			for (int _i = 0; _i < _fighterSize; _i++)
			{
				BattleFighter _fighter = fighterList[_i];
				HeroPoint _point = createHeroPoint(_fighter, _colorList[_i]);
				waitingHeroList.AddLast(_point);
			}
		}

		public virtual HeroPoint createHeroPoint(BattleFighter battleFighter, HeroColor color)
		{
			HeroPoint _newPoint = new HeroPoint(battleFighter, color);
			indexedHeroPoint[battleFighter.Index] = _newPoint;
			return _newPoint;
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

		public override int battlingActorSize()
		{
			return battlingHeroArr.Length;
		}

		public override HeroColor CurFightColor
		{
			get
			{
				if (CurActionLength == 0)
				{
					return HeroColor.NIL;
				}
				HeroPoint _firstPoint = getHeroPoint(curActionArr[0]);
				return _firstPoint.Color;
			}
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

		public override bool AllFighterDead
		{
			get
			{
				return CurHp <= 0;
    
			}
		}

		public override void onDead()
		{

		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unused") public boolean handleBattleFightInfo(int targetIndex, int[] battleIndexes)
		public override bool handleBattleFightInfo(int targetIndex, int[] battleIndexes)
		{
			if (!isRightFightInfo(targetIndex, battleIndexes))
			{
				return false;
			}
			// 先创建一个index，因为客户端要先产生index
			// TODO: 这里不太好先重构
			BattleIndexRecord _indexRecord = Battle.Record.OrCreateIndexRecord;
			updateCurActionArr(battleIndexes);
			bool _isRecover = CurActionRecover;
			calcFighterAction(_isRecover);
			createSpMaxForRecover(_isRecover);
			addProp(BattleKeyConstants.BATTLE_KEY_HERO_TEAM_TARGET, targetIndex);
			printAllColor("#handleBattleFightInfo");
			return true;
		}

		protected internal virtual bool isRightFightInfo(int targetIndex, int[] battleIndexes)
		{
			if (!isRightActionArr(battleIndexes))
			{
				Logger.Log("#HeroTeam.isRightFightInfo action error");
				Battle.Record.addErrorInfo(BattleRecordConstants.BATTLE_ERROR_HERO_COLER_INDEX, "");
				return false;
			}
			if (!battle.BattleType.canSelectTarget())
			{
				Logger.Log("#HeroTeam.isRightFightInfo target error");
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BattleTeam _oppoTeam = getOppositeTeam();
			BattleTeam _oppoTeam = OppositeTeam;
			if (!_oppoTeam.hasFighterIndex(targetIndex))
			{
				Battle.Record.addErrorInfo(BattleRecordConstants.BATTLE_ERROR_TARGET, targetIndex + "");
				return false;
			}
			return true;
		}

		protected internal virtual bool CurActionRecover
		{
			get
			{
				return getHeroPoint(curActionArr[0]).Color.Recover;
			}
		}

		protected internal virtual void calcFighterAction(bool isRecover)
		{
			BattleTeamFightRecord _record = Battle.Record.OrCreateTeamFighterRecord;
			if (isRecover)
			{
				return;
			}
			for (int _i = 0; _i < curActionArr.Length; _i++)
			{
				HeroPoint _point = getHeroPoint(curActionArr[_i]);
				_point.Fighter.onHandleFightInputAction(_record);
			}
		}

		protected internal virtual void createSpMaxForRecover(bool isRecover)
		{
			if (!isRecover)
			{
				return;
			}
			if (curActionArr.Length < BattleConstants.SP_MAX_NEED_RECOVER)
			{
				return;
			}
			foreach (HeroPoint _point in waitingHeroList)
			{
				if (!_point.Color.Recover)
				{
					_point.Fighter.addSpMaxBuff();
					//只生成一个SP
					break;
				}
			}
		}

		public virtual void clearPoint()
		{
			this.battlingHeroArr = new HeroPoint[BattleConstants.HERO_BATTLE_ARR_LENGTH];
			for (int _i = 0; _i < BattleConstants.HERO_BATTLE_ARR_LENGTH; _i++)
			{
				setBattlingHeroPoint(_i, HeroPoint.emptyPoint);
			}
		}

		public virtual void updateCurActionArr(int[] curActionArr)
		{
			this.curActionArr = curActionArr;
			updateAddMp();
		}

		protected internal virtual void updateAddMp()
		{
			addCurMp(curActionArr.Length);
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

		public virtual int[] CurActionArr
		{
			get
			{
				return curActionArr;
			}
		}

		public virtual int CurActionLength
		{
			get
			{
				return CurActionArr.Length;
			}
		}

		public override int TotalHp
		{
			get
			{
				return totalHp;
			}
		}

		public virtual void addCurMp(int addVal)
		{
			int _totalVal = changeValue(CurMp, addVal, TotalMp);
			HeroCurMp = _totalVal;
		}

		public virtual void costCurMp(int costVal)
		{
			int _reVal = changeValue(CurMp, -costVal, TotalMp);
			HeroCurMp = _reVal;
		}

		public virtual int changeValue(int baseValue, int changeValue, int maxValue)
		{
			int _result = baseValue + changeValue;
			_result = _result > maxValue ? maxValue : _result;
			_result = _result < 0 ? 0 : _result;
			return _result;
		}

		public virtual bool hasEnoughMp(int needVal)
		{
			return needVal <= CurMp;
		}

		public virtual bool isLastAllAttackIndex(int fighterIndex)
		{
			bool _allAttackIndex = curActionArr.Length == BattleConstants.HERO_BATTLE_ARR_LENGTH;
			if (_allAttackIndex)
			{
				int _pointIndex = curActionArr[BattleConstants.HERO_BATTLE_ARR_LENGTH - 1];
				_allAttackIndex = getHeroPoint(_pointIndex).Fighter.Index == fighterIndex;
			}
			return _allAttackIndex;
		}

		public override int CurMp
		{
			get
			{
				return curMp;
			}
		}

		public override void changeHp(int changeHp, BattleFighter defencer)
		{
			int _totalHp = changeValue(CurHp, changeHp, TotalHp);
			HeroCurHp = _totalHp;
		}

		public virtual int HeroCurHp
		{
			set
			{
				this.curHp = value;
			}
		}


		public virtual int HeroCurMp
		{
			set
			{
				this.curMp = value;
			}
		}

		public virtual BattleFighter getFighterFromCurActArr(int fighterIndex)
		{
			int _realIndex = getRealIndex(fighterIndex);
			return getFighterByIndex(_realIndex);
		}

		public override BattleFighter getFighterByIndex(int fighterIndex)
		{
			return getHeroPoint(fighterIndex).Fighter;
		}

		public virtual int TotalMp
		{
			get
			{
				return totalMp;
			}
		}

		public override List<BattleFighter> AllBattingFighter
		{
			get
			{
				List<BattleFighter> _activeFigherList = new List<BattleFighter>();
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final HeroPoint[] _battlingPoint = getBattlingHeroArr();
				HeroPoint[] _battlingPoint = BattlingHeroArr;
				foreach (HeroPoint _point in _battlingPoint)
				{
					_activeFigherList.Add(_point.Fighter);
				}
				return _activeFigherList;
			}
		}

		public override void changeFightColor(int fighterIndex, HeroColor color, SingleActionRecord actionRecord)
		{
			HeroPoint _point = this.indexedHeroPoint[fighterIndex];
			if (!_point.InBattle)
			{
				// TODO: BattleLogger.logError
			}
			_point.updateColor(color);
			actionRecord.addProp(BattleRecordConstants.BATTLE_HERO_PROP_COLOR_CHANGE, _point.Color.Index);
		}

		protected internal virtual void printAllColor(string @event)
		{
			StringBuilder _sb = new StringBuilder();
			_sb.Append("curAction: ");
			foreach (int _index in curActionArr)
			{
				_sb.Append(_index).Append(";");
			}
			_sb.Append("  curPoint: ");
			foreach (HeroPoint _point in battlingHeroArr)
			{
				_sb.Append(_point.toLogString()).Append(";");
			}
			_sb.Append("  curWaiting: ");
			foreach (HeroPoint _point in waitingHeroList)
			{
				_sb.Append(_point.toLogString()).Append(";");
			}
			Logger.Log(_sb.ToString());
		}

		public override int getFighterColor(int fighterIndex)
		{
			HeroPoint _point = indexedHeroPoint[fighterIndex];
			return _point.Color.Index;
		}

		public override bool isActiveFighter(BattleFighter fighter)
		{
			return this.indexedHeroPoint[fighter.index].InBattle;
		}

		public override List<BattleFighter> CurTeamShotFighters
		{
			get
			{
				List<BattleFighter> _fighterList = new List<BattleFighter>();
				foreach (int _index in curActionArr)
				{
					_fighterList.Add(getFighterByIndex(_index));
				}
				return _fighterList;
			}
		}

		public override int getFighterCurHp(BattleFighter fighter)
		{
			return curHp;
		}

		public override int getFighterTotalHp(BattleFighter fighter)
		{
			return totalHp;
		}

		public override void costFighterHp(int costHp, BattleFighter fighter)
		{
			changeHp(-costHp, fighter);
		}

		public override int getAttackRatioIndex(BattleFighter fighter)
		{
			return CurFightIndex;
		}


	}

}
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.factory.creater
{


	using BattleFighter = com.kx.sglm.gs.battle.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.actor.impl.BattleTeam;
	using HeroTeam = com.kx.sglm.gs.battle.actor.impl.HeroTeam;
	using MonsterTeam = com.kx.sglm.gs.battle.actor.impl.MonsterTeam;
	using BattleSource = com.kx.sglm.gs.battle.data.BattleSource;
	using FighterInfo = com.kx.sglm.gs.battle.data.FighterInfo;
	using BattleSideEnum = com.kx.sglm.gs.battle.enums.BattleSideEnum;
	using IBattleSkillManager = com.kx.sglm.gs.battle.skill.IBattleSkillManager;
	using FighterAProperty = com.kx.sglm.gs.battle.utils.FighterAProperty;

	/// <summary>
	/// 战斗武将静态创建者
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleFighterCreater
	{

		public static HeroTeam createHeroTeam(Battle battle, IList<FighterInfo> fighterProps, BattleSideEnum side)
		{
			HeroTeam _team = new HeroTeam(battle, side);
			initTeam(_team, fighterProps);
			return _team;
		}

		public static MonsterTeam createMonsterTeam(Battle battle, IList<FighterInfo> fighterProps, BattleSideEnum side)
		{
			MonsterTeam _team = new MonsterTeam(battle, side);
			initTeam(_team, fighterProps);
			_team.BattleSide = side;
			return _team;
		}

		protected internal static void initTeam(BattleTeam team, IList<FighterInfo> fighterProps)
		{
			foreach (FighterInfo _fighterProp in fighterProps)
			{
				BattleFighter _fighter = createBattleFighterFromBattleProp(team, _fighterProp);
				team.addActor(_fighter);
			}
		}

		public static BattleFighter createBattleFighterFromBattleProp(BattleTeam ownerTeam, FighterInfo fighterProp)
		{
			BattleFighter _fighter = new BattleFighter(ownerTeam, fighterProp);
			_fighter.Index = fighterProp.Index;
			initFighterFromFighterProp(_fighter, fighterProp.BattleProperty);
			IBattleSkillManager _skillaManager = ownerTeam.FighterType.createSkillManager(_fighter);
			_fighter.SkillManager = _skillaManager;
			_fighter.OwnerTeam = ownerTeam;
			return _fighter;
		}

		public static void initFighterFromFighterProp(BattleFighter fighter, FighterAProperty prop)
		{
			fighter.CurHp = prop.get(FighterAProperty.HP);
			fighter.Attack = prop.get(FighterAProperty.ATK);
			fighter.Recover = prop.get(FighterAProperty.RECOVER);
			fighter.Sp = prop.get(FighterAProperty.MP);
		}



		public static IList<FighterInfo> getSideFighter(Battle battle, BattleSideEnum side)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.data.BattleSource _source = battle.getBattleSource();
			BattleSource _source = battle.BattleSource;
			return _source.getSideFighters(side);
		}

		public static IList<MonsterTeam> createMonsterTeamList(Battle battle)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<com.kx.sglm.gs.battle.data.FighterInfo> _monsterList = getSideFighter(battle, com.kx.sglm.gs.battle.enums.BattleSideEnum.SIDEB);
			IList<FighterInfo> _monsterList = getSideFighter(battle, BattleSideEnum.SIDEB);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<Integer, java.util.List<com.kx.sglm.gs.battle.data.FighterInfo>> _monsterMap = createSceneFighterMap(_monsterList);
			IDictionary<int?, IList<FighterInfo>> _monsterMap = createSceneFighterMap(_monsterList);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _sceneSize = _monsterMap.size();
			int _sceneSize = _monsterMap.Count;

			IList<MonsterTeam> _monsterTeamList = new List<MonsterTeam>();
			for (int _i = 0; _i < _sceneSize; _i++)
			{
				IList<FighterInfo> _list = _monsterMap[_i];
				MonsterTeam _monster = BattleFighterCreater.createMonsterTeam(battle, _list, BattleSideEnum.SIDEB);
				_monsterTeamList.Add(_monster);
			}
			return _monsterTeamList;
		}


		public static IDictionary<int?, IList<FighterInfo>> createSceneFighterMap(IList<FighterInfo> monsterList)
		{
			IDictionary<int?, IList<FighterInfo>> _monsterMap = new Dictionary<int?, IList<FighterInfo>>();
			foreach (FighterInfo _info in monsterList)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _sceneIndex = _info.getIntProp(com.kx.sglm.gs.battle.BattleKeyConstants.BATTLE_KEY_MONSTER_SCENE);
				int _sceneIndex = _info.getIntProp(BattleKeyConstants.BATTLE_KEY_MONSTER_SCENE);

				if (!_monsterMap.ContainsKey(_sceneIndex))
				{
					_monsterMap[_sceneIndex] = new List<FighterInfo>();
				}
				IList<FighterInfo> _sceneList = _monsterMap[_sceneIndex];
				_sceneList.Add(_info);
			}
			return _monsterMap;
		}


	}
















}
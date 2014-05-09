using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.factory.creater
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;
	using MonsterTeam = com.kx.sglm.gs.battle.share.actor.impl.MonsterTeam;
	using BattleSource = com.kx.sglm.gs.battle.share.data.BattleSource;
	using FighterInfo = com.kx.sglm.gs.battle.share.data.FighterInfo;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using HeroSkillManager = com.kx.sglm.gs.battle.share.skill.HeroSkillManager;
	using IBattleSkillManager = com.kx.sglm.gs.battle.share.skill.IBattleSkillManager;
	using MonsterSkillManager = com.kx.sglm.gs.battle.share.skill.MonsterSkillManager;
	using FighterAProperty = com.kx.sglm.gs.battle.share.utils.FighterAProperty;

	/// <summary>
	/// 战斗武将静态创建�?
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleFighterCreater
	{

		public static HeroTeam createHeroTeam(Battle battle, IList<FighterInfo> fighterProps, BattleSideEnum side)
		{
			HeroTeam _team = new HeroTeam(battle, side);
			foreach (FighterInfo _fighterProp in fighterProps)
			{
				BattleFighter _fighter = createHeroFighterFromBattleProp(_team, _fighterProp);
				_team.addActor(_fighter);
			}
			return _team;
		}

		public static MonsterTeam createMonsterTeam(Battle battle, IList<FighterInfo> fighterProps, BattleSideEnum side)
		{
			MonsterTeam _team = new MonsterTeam(battle, side);
			foreach (FighterInfo _fighterProp in fighterProps)
			{
				BattleFighter _fighter = createMonsterFighterFromBattleProp(_team, _fighterProp);
				_team.addActor(_fighter);
			}
			_team.BattleSide = side;
			return _team;
		}

		protected internal static BattleFighter createHeroFighterFromBattleProp(HeroTeam heroTeam, FighterInfo fighterProps)
		{
			BattleFighter _fighter = createBattleFighterFromBattleProp(heroTeam, fighterProps);
			IBattleSkillManager _skillaManager = new HeroSkillManager(_fighter, heroTeam);
			_fighter.SkillManager = _skillaManager;
			return _fighter;
		}

		protected internal static BattleFighter createMonsterFighterFromBattleProp(MonsterTeam monsterTeam, FighterInfo fighterProps)
		{
			BattleFighter _fighter = createBattleFighterFromBattleProp(monsterTeam, fighterProps);
			IBattleSkillManager _skillaManager = new MonsterSkillManager(_fighter);
			_fighter.SkillManager = _skillaManager;
			return _fighter;
		}



		public static BattleFighter createBattleFighterFromBattleProp(BattleTeam ownerTeam, FighterInfo fighterProp)
		{
			BattleFighter _fighter = new BattleFighter(ownerTeam, fighterProp);
			_fighter.Index = fighterProp.Index;
			initFighterActFromFighterProp(_fighter, fighterProp.BattleProperty);
			_fighter.setOwnerTeam(ownerTeam);
			return _fighter;
		}



		public static void initFighterActFromFighterProp(BattleFighter fighter, FighterAProperty prop)
		{
			fighter.CurHp = fighter.TotalHp;
			fighter.Attack = prop.get(FighterAProperty.ATK);
			fighter.Recover = prop.get(FighterAProperty.RECOVER);
		}



		public static IList<FighterInfo> getSideFighter(Battle battle, BattleSideEnum side)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.share.data.BattleSource _source = battle.getBattleSource();
			BattleSource _source = battle.BattleSource;
			return _source.getSideFighters(side);
		}

		public static IList<MonsterTeam> createMonsterTeamList(Battle battle)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<com.kx.sglm.gs.battle.share.data.FighterInfo> _monsterList = getSideFighter(battle, com.kx.sglm.gs.battle.share.enums.BattleSideEnum.SIDEB);
			IList<FighterInfo> _monsterList = getSideFighter(battle, BattleSideEnum.SIDEB);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<Integer, java.util.List<com.kx.sglm.gs.battle.share.data.FighterInfo>> _monsterMap = createSceneFighterMap(_monsterList);
			IDictionary<int, IList<FighterInfo>> _monsterMap = createSceneFighterMap(_monsterList);
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


		public static IDictionary<int, IList<FighterInfo>> createSceneFighterMap(IList<FighterInfo> monsterList)
		{
			IDictionary<int, IList<FighterInfo>> _monsterMap = new Dictionary<int, IList<FighterInfo>>();
			foreach (FighterInfo _info in monsterList)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _sceneIndex = _info.getIntProp(com.kx.sglm.gs.battle.share.BattleKeyConstants.BATTLE_KEY_MONSTER_SCENE);
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
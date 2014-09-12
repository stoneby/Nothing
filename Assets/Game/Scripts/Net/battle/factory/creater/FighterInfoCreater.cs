using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.factory.creater
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using FighterInfo = com.kx.sglm.gs.battle.share.data.FighterInfo;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using FighterType = com.kx.sglm.gs.battle.share.enums.FighterType;
	using RoleAProperty = com.kx.sglm.gs.hero.properties.RoleAProperty;
	using BattleMsgHero = KXSGCodec.BattleMsgHero;
	using BattleMsgMonster = KXSGCodec.BattleMsgMonster;

	/// 
	/// <summary>
	/// @author liyuan2
	/// 
	/// </summary>
	public class FighterInfoCreater
	{

		public static List<FighterInfo> createListFromMsgHero(BattleSideEnum side, List<BattleMsgHero> msgHeroList)
		{
			List<FighterInfo> _fighterList = new List<FighterInfo>();
			foreach (BattleMsgHero _hero in msgHeroList)
			{
				FighterInfo _info = createFighterFromMsgHero(side, _hero);
				_fighterList.Add(_info);
			}
			return _fighterList;
		}

		public static FighterInfo createFighterFromMsgHero(BattleSideEnum side, BattleMsgHero msgHero)
		{
			FighterInfo _info = new FighterInfo();
			_info.Index = msgHero.Index;
			_info.BattleSide = side;
			_info.addNormalProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE, msgHero.TemplateId);
			_info.addNormalProp(BattleKeyConstants.BATTLE_KEY_HERO_JOB, msgHero.JobId);
			_info.addNormalProp(BattleKeyConstants.BATTLE_KEY_HERO_TYPE, msgHero.HeroType);
			_info.FighterType = FighterType.HERO;
			_info.BattleProperty = msgHero.FighteProp;
			_info.SkillIdList = msgHero.AllSkill;
			_info.ActiveSkillId = msgHero.ActiveSkillId;
			_info.LeaderSkillId = msgHero.LeaderSkill;
			return _info;
		}

		public static void initDropMapFromMsgMonster(List<FighterInfo> fighterInfos, List<BattleMsgMonster> msgMonsters)
		{
			int _fighterSize = fighterInfos.Count;
			int _monsterSize = msgMonsters.Count;
			if (_fighterSize != _monsterSize)
			{
				Logger.Log(string.Format("fighterInfo size = {0:D}, msgMonster size = {1:D}, not fit!", _fighterSize, _monsterSize));
			}
			for (int _i = 0; _i < _fighterSize; _i++)
			{
				createMonsterDrop(fighterInfos[_i], msgMonsters[_i].DropMap);
			}
		}

		public static void initFighterIndexFromMonsterGrop(BattleSideEnum side, List<int> monsterGroup, List<FighterInfo> monsterFighter)
		{

			// 在创建MonsterInfo的时候注入SceneIndex
			int _sceneMonsterCount = 0;
			int _sceneIndex = 0;

			foreach (FighterInfo _info in monsterFighter)
			{
				_info.addNormalProp(BattleKeyConstants.BATTLE_KEY_MONSTER_SCENE, _sceneIndex);
				_info.Index = _sceneMonsterCount;
				if (++_sceneMonsterCount >= monsterGroup[_sceneIndex])
				{
					_sceneIndex++;
					_sceneMonsterCount = 0;
				}
			}

		}

		public static FighterInfo createFighterFromMsgMonster(BattleSideEnum side, BattleMsgMonster msgMonster)
		{
			FighterInfo _info = new FighterInfo();
			_info.Index = msgMonster.Index;
			_info.BattleSide = side;
			_info.addNormalProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE, msgMonster.TemplateId);
			_info.addNormalProp(BattleKeyConstants.BATTLE_KEY_HERO_JOB, 1);
			_info.FighterType = FighterType.MONSTER;
			createMonsterDrop(_info, msgMonster.DropMap);
			return _info;
		}

		protected internal static void createMonsterDrop(FighterInfo info, Dictionary<sbyte, int> dropMap)
		{
			setMonsterDrop(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_COIN, info, dropMap);
			setMonsterDrop(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_SPRIT, info, dropMap);
			setMonsterDrop(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_HERO, info, dropMap);
			setMonsterDrop(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_ITEM, info, dropMap);
			setMonsterDrop(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_CHIP, info, dropMap);
		}

		protected internal static void setMonsterDrop(int key, FighterInfo info, Dictionary<sbyte, int> dropMap)
		{
			sbyte _byteKey = (sbyte) key;
			if (dropMap.ContainsKey(_byteKey))
			{
				int _dropHero = dropMap[_byteKey];
				info.addNormalProp(key, _dropHero);
			}
		}

		public static List<FighterInfo> createListFromTest(BattleSideEnum side)
		{
			return createTestFighterList(9, side, FighterType.MONSTER);
		}

		public static List<FighterInfo> createTestFighterList(int count, BattleSideEnum side, FighterType type)
		{
			List<FighterInfo> _fighterList = new List<FighterInfo>();
			for (int _i = 0; _i < count; _i++)
			{
				_fighterList.Add(createPropFromTest(_i, side, type, _i == count - 1));
			}
			return _fighterList;
		}


		public static FighterInfo createPropFromTest(int index, BattleSideEnum battleSide, FighterType type, bool boss)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<Integer, Integer> _aProp = createTestAProp(type.isHero(), boss);
			Dictionary<int, int> _aProp = createTestAProp(type.Hero, boss);
			FighterInfo _info = createFighterProp(index, battleSide, type, _aProp);
			_info.addNormalProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE, 111001); // 测试代码很糙请无视
			_info.addNormalProp(BattleKeyConstants.BATTLE_KEY_HERO_JOB, 1);
			return _info;
		}

		private static int[] TEST_HP = new int[] {2000, 1500, 1000, 500, 1000};
		private static int[] TEST_ATT = new int[] {500, 200, 200, 100, 50};

		private static Dictionary<int, int> createTestAProp(bool hero, bool boss)
		{
			Dictionary<int, int> _prop = new Dictionary<int, int>();
			for (int _i = 0; _i < RoleAProperty._SIZE; _i++)
			{
				int _val = 0;
				int _index = hero ? 4 : (boss ? 0 : MathUtils.random(1, 3));
				if (RoleAProperty.HP == _i)
				{
					_val = TEST_HP[_index];
				}
				else
				{
					_val = TEST_ATT[_index];
				}
				_prop[_i] = _val;
			}
			return _prop;
		}

		public static FighterInfo createFighterProp(int index, BattleSideEnum side, FighterType type, Dictionary<int, int> prop)
		{
			FighterInfo _info = new FighterInfo();
			_info.Index = index;
			_info.BattleProperty = prop;
			_info.BattleSide = side;
			_info.FighterType = type;
			return _info;
		}

	}

}
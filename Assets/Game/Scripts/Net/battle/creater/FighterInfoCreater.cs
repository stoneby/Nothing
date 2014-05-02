using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.factory.creater
{


	using FighterInfo = com.kx.sglm.gs.battle.data.FighterInfo;
	using BattleSideEnum = com.kx.sglm.gs.battle.enums.BattleSideEnum;
	using FighterType = com.kx.sglm.gs.battle.enums.FighterType;
	using FighterAProperty = com.kx.sglm.gs.battle.utils.FighterAProperty;
	using BattleMsgHero = KXSGCodec.BattleMsgHero;
	using BattleMsgMonster = KXSGCodec.BattleMsgMonster;

	public class FighterInfoCreater
	{

		public static IList<FighterInfo> createListFromMsgHero(BattleSideEnum side, IList<BattleMsgHero> msgHeroList)
		{
			IList<FighterInfo> _fighterList = new List<FighterInfo>();
			foreach (BattleMsgHero _hero in msgHeroList)
			{
				FighterInfo _info = createFighterFromMsgHero(side, _hero);
				_fighterList.Add(_info);
			}
			return _fighterList;
		}

		public static IList<FighterInfo> createListFormMsgMonster(BattleSideEnum side, IList<int?> monsterGroup, IList<BattleMsgMonster> msgMonsterList)
		{
			IList<FighterInfo> _monsterList = new List<FighterInfo>();

			// 在创建MonsterInfo的时候注入SceneIndex
			int _sceneMonsterCount = 0;
			int _sceneIndex = 0;

			foreach (BattleMsgMonster _monster in msgMonsterList)
			{
				FighterInfo _info = createFighterFromMsgMonster(side, _monster);
				_info.addNormalProp(BattleKeyConstants.BATTLE_KEY_MONSTER_SCENE, _sceneIndex);
				_info.Index = _sceneMonsterCount;
				if (++_sceneMonsterCount >= monsterGroup[_sceneIndex])
				{
					_sceneIndex++;
					_sceneMonsterCount = 0;
				}
				_monsterList.Add(_info);
			}
			return _monsterList;
		}

		public static FighterInfo createFighterFromMsgHero(BattleSideEnum side, BattleMsgHero msgHero)
		{
			FighterInfo _info = new FighterInfo();
			_info.Index = msgHero.Index;
			_info.BattleSide = side;
			_info.addFighterProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE, msgHero.TemplateId);
			_info.addFighterProp(BattleKeyConstants.BATTLE_KEY_HERO_TYPE, msgHero.HeroType);
			_info.FighterType = FighterType.HERO;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.utils.FighterAProperty _prop = createFighterAProp(msgHero.getFighteProp());
            FighterAProperty _prop = createFighterAPropInt(msgHero.FighteProp);
			_info.BattleProperty = _prop;
			return _info;
		}

		public static FighterAProperty createFighterAProp(IDictionary<int?, int?> fighterPropMap)
		{
			FighterAProperty _prop = new FighterAProperty();
			_prop.addAllProp(fighterPropMap);
			return _prop;
		}

		public static FighterInfo createFighterFromMsgMonster(BattleSideEnum side, BattleMsgMonster msgMonster)
		{
			FighterInfo _info = new FighterInfo();
			_info.Index = msgMonster.Index;
			_info.BattleSide = side;
			_info.addFighterProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE, msgMonster.TemplateId);
			_info.FighterType = FighterType.MONSTER;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.utils.FighterAProperty _prop = createFighterAProp(msgMonster.getFighteProp());
            FighterAProperty _prop = createFighterAPropInt(msgMonster.FighteProp);
			_info.BattleProperty = _prop;
			return _info;
		}

        public static FighterAProperty createFighterAPropInt(IDictionary<int, int> fighterDictionary)
        {
            IDictionary<int?, int?> _propDic = new Dictionary<int?, int?>();

            foreach (KeyValuePair<int, int> kvp in fighterDictionary)
            {
                _propDic.Add((int?)kvp.Key, (int?)kvp.Value);
            }
            return createFighterAProp(_propDic);
        }

		public static IList<FighterInfo> createListFromTest(BattleSideEnum side)
		{
			return createTestFighterList(3, side, FighterType.MONSTER);
		}

		public static IList<FighterInfo> createTestFighterList(int count, BattleSideEnum side, FighterType type)
		{
			IList<FighterInfo> _fighterList = new List<FighterInfo>();
			for (int _i = 0; _i < count; _i++)
			{
				_fighterList.Add(createPropFromTest(_i, side, type));
			}
			return _fighterList;
		}

		public static FighterInfo createPropFromTest(int index, BattleSideEnum battleSide, FighterType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.utils.FighterAProperty _aProp = createTestAProp();
			FighterAProperty _aProp = createTestAProp();
			FighterInfo _info = createFighterProp(index, battleSide, type, _aProp);
			_info.addFighterProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE, 1009); // 测试代码很糙请无视
			return _info;
		}

		private static FighterAProperty createTestAProp()
		{
			FighterAProperty _prop = new FighterAProperty();
			for (int _i = 0; _i < FighterAProperty._SIZE; _i++)
			{
				int _val = 0;
				if (FighterAProperty.HP == FighterAProperty.getBattlePropKey(_i))
				{
					_val = 1000;
				}
				else
				{
					_val = 200;
				}
				_prop.set(FighterAProperty.getBattlePropKey(_i), _val);
			}
			return _prop;
		}

		public static FighterInfo createFighterProp(int index, BattleSideEnum side, FighterType type, FighterAProperty prop)
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
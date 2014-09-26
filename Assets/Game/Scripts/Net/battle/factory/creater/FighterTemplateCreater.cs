using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.factory.creater
{


	using FighterInfo = com.kx.sglm.gs.battle.share.data.FighterInfo;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using FighterType = com.kx.sglm.gs.battle.share.enums.FighterType;
	using SkillEffectEnum = com.kx.sglm.gs.battle.share.skill.enums.SkillEffectEnum;
	using RoleAProperty = com.kx.sglm.gs.hero.properties.RoleAProperty;

	public class FighterTemplateCreater
	{

		public static List<FighterInfo> createFighterInfoListFormHeroTemp(List<Template.Auto.Hero.HeroTemplate> heroTemps, BattleSideEnum side)
		{
			List<FighterInfo> _infoList = new List<FighterInfo>();
			int _heroTempSize = heroTemps.Count;
			for (int _i = 0; _i < _heroTempSize; _i++)
			{
				FighterInfo _info = createFighterInfoFromHeroTemp(_i, side, heroTemps[_i]);
				_infoList.Add(_info);
			}
			return _infoList;
		}

		public static FighterInfo createFighterInfoFromHeroTemp(int index, BattleSideEnum side, Template.Auto.Hero.HeroTemplate heroTemp)
		{

			Dictionary<int, int> _aProps = createHeroAPropFromTemp(heroTemp);
			FighterInfo _info = FighterInfoCreater.createFighterProp(index, side, FighterType.HERO, _aProps);
			_info.addNormalProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE, heroTemp.Id);
			_info.addNormalProp(BattleKeyConstants.BATTLE_KEY_HERO_JOB, (int)heroTemp.Job);
			//是在客户端构建的
			_info.addNormalProp(BattleKeyConstants.BATTLE_KEY_HERO_TYPE, BattleConstants.FIGHTER_TYPE_HERO);
			_info.ActiveSkillId = heroTemp.ActiveSkill;
			_info.LeaderSkillId = heroTemp.LeaderSkill;

			List<int> _allSkills = new List<int>();
			_allSkills.AddRange(heroTemp.PassiveSkill);
			_allSkills.Add(heroTemp.SpSkill);
			_allSkills.Add(heroTemp.LeaderSkill);
			_allSkills.Add(heroTemp.ActiveSkill);
			_info.SkillIdList = _allSkills;
			return _info;
		}

		public static void initMonsterIdAndGroup(List<int> allMonsterIdList, List<int> monsterGroupList, List<Template.Auto.Raid.MonsterGroup> allMonster)
		{

			foreach (Template.Auto.Raid.MonsterGroup _monsterGroup in allMonster)
			{
				List<int> _groupMonsterList = getSingleMonsterGroupIdList(_monsterGroup);
				if (_groupMonsterList.Count == 0)
				{
					break;
				}
				allMonsterIdList.AddRange(_groupMonsterList);
				monsterGroupList.Add(_groupMonsterList.Count);
			}
		}

		public static List<int> getSingleMonsterGroupIdList(Template.Auto.Raid.MonsterGroup monsterGroup)
		{
			List<int> _idList = new List<int>();
			addSingleMonsterId(monsterGroup.Arg_A, _idList);
			addSingleMonsterId(monsterGroup.Arg_B, _idList);
			addSingleMonsterId(monsterGroup.Arg_C, _idList);
			return _idList;
		}

		public static void addSingleMonsterId(int monsterId, List<int> monsterList)
		{
			if (monsterId > 0)
			{
				monsterList.Add(monsterId);
			}
		}

		public static List<FighterInfo> createMosnterFighterList(List<Template.Auto.Monster.MonsterTemplate> monterList, BattleSideEnum side)
		{
			List<FighterInfo> _fighterList = new List<FighterInfo>();
			int _size = monterList.Count;
			for (int _i = 0; _i < _size; _i++)
			{
				_fighterList.Add(createMonsterProp(_i, monterList[_i], side, FighterType.MONSTER, _i == monterList.Count - 1));
			}
			return _fighterList;
		}

		public static FighterInfo createMonsterProp(int index, Template.Auto.Monster.MonsterTemplate monsterTemp, BattleSideEnum battleSide, FighterType type, bool boss)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<Integer, Integer> _aProp = createMonsterAProp(monsterTemp, boss);
			Dictionary<int, int> _aProp = createMonsterAProp(monsterTemp, boss);
			FighterInfo _info = FighterInfoCreater.createFighterProp(index, battleSide, type, _aProp);
			_info.addNormalProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE, monsterTemp.Id); // 测试代码很糙请无视
			_info.addNormalProp(BattleKeyConstants.BATTLE_KEY_HERO_JOB, 0);
			_info.addNormalProp(BattleKeyConstants.BATTLE_PROP_MONSTER_AI_ID, monsterTemp.AiID);
			_info.addNormalProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DEFAULT_CD, monsterTemp.CD);
			return _info;
		}

		private static Dictionary<int, int> createMonsterAProp(Template.Auto.Monster.MonsterTemplate monsterTemp, bool boss)
		{
			Dictionary<int, int> _prop = new Dictionary<int, int>();
			_prop[RoleAProperty.HP] = monsterTemp.HP;
			_prop[RoleAProperty.ATK] = monsterTemp.Attack;
			_prop[RoleAProperty.DEFENSE] = monsterTemp.Defense;
			_prop[RoleAProperty.MP] = 0;
			_prop[RoleAProperty.DECRDAMAGE] = (int) BattleConstants.BATTLE_RATIO_BASE;
			_prop[RoleAProperty.RECOVER] = 0;
			return _prop;
		}

		public static List<int> getSingleMonsterSkillFromAI(Template.Auto.Monster.MonsterBattleAITemplate aiTemp)
		{
			List<int> _allSkillIds = new List<int>();
			foreach (Template.Auto.Monster.SkillRatePairData _defaultSkill in aiTemp.DefaultSkills)
			{
				_allSkillIds.Add(_defaultSkill.SkillId);
			}
			// 可能没有AISkill
			if (aiTemp.AiSkills != null)
			{
				foreach (Template.Auto.Monster.MonsterSkillAIData _aiData in aiTemp.AiSkills)
				{
					_allSkillIds.Add(_aiData.SkillId);
				}
			}
			return _allSkillIds;
		}

		public static List<int> getAllBuffIds(int spMapxId, List<int> monsterShieldIds, List<Template.Auto.Skill.HeroBattleSkillTemplate> heroSkill, List<Template.Auto.Skill.MonsterBattleSkillTemplate> monsterSkill)
		{
			List<int> _buffIds = new List<int>();
			_buffIds.Add(spMapxId);
			_buffIds.AddRange(monsterShieldIds);
			foreach (Template.Auto.Skill.HeroBattleSkillTemplate _temp in heroSkill)
			{
				_buffIds.AddRange(getSingleSkillBuffIds(_temp.EffectList));
			}
			foreach (Template.Auto.Skill.MonsterBattleSkillTemplate _temp in monsterSkill)
			{
				_buffIds.AddRange(getSingleSkillBuffIds(_temp.EffectList));
			}

			return _buffIds;
		}

		private static List<int> getSingleSkillBuffIds(List<Template.Auto.Skill.SkillBattleEffectData> skillEffectList)
		{
			List<int> _skillBuff = new List<int>();
			foreach (Template.Auto.Skill.SkillBattleEffectData _effect in skillEffectList)
			{
				if (_effect.BattleEffectType == SkillEffectEnum.BUFF_ADD.Index || _effect.BattleEffectType == SkillEffectEnum.DEBUFF_ADD.Index)
				{
					string[] _buffIds = _effect.BattleEffectParam1.Split(',');
					foreach (string _buffId in _buffIds)
					{
						addToBuffIds(_buffId, _skillBuff);
					}
				}
			}
			return _skillBuff;
		}

		protected internal static void addToBuffIds(string buffIdInt, List<int> buffIds)
		{
			int buffId = Convert.ToInt32(buffIdInt);
			if (buffId == 0)
			{
				return;
			}
			buffIds.Add(buffId);
		}

		public static Dictionary<int, int> createHeroAPropFromTemp(Template.Auto.Hero.HeroTemplate heroTemp)
		{
			Dictionary<int, int> _addProps = new Dictionary<int, int>();
			_addProps[RoleAProperty.ATK] = heroTemp.Attack;
			_addProps[RoleAProperty.HP] = heroTemp.HP;
			_addProps[RoleAProperty.MP] = heroTemp.MP;
			_addProps[RoleAProperty.RECOVER] = heroTemp.Recover;
			_addProps[RoleAProperty.LUCY] = heroTemp.Lucky;
			return _addProps;
		}

	}

}
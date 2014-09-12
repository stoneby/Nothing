using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.factory.creater
{


	using BattleSource = com.kx.sglm.gs.battle.share.data.BattleSource;
	using FighterInfo = com.kx.sglm.gs.battle.share.data.FighterInfo;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using BattleType = com.kx.sglm.gs.battle.share.enums.BattleType;
	using FighterType = com.kx.sglm.gs.battle.share.enums.FighterType;
	using BattleMsgMonster = KXSGCodec.BattleMsgMonster;

	public class BattleSourceTemplateCreater
	{

		private IBattleTemplateService tmplService;
		private BattleSource battleSource;

		public BattleSourceTemplateCreater(IBattleTemplateService tmplService)
		{
			this.tmplService = tmplService;
		}

		public virtual BattleSource createPVESource(KXSGCodec.SCBattlePveStartMsg msg)
		{

			battleSource = createBaseSourceInfo(msg);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<com.kx.sglm.gs.battle.share.data.FighterInfo> _heroList = FighterInfoCreater.createListFromMsgHero(com.kx.sglm.gs.battle.share.enums.BattleSideEnum.SIDE_LEFT, msg.getFighterList());
			List<FighterInfo> _heroList = FighterInfoCreater.createListFromMsgHero(BattleSideEnum.SIDE_LEFT, msg.FighterList);
			battleSource.FighterProp = _heroList;

			Template.Auto.Raid.RaidMonsterGroupTemplate _monsterGroupTemplate = RaidMonsterGropTemp;
			if (_monsterGroupTemplate == null)
			{
				return battleSource;
			}
			initMonsterTemplateInfoForSource(_monsterGroupTemplate, msg.MonsterList);

			initBattleSkillService();

			return battleSource;
		}

		public virtual BattleSource createBaseSourceInfo(KXSGCodec.SCBattlePveStartMsg msg)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.share.enums.BattleType _type = com.kx.sglm.gs.battle.share.enums.BattleType.getValue(msg.getBattleType());
			BattleType _type = BattleType.getValue(msg.BattleType);
			BattleSource _source = new BattleSource(_type);
			_source.Uuid = msg.Uuid;
			_source.SpMaxBuffId = msg.SpMaxBuffId;
			_source.RaidStageId = msg.RaidID;
			return _source;
		}

		public virtual Template.Auto.Raid.RaidMonsterGroupTemplate RaidMonsterGropTemp
		{
			get
			{
				Template.Auto.Raid.RaidMonsterGroupTemplate _monsterGroupTemplate = null;
				Template.Auto.Raid.RaidStageTemplate _stageTmpl = tmplService.getRaidStageTemp(battleSource.RaidStageId);
				if (_stageTmpl == null)
				{
					return _monsterGroupTemplate;
				}
				_monsterGroupTemplate = tmplService.getRaidMonsterGroupTemp(_stageTmpl.MonsterGroupId);
				return _monsterGroupTemplate;
			}
		}


		public virtual void initMonsterTemplateInfoForSource(Template.Auto.Raid.RaidMonsterGroupTemplate monsterGroup, List<BattleMsgMonster> msgMonsterList)
		{
			Dictionary<int, Template.Auto.Monster.MonsterTemplate> allMosnters = tmplService.AllMonsterMap;
			List<Template.Auto.Raid.MonsterGroup> _allMonster = monsterGroup.MonsterGroup;

			List<int> _allMonsterIdList = new List<int>();
			List<int> _monsterGroupList = new List<int>();

			//生成基础数据列表
			FighterTemplateCreater.initMonsterIdAndGroup(_allMonsterIdList, _monsterGroupList, _allMonster);
			//获取怪物模板
			List<Template.Auto.Monster.MonsterTemplate> _template = getAllBattleMonsterTemp(_allMonsterIdList, allMosnters);
			//生成基础怪物数据
			List<FighterInfo> _monsterFighterList = FighterTemplateCreater.createMosnterFighterList(_template, BattleSideEnum.SIDEB_RIGHT);

			FighterInfoCreater.initDropMapFromMsgMonster(_monsterFighterList, msgMonsterList);

			FighterInfoCreater.initFighterIndexFromMonsterGrop(BattleSideEnum.SIDEB_RIGHT, _monsterGroupList, _monsterFighterList);

			battleSource.MonsterGroup = _monsterGroupList;

			initMonsterFighterInfoAIID(_monsterFighterList);

			battleSource.addFighterProp(_monsterFighterList);

		}

		public virtual void initMonsterFighterInfoAIID(List<FighterInfo> monsterFighter)
		{
			Dictionary<int, Template.Auto.Monster.MonsterBattleAITemplate> _allAiTemp = tmplService.AllMonsterAIMap;
			foreach (FighterInfo _info in monsterFighter)
			{
				Template.Auto.Monster.MonsterBattleAITemplate _aiTemp = _allAiTemp[_info.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_AI_ID)];
				if (_aiTemp == null)
				{
					// TODO: loggers.error
					continue;
				}

				initSingleMonsterFighterFromAI(_info, _aiTemp);
			}

		}

		protected internal virtual void initSingleMonsterFighterFromAI(FighterInfo info, Template.Auto.Monster.MonsterBattleAITemplate aiTemp)
		{
			List<int> _allSkillId = FighterTemplateCreater.getSingleMonsterSkillFromAI(aiTemp);
			info.SkillIdList = _allSkillId;
			info.addNormalProp(BattleKeyConstants.BATTLE_PROP_MONSTER_SHIELD_ID, aiTemp.ShieldBuffId);
		}

		public virtual List<Template.Auto.Monster.MonsterTemplate> getAllBattleMonsterTemp(List<int> monsterList, Dictionary<int, Template.Auto.Monster.MonsterTemplate> allMosnter)
		{
			List<Template.Auto.Monster.MonsterTemplate> _tempList = new List<Template.Auto.Monster.MonsterTemplate>();
			foreach (int _monsterId in monsterList)
			{
				if (allMosnter.ContainsKey(_monsterId))
				{
					_tempList.Add(allMosnter[_monsterId]);
				}
			}
			return _tempList;
		}

		public virtual void initBattleSkillService()
		{
			BattleActionService _service = BattleActionService.Service;
			_service.SpMaxBuffId = battleSource.SpMaxBuffId;
			_service.initNormalAction();
			List<Template.Auto.Skill.HeroBattleSkillTemplate> _heroSkill = AllSkillTemplate;
			List<Template.Auto.Skill.MonsterBattleSkillTemplate> _monsterSkill = AllMonsterSkillTemplate;
			List<Template.Auto.Buff.BattleBuffTemplate> _buffTemp = getAllBuffTemplate(_heroSkill, _monsterSkill);
			List<Template.Auto.Monster.MonsterBattleAITemplate> _aiTemp = AllMonsterAITemplate;
			_service.initTemplateHeroSkillAction(_heroSkill);
			_service.initTemplateMonsterSkillAction(_monsterSkill);
			_service.initAllMonsterAI(_aiTemp);
			_service.initTemplateBuffAction(_buffTemp);
		}

		public virtual List<int> createAllHeroSkillIds()
		{
			return createFighterInfoSkillList(battleSource, FighterType.HERO);
		}

		public virtual List<int> createAllMonsterSkillIds()
		{
			return createFighterInfoSkillList(battleSource, FighterType.MONSTER);
		}

		private List<int> createFighterInfoSkillList(BattleSource battleSource, FighterType fighterType)
		{
			List<FighterInfo> _allTypeFighter = battleSource.getTypeFighter(fighterType);
			List<int> _allSkill = new List<int>();
			foreach (FighterInfo _info in _allTypeFighter)
			{
				_allSkill.AddRange(_info.SkillIdList);
			}
			return _allSkill;
		}

		public virtual List<Template.Auto.Skill.HeroBattleSkillTemplate> AllSkillTemplate
		{
			get
			{
				List<int> _heroSkillList = createAllHeroSkillIds();
				Dictionary<int, Template.Auto.Skill.HeroBattleSkillTemplate> _skillMap = tmplService.AllHeroSkillMap;
				return getAllTempFromList(_heroSkillList, _skillMap);
			}
		}

		public virtual List<Template.Auto.Skill.MonsterBattleSkillTemplate> AllMonsterSkillTemplate
		{
			get
			{
				Dictionary<int, Template.Auto.Skill.MonsterBattleSkillTemplate> skillMap = tmplService.AllMonsterSkillMap;
				List<int> _monsterSkillList = createAllMonsterSkillIds();
				return getAllTempFromList(_monsterSkillList, skillMap);
			}
		}

		public virtual List<Template.Auto.Buff.BattleBuffTemplate> getAllBuffTemplate(List<Template.Auto.Skill.HeroBattleSkillTemplate> heroSkill, List<Template.Auto.Skill.MonsterBattleSkillTemplate> monsterSkill)
		{
			List<int> _shieldIds = battleSource.getFighterPropList(BattleKeyConstants.BATTLE_PROP_MONSTER_SHIELD_ID, true);
			List<int> _allBuffIds = FighterTemplateCreater.getAllBuffIds(battleSource.SpMaxBuffId, _shieldIds, heroSkill, monsterSkill);
			return getAllTempFromList(_allBuffIds, tmplService.AllBuffMap);
		}

		public virtual List<Template.Auto.Monster.MonsterBattleAITemplate> AllMonsterAITemplate
		{
			get
			{
				List<int> _aiIds = battleSource.getFighterPropList(BattleKeyConstants.BATTLE_PROP_MONSTER_AI_ID, true);
				return getAllTempFromList(_aiIds, tmplService.AllMonsterAIMap);
			}
		}

		public virtual List<T> getAllTempFromList<T>(List<int> idList, Dictionary<int, T> allTempMap)
		{
			List<T> _list = new List<T>();
			foreach (int _id in idList)
			{
				if (allTempMap.ContainsKey(_id))
				{
					_list.Add(allTempMap[_id]);
				}
			}
			return _list;
		}

	}

}
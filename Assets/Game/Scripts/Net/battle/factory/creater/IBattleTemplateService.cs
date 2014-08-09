using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.factory.creater
{


	public interface IBattleTemplateService
	{

		Template.Auto.Raid.RaidStageTemplate getRaidStageTemp(int stageId);

		Template.Auto.Raid.RaidMonsterGroupTemplate getRaidMonsterGroupTemp(int monsterGroupId);

		Dictionary<int, Template.Auto.Monster.MonsterTemplate> AllMonsterMap {get;}

		Dictionary<int, Template.Auto.Monster.MonsterBattleAITemplate> AllMonsterAIMap {get;}

		Dictionary<int, Template.Auto.Skill.HeroBattleSkillTemplate> AllHeroSkillMap {get;}

		Dictionary<int, Template.Auto.Skill.MonsterBattleSkillTemplate> AllMonsterSkillMap {get;}

		Dictionary<int, Template.Auto.Buff.BattleBuffTemplate> AllBuffMap {get;}

	}

}
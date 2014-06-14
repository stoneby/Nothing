using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.ai
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;

	public class MonsterAI
	{

		private List<SkillRoulettePair> allDefaultSkill;

		private List<SkillAIHolder> allAISkill;

		public MonsterAI()
		{
			allAISkill = new List<SkillAIHolder>();
			allDefaultSkill = new List<SkillRoulettePair>();
		}

		protected internal virtual void sortAISkills()
		{
			allAISkill.Sort();

		}


		// TODO: add param
		public virtual int calcCurSkill()
		{
			List<SkillRoulettePair> _skillList = calcCanOpSkills();
			SkillRoulettePair _resultSkill = MathUtils.rolette(_skillList);
			int _skillId = _resultSkill == null ? 0 : _resultSkill.SkillId;
			return _skillId;
		}

		// TODO: add param
		protected internal virtual List<SkillRoulettePair> calcCanOpSkills()
		{
			List<SkillRoulettePair> _canOpSkill = new List<SkillRoulettePair>();
			foreach (SkillAIHolder _holder in allAISkill)
			{
				_canOpSkill.AddRange(_holder.CanOptionSkill);
				if (_canOpSkill.Count > 0)
				{
					break;
				}
			}
			if (_canOpSkill.Count == 0)
			{
				_canOpSkill.AddRange(allDefaultSkill);
			}
			return _canOpSkill;
		}

		public virtual List<SkillAIHolder> AllAISkill
		{
			set
			{
				this.allAISkill = value;
			}
			get
			{
				return allAISkill;
			}
		}

		public virtual List<SkillRoulettePair> AllDefaultSkill
		{
			set
			{
				this.allDefaultSkill = value;
			}
			get
			{
				return allDefaultSkill;
			}
		}



	}

}
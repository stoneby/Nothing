using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.ai
{


	using com.kx.sglm.gs.battle.share.model;

	public class SkillAIHolder : ComparableListHolder<SkillAI>
	{

		public SkillAIHolder(int priority) : base(priority)
		{
			Asc = false;
		}

		//TODO: 加上参数
		public virtual List<SkillRoulettePair> CanOptionSkill
		{
			get
			{
				List<SkillRoulettePair> _resultList = new List<SkillRoulettePair>();
				List<SkillAI> _aiList = CanOpSkillAi;
				foreach (SkillAI _ai in _aiList)
				{
					_resultList.Add(_ai.getSkillInfo());
				}
				return _resultList;
			}
		}

		//TODO: 加上参数
		protected internal virtual List<SkillAI> CanOpSkillAi
		{
			get
			{
				return HoldList;
			}
		}



	}

}
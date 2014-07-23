using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.ai
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using MonsterShield = com.kx.sglm.gs.battle.share.buff.effect.MonsterShield;

	public class MonsterAI
	{

		private int aiId;

		private int shieldBuffId;

		private List<SkillRoulettePair> allDefaultSkill;

		private List<SkillAIHolder> allAISkill;

		public MonsterAI()
		{
			allAISkill = new List<SkillAIHolder>();
			allDefaultSkill = new List<SkillRoulettePair>();
		}

		protected internal virtual void sortAISkills()
		{
			allAISkill.Sort(new ComparatorAnonymousInnerClassHelper(this));
		}

		private class ComparatorAnonymousInnerClassHelper : IComparer<SkillAIHolder>
		{
			private readonly MonsterAI outerInstance;

			public ComparatorAnonymousInnerClassHelper(MonsterAI outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual int Compare(SkillAIHolder o1, SkillAIHolder o2)
			{
				return o1.compareTo(o2);
			}
		}


		public virtual int calcCurSkill(BattleFighter attacker)
		{
			List<SkillRoulettePair> _skillList = calcCanOpSkills(attacker);
			SkillRoulettePair _resultSkill = MathUtils.rolette(_skillList);
			int _skillId = _resultSkill == null ? 0 : _resultSkill.SkillId;
			return _skillId;
		}

		protected internal virtual List<SkillRoulettePair> calcCanOpSkills(BattleFighter attacker)
		{
			List<SkillRoulettePair> _canOpSkill = new List<SkillRoulettePair>();
			foreach (SkillAIHolder _holder in allAISkill)
			{
				_canOpSkill.AddRange(_holder.getCanOptionSkill(attacker));
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

		public virtual int AiId
		{
			get
			{
				return aiId;
			}
			set
			{
				this.aiId = value;
			}
		}


		public virtual List<SkillAIHolder> AllAISkill
		{
			set
			{
				this.allAISkill = value;
				sortAISkills();
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



		public virtual int ShieldBuffId
		{
			set
			{
				this.shieldBuffId = value;
			}
			get
			{
				return shieldBuffId;
			}
		}


		public virtual bool hasShield()
		{
			return ShieldBuffId != 0;
		}

		public virtual MonsterShield MonsterShield
		{
			get
			{
				//TODO: 这里用了强转，很不好，以后修改
				return !hasShield() ? null : (MonsterShield)BattleActionService.Service.getBuffAction(shieldBuffId);
			}
		}



	}

}
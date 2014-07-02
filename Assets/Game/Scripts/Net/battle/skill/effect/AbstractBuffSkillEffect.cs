using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.effect
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	public abstract class AbstractBuffSkillEffect : AbstractSkillEffect
	{

		private List<int> buffIds;

		public AbstractBuffSkillEffect(bool enemyEffect) : base(enemyEffect)
		{
			buffIds = new List<int>();
		}

		protected internal override void initRecord(BattleFighter attacker, BattleFightRecord fightRecord)
		{
			// TODO add record

		}

		protected internal override void onSingleAction(BattleFighter attacker, BattleFighter defencer, SkillDataHolder resultData)
		{
			foreach (int _buffId in buffIds)
			{
				defencer.addBuff(_buffId);
			}
		}

		public override void build(params string[] param)
		{
			for (int _i = 0; _i < param.Length; _i++)
			{
				int _buffId = Convert.ToInt32(param[_i]);
				if (_buffId > 0)
				{
					buffIds.Add(_buffId);
				}
			}
		}

	}

}
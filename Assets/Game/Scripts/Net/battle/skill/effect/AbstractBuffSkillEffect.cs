using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.effect
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BuffInfo = com.kx.sglm.gs.battle.share.buff.BuffInfo;
	using IBuffCondition = com.kx.sglm.gs.battle.share.buff.condition.IBuffCondition;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using SkillActionCreater = com.kx.sglm.gs.battle.share.skill.creater.SkillActionCreater;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	public abstract class AbstractBuffSkillEffect : AbstractSkillEffect
	{

		private List<int> buffIds;
		private IBuffCondition buffCondition;

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
				defencer.addBuff(new BuffInfo(_buffId, buffCondition));
			}
		}

		public override void build(params string[] param)
		{
			string[] _buffIds = param[0].Split(",", true);
			for (int _i = 0; _i < _buffIds.Length; _i++)
			{
				int _buffId = Convert.ToInt32(_buffIds[_i]);
				if (_buffId > 0)
				{
					buffIds.Add(_buffId);
				}
			}
			string[] _conditionStr = param[1].Split(",", true);
			if (Convert.ToInt32(_conditionStr[0]) > 0)
			{
				buffCondition = SkillActionCreater.createBuffAction(param[1]);
			}
		}

	}

}
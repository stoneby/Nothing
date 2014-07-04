using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	public abstract class AbstractSkillEffect : ISkillEffect
	{

		private bool enemyEffect;

		private float ratio;

		public AbstractSkillEffect(bool enemyEffect)
		{
			this.enemyEffect = enemyEffect;
		}

		public override void onAction(BattleFighter attacker, List<BattleFighter> defencerList, SkillDataHolder resultData)
		{
			initRecord(attacker, resultData.Record);
			foreach (BattleFighter _defencer in defencerList)
			{
				onSingleAction(attacker, _defencer, resultData);
			}

		}

		public override float Ratio
		{
			set
			{
				this.ratio = value;
			}
			get
			{
				return ratio;
			}
		}


		protected internal abstract void initRecord(BattleFighter attacker, BattleFightRecord fightRecord);

		protected internal abstract void onSingleAction(BattleFighter attacker, BattleFighter defencer, SkillDataHolder resultData);

		public override bool EnemyEffect
		{
			get
			{
				return enemyEffect;
			}
		}

	}

}
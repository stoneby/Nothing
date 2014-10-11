using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	public abstract class AbstractRecoverEffect : AbstractSkillEffect
	{

		public AbstractRecoverEffect() : base(false)
		{
		}


		protected internal abstract float recoverOption(BattleFighter attacker, BattleFighter defencer);

		protected internal override void initRecord(BattleFighter attacker, BattleFightRecord fightRecord)
		{
			BattleRecordHelper.initBattleFight(BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVER, attacker, fightRecord);
		}

		protected internal abstract void recoverRecord(BattleFighter attacker, BattleFighter defencer, SkillDataHolder resultData);

		protected internal override void onSingleAction(BattleFighter attacker, BattleFighter defencer, SkillDataHolder resultData)
		{
			recoverOption(attacker, defencer);
			recoverRecord(attacker, defencer, resultData);
		}

		public override void defencerAfterEffect(BattleFighter attacker, List<BattleFighter> defencerList, BattleFightRecord record)
		{
			//cur do nothing
		}

	}






}
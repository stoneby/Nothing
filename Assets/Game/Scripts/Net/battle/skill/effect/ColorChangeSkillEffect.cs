using System;

namespace com.kx.sglm.gs.battle.share.skill.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;
	using SkillDataHolder = com.kx.sglm.gs.battle.share.skill.model.SkillDataHolder;

	public class ColorChangeSkillEffect : AbstractSkillEffect
	{

		public ColorChangeSkillEffect() : base(false)
		{
		}

		private int colorIndex;


		public override void build(params string[] param)
		{
			this.colorIndex = Convert.ToInt32(param[0]);
		}

		protected internal override void initRecord(BattleFighter attacker, BattleFightRecord fightRecord)
		{
			BattleFightRecord _record = fightRecord;
			BattleRecordHelper.initBattleFight(BattleRecordConstants.SINGLE_ACTION_TYPE_CHANGE_COLOR, attacker, _record);
		}

		protected internal override void onSingleAction(BattleFighter attacker, BattleFighter defencer, SkillDataHolder resultData)
		{
			HeroColor _color = HeroColor.getValue(colorIndex);
			BattleFightRecord _record = resultData.Record;
			SingleActionRecord _singleRecord = _record.OrCreateDefence;
			_singleRecord.ActType = BattleRecordConstants.SINGLE_ACTION_TYPE_CHANGED_COLOR;
			BattleRecordHelper.initSingleRecord(defencer, _singleRecord);
			defencer.changeColor(_color, _singleRecord);
			_record.finishCurDefecner();
		}

	}

}
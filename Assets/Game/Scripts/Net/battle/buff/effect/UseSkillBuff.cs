using System;

namespace com.kx.sglm.gs.battle.share.buff.effect
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleTeamFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;


	/// <summary>
	/// 使用技能的buff，持续每回合使用一个技能
	/// @author liyuan2
	/// 
	/// </summary>
	public class UseSkillBuff : AbstractBuffEffect
	{

		private int skillId;

		public override FighterStateEnum StateEnum
		{
			get
			{
				return FighterStateEnum.NORMAL_STATE;
			}
		}

		public override void onActive(BattleFighter fighter)
		{

		}

		public override void onRemove(BattleFighter fighter)
		{

		}

		public override void onTeamBeforeAttack(BattleFighter fighter)
		{
			//TODO: 这里要优化和调试，战斗Record还是有些不顺
			BattleTeamFightRecord _teamRecord = fighter.Battle.Record.OrCreateTeamFighterRecord;

			BattleFightRecord _record = _teamRecord.OrCreateRecord;

			fighter.SkillManager.useSkill(skillId, _record);

			_teamRecord.finishCurRecord();
		}

		public override void onAttack(BattleFighter attacker)
		{

		}

		public override void onDefence(BattleFighter attacker, BattleFighter owner)
		{

		}

		public override bool needShow(BattleFighterBuff buffInst)
		{
			return false;
		}

		public override void build(params string[] param)
		{
			this.skillId = Convert.ToInt32(param[0]);
		}

	}

}
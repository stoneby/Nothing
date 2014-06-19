namespace com.kx.sglm.gs.battle.share.skill.action
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;

	/// <summary>
	/// 怪物技能动作
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BaseMonsterSkillAction : AbstractSingletonBattleAction
	{

		private int extraCD;

		/// <summary>
		/// 蓄力回合描述 </summary>
		private string extraCDDesc;

		public virtual int ExtraCD
		{
			get
			{
				return extraCD;
			}
			set
			{
				this.extraCD = value;
			}
		}


		public virtual string ExtraCDDesc
		{
			get
			{
				return extraCDDesc;
			}
			set
			{
				this.extraCDDesc = value;
			}
		}


		public virtual bool hasExtraRound()
		{
			return ExtraCD > 0;
		}

		public override bool canOption(BattleFighter attacker)
		{
			return true;
		}

	}

}
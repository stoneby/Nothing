using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.model
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;

	/// <summary>
	/// 技能内一些数据
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class SkillDataHolder
	{

		private BattleFightRecord record;
		private BattleFighter attacker;
		private List<BattleFighter> enemyList;
		private List<BattleFighter> friendList;
		private List<ISkillEffect> activeEffect;


		public virtual List<BattleFighter> getTargets(ISkillEffect effect)
		{
			return effect.EnemyEffect ? enemyList : friendList;
		}

		public virtual BattleFighter Attacker
		{
			get
			{
				return attacker;
			}
			set
			{
				this.attacker = value;
			}
		}


		public virtual List<BattleFighter> EnemyList
		{
			get
			{
				return enemyList;
			}
			set
			{
				this.enemyList = value;
			}
		}


		public virtual List<BattleFighter> FriendList
		{
			get
			{
				return friendList;
			}
			set
			{
				this.friendList = value;
			}
		}


		public virtual BattleFightRecord Record
		{
			set
			{
				this.record = value;
			}
			get
			{
				return record;
			}
		}


		public virtual List<ISkillEffect> ActiveEffect
		{
			get
			{
				return activeEffect;
			}
			set
			{
				this.activeEffect = value;
			}
		}



	}

}
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.buff
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BuffPolicyEnum = com.kx.sglm.gs.battle.share.buff.enums.BuffPolicyEnum;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;

	/// <summary>
	/// a holder for type, all buff in the holder have same typeA
	/// @author liyuan2
	/// 
	/// </summary>
	public class BuffTypeHolder
	{

		private int typeA;

		private Dictionary<int, BattleFighterBuff> buffMap;

		public BuffTypeHolder(int typeA)
		{
			this.buffMap = new Dictionary<int, BattleFighterBuff>();
			this.typeA = typeA;
		}


		public virtual BattleFighterBuff putBuff(BattleFighter owner, IBuffAction buffAction)
		{
			BattleFighterBuff _buff = createBattleFighterBuff(owner, buffAction);
			buffMap[buffAction.TypeB] = _buff;
			_buff.stackingBuff();
			return _buff;
		}


		public virtual bool hasSameBuff(IBuffAction buffAction)
		{
			return buffMap.ContainsKey(buffAction.TypeB);
		}

		public virtual int TypeA
		{
			get
			{
				return typeA;
			}
		}

		public virtual void removeBuff(IBuffAction buffAction)
		{
			this.buffMap.Remove(buffAction.TypeB);
		}

		public virtual void activeBuff()
		{
			foreach (BattleFighterBuff _buff in values())
			{
				_buff.activeBuff();
			}
		}


		public virtual void onTeamBeforeAttack()
		{
			foreach (BattleFighterBuff _buff in values())
			{
				_buff.onTeamBeforeAttack();
			}
		}


		public virtual void countDownRound(BattleRoundCountRecord record)
		{
			foreach (BattleFighterBuff _buff in values())
			{
				_buff.countDown(record);
			}

		}

		public virtual BattleFighterBuff getBuff(int typeB)
		{
			return buffMap[typeB];
		}

		protected internal virtual BattleFighterBuff createBattleFighterBuff(BattleFighter owner, IBuffAction buffAction)
		{
			BattleFighterBuff _buff = new BattleFighterBuff(owner, buffAction);
			return _buff;
		}

		protected internal virtual BuffPolicyEnum calcPolicy(IBuffAction buffAction)
		{
			return hasSameBuff(buffAction) ? BuffPolicyEnum.COVER : BuffPolicyEnum.OVERLAY;
		}

		public virtual int size()
		{
			return buffMap.Count;
		}

		public virtual ICollection<BattleFighterBuff> values()
		{
			return buffMap.Values;
		}

		public virtual bool Empty
		{
			get
			{
				return buffMap.Count == 0;
			}
		}

		public virtual ICollection<int> BuffIds
		{
			get
			{
				HashSet<int> _ids = new HashSet<int>();
				foreach (BattleFighterBuff _buff in buffMap.Values)
				{
					_ids.Add(_buff.BuffId);
				}
				return _ids;
			}
		}

	}

}
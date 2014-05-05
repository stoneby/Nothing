using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.actor.impl
{


	using IBattleBuffManager = com.kx.sglm.gs.battle.buff.IBattleBuffManager;
	using FighterInfo = com.kx.sglm.gs.battle.data.FighterInfo;
	using BattleFightRecord = com.kx.sglm.gs.battle.data.record.BattleFightRecord;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.data.record.BattleRoundCountRecord;
	using BattleSideEnum = com.kx.sglm.gs.battle.enums.BattleSideEnum;
	using FighterState = com.kx.sglm.gs.battle.enums.FighterState;
	using AbstractSingletonAttackAction = com.kx.sglm.gs.battle.singleton.AbstractSingletonAttackAction;
	using IBattleSkillManager = com.kx.sglm.gs.battle.skill.IBattleSkillManager;
	using FighterAProperty = com.kx.sglm.gs.battle.utils.FighterAProperty;

	/// <summary>
	/// 战斗武将，包括Monster和Hero
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleFighter : IBattleSideActor
	{

		/// <summary>
		/// 战斗武将的位置，初始化后不再变化
		/// </summary>
		protected internal int index;
		/// <summary>
		/// 当前血量
		/// </summary>
		protected internal int curHp;
		/// <summary>
		/// 当前攻击
		/// </summary>
		protected internal int attack;
		/// <summary>
		/// 当前回复
		/// </summary>
		protected internal int recover;
		/// <summary>
		/// 当前气力
		/// </summary>
		protected internal int sp;
		/// <summary>
		/// 当前防御
		/// </summary>
		protected internal int defence;
		/// <summary>
		/// 当前免伤
		/// </summary>
		protected internal int damageFree;
		/// <summary>
		/// 职业
		/// </summary>
		protected internal int job;
		/// <summary>
		/// 武将名字
		/// </summary>
		protected internal string name;

		protected internal bool dead;

		protected internal readonly FighterInfo baseProp;

		protected internal BattleTeam ownerTeam;

		protected internal IBattleBuffManager buffManager;

		protected internal IBattleSkillManager skillManager;

		protected internal IList<FighterState> stateList;

		public BattleFighter(BattleTeam ownerTeam, FighterInfo baseProp)
		{
			this.baseProp = baseProp;
			this.dead = false;
			this.stateList = new List<FighterState>();
		}


		public virtual IBattleSkillManager SkillManager
		{
			set
			{
				this.skillManager = value;
			}
			get
			{
				return skillManager;
			}
		}


		public virtual bool canAttack()
		{
			bool _canAttack = !dead;
			if (_canAttack)
			{
				_canAttack = skillManager.canAttack();
			}
			foreach (FighterState _state in stateList)
			{
					if (!_canAttack)
					{
						break;
					}
					_canAttack = _state.FightAble;
			}
			return _canAttack;

		}

		public virtual void afterAttack(BattleFightRecord record)
		{
			skillManager.afterAttack(record);
		}

		public virtual void afterDefence(BattleFightRecord record)
		{

		}

		public virtual void onCostHp(int costHp)
		{
			this.curHp -= costHp;
		}

		public virtual int CurHp
		{
			get
			{
				return curHp;
			}
			set
			{
				this.curHp = value;
			}
		}

		public virtual BattleTeam OwnerTeam
		{
			get
			{
				return ownerTeam;
			}
			set
			{
				this.ownerTeam = value;
			}
		}

		public virtual int AttackVal
		{
			get
			{
				return attack;
			}
		}

		public virtual int DamageFree
		{
			get
			{
				return damageFree;
			}
			set
			{
				this.damageFree = value;
			}
		}

		public virtual void resetOnNewAction()
		{
			// TODO: reset prop
		}

		public virtual bool Alive
		{
			get
			{
				return CurHp > 0;
			}
		}

		public virtual int Defence
		{
			get
			{
				return defence;
			}
			set
			{
				this.defence = value;
			}
		}


		public virtual int Job
		{
			get
			{
				return job;
			}
			set
			{
				this.job = value;
			}
		}


		public virtual void addBuff()
		{
			// TODO: add buff logic
		}

		public virtual IBattleBuffManager BuffManager
		{
			get
			{
				return buffManager;
			}
			set
			{
				this.buffManager = value;
			}
		}


		public virtual AbstractSingletonAttackAction FightAction
		{
			get
			{
				return skillManager.FightAction;
			}
		}

		public virtual int getFighterOtherProp(int key)
		{
			return baseProp.getIntProp(key);
		}

		public virtual bool Dead
		{
			get
			{
				return dead;
			}
			set
			{
				this.dead = value;
			}
		}



		public virtual int TotalHp
		{
			get
			{
				return baseProp.BattleProperty.get(FighterAProperty.HP);
			}
		}



		public virtual void onDead()
		{
			// TODO Auto-generated method stub

		}

		public virtual Battle Battle
		{
			get
			{
				return ownerTeam.Battle;
			}
		}

		public virtual void tryDead()
		{
			if (curHp == 0)
			{
				Dead = true;
			}
		}

		public virtual BattleSideEnum BattleSide
		{
			get
			{
				return ownerTeam.BattleSide;
			}
		}

		public virtual void onRoundFinish(BattleRoundCountRecord roundRecord)
		{
			skillManager.countDownRound(roundRecord);
	//		buffManager.countDownRound();//TODO: 暂时没有BUFF
		}

		public virtual int Side
		{
			get
			{
				return OwnerTeam.BattleSide.Index;
			}
		}

		public virtual int Index
		{
			get
			{
				return index;
			}
			set
			{
				this.index = value;
			}
		}


		public virtual int Attack
		{
			get
			{
				return attack;
			}
			set
			{
				this.attack = value;
			}
		}


		public virtual int Recover
		{
			get
			{
				return recover;
			}
			set
			{
				this.recover = value;
			}
		}


		public virtual int Sp
		{
			get
			{
				return sp;
			}
			set
			{
				this.sp = value;
			}
		}


		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				this.name = value;
			}
		}


		public virtual IList<FighterState> StateList
		{
			get
			{
				return stateList;
			}
			set
			{
				this.stateList = value;
			}
		}


		public virtual FighterInfo BaseProp
		{
			get
			{
				return baseProp;
			}
		}








	}

}
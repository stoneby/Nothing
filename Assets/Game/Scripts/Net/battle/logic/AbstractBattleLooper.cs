using System;

namespace com.kx.sglm.gs.battle.share.logic
{

	using IBattleActor = com.kx.sglm.gs.battle.share.actor.IBattleActor;
	using BattleRecord = com.kx.sglm.gs.battle.share.data.record.BattleRecord;
	using BattleState = com.kx.sglm.gs.battle.share.enums.BattleState;

	/// <summary>
	/// �������߼���ѭ���屾�������� <br>
	/// ���䱾��Ҳ�п����Ǳ�ѭ���Ķ���
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class AbstractBattleLooper<T, A> : IBattleLoop where T : IBattleLoop where A : com.kx.sglm.gs.battle.share.actor.IBattleActor
	{
		public abstract void createDeadth();

		private Battle battle;

		private T curSubAction;

		private A curAttacker;

		private A curDefencer;

		private BattleState curState;

		private bool finished;

		private bool firstEnter;

		/// <summary>
		/// ѭ�������ļ���
		/// </summary>
		private int loopCount;

		public AbstractBattleLooper(Battle battle)
		{
			this.battle = battle;
			this.curState = BattleState.RUNTIME;
			this.firstEnter = true;
		}

		public AbstractBattleLooper(Battle battle, A curAttacker) : this(battle)
		{
			this.curAttacker = curAttacker;
		}

		public AbstractBattleLooper(Battle battle, A curAttacker, A curDefencer) : this(battle)
		{
			this.curAttacker = curAttacker;
			this.curDefencer = curDefencer;
		}

		// @Override
		public virtual void onAction()
		{
			// �����ע����ʵ���Ӳ�����ѭ���߼�
			onFirstEnter();

			// ѭ���Ӷ���
			loopSubAction();

			// ���������Ϊ��������ѭ������˵�����Action������
			if (!HangUp)
			{
				setFinish();
				onFinish();
			}
		}

		/// <summary>
		/// �״ν��붯���ĳ�ʼ�����������Ƿǹ���״̬<br>
		/// ��Ϊ����ǹ���״̬���ܶ���Ϣ��û�е�
		/// </summary>
		protected internal virtual void onFirstEnter()
		{
			if (!firstEnter)
			{
				return;
			}
			if (HangUp)
			{
				return;
			}
			onStart();
			createNewSubAction();
			firstEnter = false;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Console.WriteLine("First enter: " + this.GetType().FullName);
		}

		/// <summary>
		/// ִ��ѭ������
		/// </summary>
		protected internal virtual void loopSubAction()
		{

			bool _dead = false;
			// ������ѭ���ڵ�һ��ֱ��Action����Ϊ����𣬲���ѭ����ʼ��create
			while (true)
			{
				if (curSubAction == null)
				{
					// TODO: do you need log?
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Logger.Log(string.Format("#AbstractBattleLooper.onAction.curSubAction == null,className = {0}", this.GetType().FullName));
					break;
				}
				if (HangUp)
				{
					break;
				}

				// ִ�ж���
				optionSubAction();
				if (HangUp)
				{
					break;
				}
				// ����Զ���������������
				if (curSubAction.Dead)
				{
					createDeadth();
					_dead = Dead;
				}
				if (_dead)
				{
					break;
				}
				addActorIndex();
				if (!hasNextSubAction())
				{
					break;
				}
				createNewSubAction();
			}
		}

		/// <summary>
		///ִ���Ӷ���
		/// 
		/// </summary>
		protected internal virtual void optionSubAction()
		{
			curSubAction.onAction();
			// ����һЩ������������֮����������
			if (curSubAction.DeadInTime)
			{
				curSubAction.createDeadth();
			}
		}

		public virtual bool HangUp
		{
			get
			{
				return curState.HangUp;
			}
		}

		public virtual void createNewSubAction()
		{
			if (HangUp)
			{
				return;
			}
			curSubAction = createSubActionByType();
			// ÿ����һ���¶����ͻ�����һ�غ�
			loopCount++;
			initOnCreateSubAction();
		}

		// @Override
		public virtual void updateBattleState(BattleState battelState, bool updateSub)
		{
			CurState = battelState;
			if (updateSub)
			{
				if (CurSubAction == null)
				{
					return;
				}
				CurSubAction.updateBattleState(battelState, updateSub);
			}
		}

		public virtual BattleState CurState
		{
			set
			{
				this.curState = value;
			}
			get
			{
				return curState;
			}
		}

		public virtual void setFinish()
		{
			this.finished = true;
			this.curState = BattleState.STOP;
		}

		public virtual A CurDefencer
		{
			set
			{
				this.curDefencer = value;
			}
			get
			{
				return curDefencer;
			}
		}

		public virtual A CurAttacker
		{
			get
			{
				return curAttacker;
			}
		}


		public virtual T CurSubAction
		{
			get
			{
				return curSubAction;
			}
		}


		public virtual Battle Battle
		{
			get
			{
				return battle;
			}
		}

		// @Override
		public virtual bool Finished
		{
			get
			{
				return finished;
			}
		}

		public virtual bool Dead
		{
			get
			{
				return isActorDead(curAttacker) || isActorDead(curDefencer);
			}
		}

		protected internal virtual bool isActorDead(A actor)
		{
			return actor == null ? false :!actor.hasHp();
		}

		public virtual bool FirstEnter
		{
			get
			{
				return firstEnter;
			}
		}

		public virtual int LoopCount
		{
			get
			{
				return loopCount;
			}
		}

		public virtual BattleRecord Record
		{
			get
			{
				return battle.Record;
			}
		}

		/// <summary>
		/// �ڶ��������������Ϣ
		/// </summary>
		public abstract void addActorIndex();

		/// <summary>
		/// �Ƿ���������
		/// 
		/// @return
		/// </summary>
		public abstract bool hasNextSubAction();

		/// <summary>
		/// ��ȡ��һ������
		/// 
		/// @return
		/// </summary>
		public abstract T createSubActionByType();

		/// <summary>
		/// ��ʼ���ող����Ķ���
		/// </summary>
		public abstract void initOnCreateSubAction();

		/// <summary>
		/// �Ƿ�ȫ�������Լ�����
		/// 
		/// @return
		/// </summary>
		public abstract bool AllActionFinish {get;}

		public abstract void onStart();

		public abstract void onFinish();

		// @Override
		public virtual bool DeadInTime
		{
			get
			{
				return true;
			}
		}

	}

}
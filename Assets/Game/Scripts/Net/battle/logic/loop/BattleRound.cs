namespace com.kx.sglm.gs.battle.share.logic.loop
{

	using BattleArmy = com.kx.sglm.gs.battle.share.actor.impl.BattleArmy;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using BattleState = com.kx.sglm.gs.battle.share.enums.BattleState;
	using IBattleExecuter = com.kx.sglm.gs.battle.share.executer.IBattleExecuter;
	using com.kx.sglm.gs.battle.share.logic;

	/// <summary>
	/// ����һ���غϣ���ս��������BattleTeam��һ���������֡� �����������ֹ��� ������<seealso cref="IBattleExecuter"/>������
	/// ���ֽ�������δ��������ֱ�ӽ�����һ���غϣ��������������֪ͨ<seealso cref="BattleScene"/>�Ƿ���Ҫ�л�
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleRound : AbstractBattleLooper<BattleTeamShot, BattleArmy>
	{

		public BattleRound(Battle battle) : base(battle, battle.BattleArmy, battle.BattleArmy)
		{
			//��һ���attacker�Ƚ�������������BattleArmy
		}

		public override bool Dead
		{
			get
			{
				return isActorDead(CurAttacker) || isActorDead(CurDefencer);
			}
		}

		public override void onFinish()
		{
			//TODO �Ƿ���ÿ�λغϽ����Ľ��㣬�Ժ���ܻ�������߼�
			BattleRoundCountRecord _roundRecord = Battle.Record.OrCreateRoundCountRecord;
			//TODO: �����Fighter�ж�������isDead()�������ظ��ģ��Ժ�Ҫ�ع�
			if (!Dead)
			{
				CurAttacker.onRoundFinish(_roundRecord);
			}
			Battle.Record.finishCurRoundCountRecord();
		}

		public override bool hasNextSubAction()
		{
			return CurAttacker.hasCurActor();
		}

		public override BattleTeamShot createSubActionByType()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.share.actor.impl.BattleTeam _attackerTeam = getCurAttacker().getCurActor();
			BattleTeam _attackerTeam = CurAttacker.CurActor;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.share.actor.impl.BattleTeam _defencerTeam = getCurAttacker().getOppositeTeam(_attackerTeam);
			BattleTeam _defencerTeam = CurAttacker.getOppositeTeam(_attackerTeam);
			BattleTeamShot _teamSlot = new BattleTeamShot(Battle, _attackerTeam, _defencerTeam);
			return _teamSlot;
		}

		public override void addActorIndex()
		{
			CurAttacker.addCurIndex();
		}

		public override void initOnCreateSubAction()
		{
			BattleTeam _battleTeam = CurAttacker.CurActor;
			_battleTeam.resetOnNewAction();
			//TODO: ÿ�غϿ�ʼǰ��team����index��Ϊ0
			bool _needHungUp = Battle.BattleExcuter.needHungUp(this, _battleTeam);
			if (_needHungUp)
			{
				Battle.updateBattleState(BattleState.HUNGUP);
			}
			//�����ʼ�������������Զ�PVPʱ����������Ϣ�ȵ�
			Battle.BattleExcuter.onBattleRoundStart(this, _battleTeam);
		}


		public override bool AllActionFinish
		{
			get
			{
				return CurState != BattleState.HUNGUP;
			}
		}

		public override void createDeadth()
		{
			foreach (BattleTeam _side in CurAttacker.ActorList)
			{
				if (!_side.hasHp())
				{
					CurAttacker.DeadBattleSide = _side.BattleSide;
					break;
				}
			}
		}

		public override void onStart()
		{
			// TODO Auto-generated method stub

		}

	}

}
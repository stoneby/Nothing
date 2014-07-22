namespace com.kx.sglm.gs.battle.share.logic.loop
{

	using BattleArmy = com.kx.sglm.gs.battle.share.actor.impl.BattleArmy;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using BattleTeamFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord;
	using TeamShotStartEvent = com.kx.sglm.gs.battle.share.@event.impl.TeamShotStartEvent;
	using BattleLogicHelper = com.kx.sglm.gs.battle.share.helper.BattleLogicHelper;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;
	using com.kx.sglm.gs.battle.share.logic;
	using BattleAttackAction = com.kx.sglm.gs.battle.share.logic.action.BattleAttackAction;

	/// <summary>
	/// �����������ѭ���������࣬ ������һ��<seealso cref="BattleTeam"/>�� ������ѭ��һ��<seealso cref="BattleTeam"/>������Ҫ���ֵĶ��󣬴���
	/// <seealso cref="BattleAttackAction"/>������һ�������� ����ȫ����ɺ���Ҫ����RoundCounter�Ľ���
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleTeamShot : AbstractBattleLooper<BattleAttackAction, BattleTeam>
	{

		public BattleTeamShot(Battle battle, BattleTeam attackTeam, BattleTeam defencerTeam) : base(battle, attackTeam, defencerTeam)
		{

		}


		public override void onStart()
		{

			Battle.BattleExcuter.onBattleTeamShotStart(this);
			TeamShotStartEvent _event = createEvent();
			CurAttacker.onTeamShotStart(_event);
			CurAttacker.recalcTeamProp();
			CurDefencer.recalcTeamProp();
			recordBattleTeamInfo();
		}

		protected internal virtual TeamShotStartEvent createEvent()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord _teamReocrd = getBattle().getRecord().getOrCreateTeamFighterRecord();
			BattleTeamFightRecord _teamReocrd = Battle.Record.OrCreateTeamFighterRecord;
			_teamReocrd.TeamSide = CurAttacker.BattleSide.Index;
			TeamShotStartEvent _event = new TeamShotStartEvent(Battle.CurScene);
			_event.TeamFightRecord = _teamReocrd;
			_event.CurSide = CurAttacker.BattleSide;
			return _event;
		}

		protected internal virtual void recordBattleTeamInfo()
		{
			BattleArmy _army = Battle.BattleArmy;
			foreach (BattleTeam _team in _army.ActorList)
			{
				BattleRecordHelper.recordBattleTeamRecord(_team, Record);
			}
		}

		public override void onFinish()
		{
			// �����Լ���BUFF
			CurAttacker.activeAllBuff(BattleConstants.BUFF_ALL_FALG);
			//ˢ�������˵���ʾ״̬
			BattleLogicHelper.refreshState(Battle.BattleArmy);
			// �������Ҵ�����һ���佫���볡
			Battle.BattleExcuter.onBattleTeamShotFinish(this);
			Record.finishCurTeamFightRecord();
		}

		public override bool hasNextSubAction()
		{
			return CurAttacker.hasFightFighter();
		}

		public override BattleAttackAction createSubActionByType()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.share.actor.impl.BattleFighter _curFighter = getCurAttacker().getCurFighter();
			BattleFighter _curFighter = CurAttacker.CurFighter;
			// TODO: �����Ƿ����ͬһ��Team��ÿ�γ����ڹ�����ͬ��Team������֮����߻�ȷ�ϣ���ʱ��֧��
			BattleAttackAction _fightAction = new BattleAttackAction(Battle, _curFighter, CurDefencer);

			return _fightAction;
		}

		public override void addActorIndex()
		{
			CurAttacker.addCurFightIndex();

		}

		public override void initOnCreateSubAction()
		{
			//��ʼ��ս��
		}


		public override bool AllActionFinish
		{
			get
			{
				return CurAttacker.hasCurActor();
			}
		}

		public override void createDeadth()
		{
			CurAttacker.tryDead();
			CurDefencer.tryDead();
		}


	}

}
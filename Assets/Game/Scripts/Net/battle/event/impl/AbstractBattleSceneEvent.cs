namespace com.kx.sglm.gs.battle.share.@event.impl
{

	using BattleScene = com.kx.sglm.gs.battle.share.logic.loop.BattleScene;


	/// <summary>
	/// �����¼�������ս�����������ɵ� 
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class AbstractBattleSceneEvent : InnerBattleEvent
	{

		private readonly int eventType;
		private readonly BattleScene battleScene;

		public AbstractBattleSceneEvent(int eventType, BattleScene battleScene)
		{
			this.eventType = eventType;
			this.battleScene = battleScene;
		}

		public virtual void fireEvent()
		{

		}

		public virtual BattleScene BattleScene
		{
			get
			{
				return battleScene;
			}
		}

		public virtual int EventType
		{
			get
			{
				return eventType;
			}
		}

	}

}
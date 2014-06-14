namespace com.kx.sglm.gs.battle.share.@event.impl
{

	using BattleScene = com.kx.sglm.gs.battle.share.logic.loop.BattleScene;


	public class SceneStartEvent : AbstractBattleSceneEvent
	{

		public SceneStartEvent(BattleScene battleScene) : base(BattleEventConstants.BATTLE_SCENE_START, battleScene)
		{
		}






	}

}
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data.store
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;

	public class SceneEndExecuter : AbstractBattleStoreExecuter
	{

		private int sceneIndex;
		private int curHp;
		private int curMp;
		private List<int> spIndexList;


		public SceneEndExecuter(BattleStoreData data) : base(data)
		{
			spIndexList = new List<int>();
		}

		public virtual void handleSceneFinish(int sceneIndex, HeroTeam heroTeam)
		{
			this.sceneIndex = sceneIndex;
			this.curHp = heroTeam.CurHp;
			this.curMp = heroTeam.CurMp;
			foreach (BattleFighter _fighter in heroTeam.ActorList)
			{
				if (_fighter.hasState(BattleConstants.SP_MAX_FALG))
				{
					spIndexList.Add(_fighter.Index);
				}
			}
			saveToData();
		}


		public override void saveToData()
		{
			data.saveSpString(spIndexList);
			data.addIntValue(BattleKeyConstants.BATTLE_STORE_CUR_SCENE_INDEX, sceneIndex);
			data.addIntValue(BattleKeyConstants.BATTLE_STORE_CUR_HERO_HP, curHp);
			data.addIntValue(BattleKeyConstants.BATTLE_STORE_CUR_HERO_MP, curMp);
		}

	}

}
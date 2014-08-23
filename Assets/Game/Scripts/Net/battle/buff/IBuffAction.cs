namespace com.kx.sglm.gs.battle.share.buff
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using FighterStateEnum = com.kx.sglm.gs.battle.share.enums.FighterStateEnum;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;

	/// <summary>
	/// buff action, interface for AbstractBuffAction, singleton action
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class IBuffAction : IBattlePartInfo
	{

		//TODO: ��ǰ��buff�����кܶ����࣬����Ŀǰʱ�䲻�����ع����¸��汾����֮��Ҫ�ó�һ��ʱ���ع�buff�ṹ
		public abstract int Id {get;set;}

		public abstract int TypeA {get;set;}

		public abstract int TypeB {get;set;}

		public abstract int priority();

		public abstract int CDRound {get;set;}

		public abstract int BuffShowId {get;set;}

		public abstract int MaxStackingCount {get;set;}

		public abstract int SceneClear {get;set;}

		public abstract FighterStateEnum StateEnum {get;}




		public abstract int Priority {set;}


		public abstract int BuffFlag {set;}




		public abstract void onActive(BattleFighter fighter);

		public abstract void onRemove(BattleFighter fighter);

		public abstract void onTeamBeforeAttack(BattleFighter fighter);

		public abstract void onAttack(BattleFighter attacker);

		public abstract void onDefence(BattleFighter attacker, BattleFighter owner);

		public abstract bool needShow(BattleFighterBuff buffInst);

		public abstract bool Buff {get;}

		public abstract bool SceneClearBuff {get;}

	}

}
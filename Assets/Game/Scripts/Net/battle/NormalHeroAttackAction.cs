using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.singleton
{


	using BattleFighter = com.kx.sglm.gs.battle.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.actor.impl.BattleTeam;
	using SingleActionRecord = com.kx.sglm.gs.battle.data.record.SingleActionRecord;

	/// 
	/// <summary>
	/// @author liyuan2
	/// 
	/// </summary>
	public class NormalHeroAttackAction : AbstractSingletonAttackAction
	{

		public override void onAddBuff(BattleFighter attacker, BattleTeam defencerTeam)
		{
			// There is no buff here

		}

		public override int getAttackRatio(BattleFighter attacker)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _curIndex = attacker.getOwnerTeam().getCurIndex();
			int _curIndex = attacker.OwnerTeam.CurIndex;
			int _ratio = 0;
			if (_curIndex < BattleConstants.BATTLE_HERO_INDEX_RATIO.Length)
			{
				_ratio = BattleConstants.BATTLE_HERO_INDEX_RATIO[_curIndex];
			}
			return _ratio;
		}

		public override void costHp(int costHp, BattleFighter defencer, SingleActionRecord record)
		{
			int _finalHp = defencer.CurHp - costHp;
			_finalHp = _finalHp < 0 ? 0 : _finalHp;
			defencer.CurHp = _finalHp;
			record.ResultHp = defencer.CurHp;
		}

		public override IList<BattleFighter> getDefencerList(BattleFighter attacker, BattleTeam defencerTeam)
		{
			IList<BattleFighter> _defencerList = new List<BattleFighter>();
			int _curTargetIndex = attacker.OwnerTeam.getIntProp(BattleKeyConstants.BATTLE_KEY_HERO_TEAM_TARGET);
	//		int _monsterSize = defencerTeam.actorSize();
			BattleFighter _defencer = defencerTeam.getActor(_curTargetIndex);
	//		for (int _i = 0; _i < _monsterSize; _i++) {
	//			if (_defencer.isAlive()) {// TODO: 这里可能还会有其他情况不能被打
	//				break;
	//			}
	//			_defencer = defencerTeam.getActor(_i);
	//		}
			_defencerList.Add(_defencer);
			return _defencerList;
		}

		public override float calcDamage(float attack, BattleFighter attacker, BattleFighter defencer)
		{
			float _damageFree = defencer.DamageFree;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _jobWeak = defencer.getFighterOtherProp(com.kx.sglm.gs.battle.BattleKeyConstants.BATTLE_KEY_MONSTER_WEEK_JOB);
			int _jobWeak = defencer.getFighterOtherProp(BattleKeyConstants.BATTLE_KEY_MONSTER_WEEK_JOB);
			if (_jobWeak == attacker.Job)
			{
				// attack *= BattleConstants.MONSTER_WEAK_RATIO;//TODO: add job week
			}
			return calcDamagefree(_damageFree, attack);
		}

	}

}
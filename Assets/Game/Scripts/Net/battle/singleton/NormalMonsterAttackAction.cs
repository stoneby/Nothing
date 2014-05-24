using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.singleton
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using MonsterSkillManager = com.kx.sglm.gs.battle.share.skill.MonsterSkillManager;

	public class NormalMonsterAttackAction : AbstractSingletonAttackAction
	{

		public virtual void onSkillCountDown(MonsterSkillManager manager)
		{

		}

		public virtual int DefaultRound
		{
			get
			{
				return 1; // TODO: 测试代码很糙要改
			}
		}

		public override float calcDamage(float attack, BattleFighter attacker, BattleFighter defencer)
		{
			float _damageFree = defencer.DamageFree;
			return calcDamagefree(_damageFree, attack);
		}

		public override void onAddBuff(BattleFighter attacker, BattleTeam defencerTeam)
		{
			// cur nothing
		}

		public override int getAttackRatio(BattleFighter attacker)
		{
			return BattleConstants.BATTLE_HERO_INDEX_RATIO[0];
		}

		public override void costHp(int costHp, BattleFighter defencer, SingleActionRecord record)
		{
			BattleTeam _heroTeam = defencer.getOwnerTeam();
			_heroTeam.changeHp(-costHp, defencer);
			record.ResultHp = _heroTeam.CurHp;

		}

		public override IList<BattleFighter> getDefencerList(BattleFighter attacker, BattleTeam defencerTeam)
		{
			IList<BattleFighter> _fighterList = new List<BattleFighter>();
			int _index = MathUtils.random(0, BattleConstants.HERO_BATTLE_ARR_LENGTH - 1);
			_fighterList.Add(defencerTeam.getFighterByIndex(_index));
			return _fighterList;
		}

	}

}
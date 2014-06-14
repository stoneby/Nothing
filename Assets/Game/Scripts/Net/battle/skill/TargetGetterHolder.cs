using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;

	/// <summary>
	/// 目标获取器，自己知道是对敌还是对�?
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class TargetGetterHolder
	{

		/// <summary>
		/// 是否是对敌的 </summary>
		private bool enemyEffect;
		/// <summary>
		/// 目标列表 </summary>
		private List<ISkillTargetGetter> targetGetterList;

		public TargetGetterHolder(bool enemyEffect)
		{
			this.enemyEffect = enemyEffect;
			this.targetGetterList = new List<ISkillTargetGetter>();
		}

		public virtual List<BattleFighter> calcTargetList(BattleFighter attacker)
		{
			BattleTeam _targetTeam = getTargetTeam(attacker, enemyEffect);
			return calcTargetList(attacker, _targetTeam);
		}

		protected internal virtual List<BattleFighter> calcTargetList(BattleFighter attacker, BattleTeam targetTeam)
		{
			int _targetFlag = calcResultFlag(attacker, targetTeam);

			List<BattleFighter> _resultTarget = new List<BattleFighter>();
			int _teamSize = targetTeam.allActorSize();
			for (int _i = 0; _i < _teamSize; _i++)
			{
				if (MathUtils.hasFlagIndex(_targetFlag, _i))
				{
					_resultTarget.Add(targetTeam.getActor(_i));
				}
			}
			return _resultTarget;
		}

		private int calcResultFlag(BattleFighter attacker, BattleTeam targetTeam)
		{
			int _targetFlag = 0xffff;
			foreach (ISkillTargetGetter _targetGetter in targetGetterList)
			{
				List<BattleFighter> _targetFighterList = _targetGetter.getTarget(attacker, targetTeam);
				int _curFlag = calcTargetFlag(_targetFighterList);
				_targetFlag &= _curFlag;
			}
			return _targetFlag;
		}

		private int calcTargetFlag(List<BattleFighter> targetFighterList)
		{
			int _flag = 0;
			foreach (BattleFighter _fighter in targetFighterList)
			{
				_flag = MathUtils.optionOrFlag(_flag, _fighter.Index);
			}
			return _flag;
		}

		public virtual BattleTeam getTargetTeam(BattleFighter attacker, bool enemyEffect)
		{
			BattleTeam _ownerTeam = attacker.getOwnerTeam();
			return enemyEffect ? _ownerTeam.OppositeTeam : _ownerTeam;
		}

		public virtual void addTargetGetter(ISkillTargetGetter targetGetter)
		{
			this.targetGetterList.Add(targetGetter);
		}

		public virtual void addAll(List<ISkillTargetGetter> targetGetterList)
		{
			this.targetGetterList.Clear();
			this.targetGetterList.AddRange(targetGetterList);
		}
	}

}
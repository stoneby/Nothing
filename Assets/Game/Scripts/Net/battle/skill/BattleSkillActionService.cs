using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill
{


	using MonsterAI = com.kx.sglm.gs.battle.share.ai.MonsterAI;
	using SkillAI = com.kx.sglm.gs.battle.share.ai.SkillAI;
	using SkillAIHolder = com.kx.sglm.gs.battle.share.ai.SkillAIHolder;
	using SkillRoulettePair = com.kx.sglm.gs.battle.share.ai.SkillRoulettePair;
	using BaseHeroBattleSkillAction = com.kx.sglm.gs.battle.share.skill.action.BaseHeroBattleSkillAction;
	using BaseMonsterSkillAction = com.kx.sglm.gs.battle.share.skill.action.BaseMonsterSkillAction;
	using SkillActionCreater = com.kx.sglm.gs.battle.share.skill.creater.SkillActionCreater;
	using BattleHeroSkillMsgAction = KXSGCodec.BattleHeroSkillMsgAction;
	using BattleMonsterAIMsgAction = KXSGCodec.BattleMonsterAIMsgAction;
	using BattleMonsterSkillMsgAction = KXSGCodec.BattleMonsterSkillMsgAction;

	/// <summary>
	/// ���ܶ����������������������ʱ���𻺴����ã������Ժ�һ�����Ż�
	/// 
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	// TODO: �Ż�SkillActionService
	public class BattleSkillActionService
	{

		/// <summary>
		/// ��Ϊ��Ҫת����C#���룬�����û�б�����ȫ�ֵ�GSService��ʹ���Լ��ĵ������ڿͻ���ս��������ط����õ�ʱ��Ŵ����ģ����Ҳ�����ȫ���ļ��� </summary>
		private static BattleSkillActionService service = new BattleSkillActionService();

		/// <summary>
		/// Ӣ����ͨ���� </summary>
		private BaseHeroBattleSkillAction normalHeroAttack;
		/// <summary>
		/// Ӣ����ͨ��Ѫ </summary>
		private BaseHeroBattleSkillAction normalHeroRecover;
		/// <summary>
		/// Ӣ�����м��ܼ��� </summary>
		private Dictionary<int, BaseHeroBattleSkillAction> heroSkillActionMap;
		/// <summary>
		/// �������м��ܽ�� </summary>
		private Dictionary<int, BaseMonsterSkillAction> monsterSkillActionMap;

		private Dictionary<int, MonsterAI> monsterAIMap;

		public BattleSkillActionService()
		{
			heroSkillActionMap = new Dictionary<int, BaseHeroBattleSkillAction>();
			monsterSkillActionMap = new Dictionary<int, BaseMonsterSkillAction>();
			monsterAIMap = new Dictionary<int, MonsterAI>();
		}

		public static BattleSkillActionService Service
		{
			get
			{
				return service;
			}
		}

		public virtual List<BaseHeroBattleSkillAction> getSkillAction(List<int> skillIds)
		{
			List<BaseHeroBattleSkillAction> _actionList = new List<BaseHeroBattleSkillAction>();
			foreach (int _id in skillIds)
			{
				if (heroSkillActionMap.ContainsKey(_id))
				{
					_actionList.Add(getHeroSkillAction(_id));
				}
			}
			return _actionList;
		}

		public virtual Dictionary<int, BaseMonsterSkillAction> getSkillActions(MonsterAI ai)
		{
			Dictionary<int, BaseMonsterSkillAction> _actionMap = new Dictionary<int, BaseMonsterSkillAction>();
			foreach (SkillRoulettePair _defaultPair in ai.AllDefaultSkill)
			{
				int _id = _defaultPair.SkillId;
				if (monsterSkillActionMap.ContainsKey(_id))
				{
					_actionMap[_id] = getMonsterSkillAction(_id);
				}
			}

			foreach (SkillAIHolder _aiHolder in ai.AllAISkill)
			{
				foreach (SkillAI _skillAi in _aiHolder.HoldList)
				{
					int _id = _skillAi.getSkillInfo().SkillId;
					if (monsterSkillActionMap.ContainsKey(_id))
					{
						_actionMap[_id] = getMonsterSkillAction(_id);
					}
				}
			}
			return _actionMap;
		}

		public virtual BaseHeroBattleSkillAction getHeroSkillAction(int skillId)
		{
			return heroSkillActionMap[skillId];
		}


		public virtual BaseMonsterSkillAction getMonsterSkillAction(int skillId)
		{
			return monsterSkillActionMap[skillId];
		}

		public virtual MonsterAI getMonsterAI(int aiId)
		{
			return monsterAIMap[aiId];
		}
		/// <summary>
		/// ������ͨ������Ŀǰֻ��������Ӣ�۹�����Ӣ�ۻ�Ѫ�����﹥��ȫ���Ǽ��ܹ���
		/// </summary>
		public virtual void initNormalAction()
		{
			normalHeroAttack = SkillActionCreater.createDefaultNormalAction(true);
			normalHeroRecover = SkillActionCreater.createDefaultNormalAction(false);
		}

		/// <summary>
		/// ����������輼��ģ��ת���ɼ���ʹ�ö���
		/// </summary>
		/// <param name="dataList"> </param>
		public virtual void initTemplateHeroSkillAction(List<BattleHeroSkillMsgAction> dataList)
		{
			foreach (BattleHeroSkillMsgAction _data in dataList)
			{
				BaseHeroBattleSkillAction _action = SkillActionCreater.createHeroSkillAction(_data);
				heroSkillActionMap[_data.Id] = _action;
			}
		}



		public virtual void initAllMonsterAI(List<BattleMonsterAIMsgAction> allMonsterAIData)
		{
			foreach (BattleMonsterAIMsgAction _actionData in allMonsterAIData)
			{
				MonsterAI _ai = SkillActionCreater.createMonsterAI(_actionData);
				monsterAIMap[_actionData.Id] = _ai;
			}
		}
		/// <summary>
		/// �������＼����Ϣ����
		/// </summary>
		/// <param name="dataList"> </param>
		public virtual void initTemplateMonsterSkillAction(List<BattleMonsterSkillMsgAction> dataList)
		{
			foreach (BattleMonsterSkillMsgAction _data in dataList)
			{
				BaseMonsterSkillAction _action = SkillActionCreater.createMonsterSkillAction(_data);
				monsterSkillActionMap[_data.Id] = _action;
			}
		}

		public virtual BaseHeroBattleSkillAction NormalHeroAttack
		{
			get
			{
				return normalHeroAttack;
			}
		}

		public virtual BaseHeroBattleSkillAction NormalHeroRecover
		{
			get
			{
				return normalHeroRecover;
			}
		}

	}

}
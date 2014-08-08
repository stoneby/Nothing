using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share
{


	using MonsterAI = com.kx.sglm.gs.battle.share.ai.MonsterAI;
	using SkillAI = com.kx.sglm.gs.battle.share.ai.SkillAI;
	using SkillAIHolder = com.kx.sglm.gs.battle.share.ai.SkillAIHolder;
	using SkillRoulettePair = com.kx.sglm.gs.battle.share.ai.SkillRoulettePair;
	using IBuffAction = com.kx.sglm.gs.battle.share.buff.IBuffAction;
	using BaseHeroBattleSkillAction = com.kx.sglm.gs.battle.share.skill.action.BaseHeroBattleSkillAction;
	using BaseMonsterSkillAction = com.kx.sglm.gs.battle.share.skill.action.BaseMonsterSkillAction;
	using SkillActionCreater = com.kx.sglm.gs.battle.share.skill.creater.SkillActionCreater;
	using BattleBuffMsgData = KXSGCodec.BattleBuffMsgData;
	using BattleHeroSkillMsgAction = KXSGCodec.BattleHeroSkillMsgAction;
	using BattleMonsterAIMsgAction = KXSGCodec.BattleMonsterAIMsgAction;
	using BattleMonsterSkillMsgAction = KXSGCodec.BattleMonsterSkillMsgAction;
	using AmendManagerImpl = com.kx.sglm.gs.role.properties.amend.AmendManagerImpl;
	using IAmendManager = com.kx.sglm.gs.role.properties.amend.IAmendManager;

	/// <summary>
	/// 技能动作管理器，这个类现在暂时不起缓存作用，但是以后一定会优化
	/// 
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	// TODO: 优化SkillActionService
	public class BattleActionService
	{

		/// <summary>
		/// 因为需要转换成C#代码，这个类没有被归入全局的GSService，使用自己的单例，在客户端战斗中这个地方是用的时候才创建的，而且不包括全部的技能 </summary>
		private static BattleActionService service = new BattleActionService();

		private int spMaxBuffId;

		/// <summary>
		/// 英雄普通攻击 </summary>
		private BaseHeroBattleSkillAction normalHeroAttack;
		/// <summary>
		/// 英雄普通回血 </summary>
		private BaseHeroBattleSkillAction normalHeroRecover;

		private BaseHeroBattleSkillAction normalAOEAttack;
		/// <summary>
		/// 英雄所有技能集合 </summary>
		private Dictionary<int, BaseHeroBattleSkillAction> heroSkillActionMap;
		/// <summary>
		/// 怪物所有技能结合 </summary>
		private Dictionary<int, BaseMonsterSkillAction> monsterSkillActionMap;

		private Dictionary<int, MonsterAI> monsterAIMap;

		private Dictionary<int, IBuffAction> allBuffAction;

		private IAmendManager amendManager;

		public BattleActionService()
		{
			heroSkillActionMap = new Dictionary<int, BaseHeroBattleSkillAction>();
			monsterSkillActionMap = new Dictionary<int, BaseMonsterSkillAction>();
			monsterAIMap = new Dictionary<int, MonsterAI>();
			allBuffAction = new Dictionary<int, IBuffAction>();
			amendManager = new AmendManagerImpl();
		}

		public static BattleActionService Service
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
				BaseHeroBattleSkillAction _action = getHeroSkillAction(_id);
				if (_action != null)
				{
					_actionList.Add(_action);
				}
			}
			return _actionList;
		}


		public virtual BaseHeroBattleSkillAction getHeroSkillAction(int skillId)
		{
			BaseHeroBattleSkillAction _action = null;
			if (heroSkillActionMap.ContainsKey(skillId))
			{
				_action = heroSkillActionMap[skillId];
			}
			return _action;
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

		public virtual IBuffAction getBuffAction(int buffId)
		{
		    try
		    {
                return allBuffAction[buffId];
		    }
		    catch (Exception)
		    {
                Logger.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!allBuffAction[buffid] is null, buffid:"+buffId);
		        return null;
		    }
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
		/// 创建普通动作，目前只有两个，英雄攻击和英雄回血，怪物攻击全部是技能攻击
		/// </summary>
		public virtual void initNormalAction()
		{
			normalHeroAttack = SkillActionCreater.createDefaultNormalAction(true, false);
			normalHeroRecover = SkillActionCreater.createDefaultNormalAction(false, false);
			normalAOEAttack = SkillActionCreater.createDefaultNormalAction(true, true);
		}


		/// <summary>
		/// 将传入的所需技能模板转换成技能使用对象
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
		/// 创建怪物技能信息对象
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

		public virtual void initTemplateBuffAction(List<BattleBuffMsgData> dataList)
		{
			List<IBuffAction> _actionList = SkillActionCreater.createBuffActions(dataList);
			foreach (IBuffAction _action in _actionList)
			{
				allBuffAction[_action.Id] = _action;
			}
		}



		public virtual int SpMaxBuffId
		{
			get
			{
				return spMaxBuffId;
			}
			set
			{
				this.spMaxBuffId = value;
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

		public virtual BaseHeroBattleSkillAction AOEAttack
		{
			get
			{
				return normalAOEAttack;
			}
		}

		public virtual IAmendManager AmendManager
		{
			get
			{
				return amendManager;
			}
		}

	}










}
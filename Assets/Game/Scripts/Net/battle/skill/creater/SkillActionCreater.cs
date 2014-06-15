using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.creater
{


	using MonsterAI = com.kx.sglm.gs.battle.share.ai.MonsterAI;
	using SkillAI = com.kx.sglm.gs.battle.share.ai.SkillAI;
	using SkillAIHolder = com.kx.sglm.gs.battle.share.ai.SkillAIHolder;
	using SkillRoulettePair = com.kx.sglm.gs.battle.share.ai.SkillRoulettePair;
	using BaseHeroBattleSkillAction = com.kx.sglm.gs.battle.share.skill.action.BaseHeroBattleSkillAction;
	using BaseMonsterSkillAction = com.kx.sglm.gs.battle.share.skill.action.BaseMonsterSkillAction;
	using ArrayUtils = com.kx.sglm.gs.battle.share.utils.ArrayUtils;
	using BattleHeroSkillMsgAction = KXSGCodec.BattleHeroSkillMsgAction;
	using BattleMonsterAIMsgAction = KXSGCodec.BattleMonsterAIMsgAction;
	using BattleMonsterSkillMsgAction = KXSGCodec.BattleMonsterSkillMsgAction;
	using BattleSkillMsgCondition = KXSGCodec.BattleSkillMsgCondition;
	using MonsterSkillAIMsgData = KXSGCodec.MonsterSkillAIMsgData;
	using SkillBattleEffectMsgData = KXSGCodec.SkillBattleEffectMsgData;
	using SkillRatePairMsgData = KXSGCodec.SkillRatePairMsgData;
	using SkillTargetGetterMsgData = KXSGCodec.SkillTargetGetterMsgData;

	/// <summary>
	/// 技能动作生成类
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class SkillActionCreater
	{

		/// <summary>
		/// 创建怪物AI，和技能一样也是理论上单例的，不持有战斗对象状态数�?
		/// </summary>
		/// <param name="baseData">
		/// @return </param>
		public static MonsterAI createMonsterAI(BattleMonsterAIMsgAction baseData)
		{
			MonsterAI _ai = new MonsterAI();
			_ai.AllDefaultSkill = createDefaultSkillFromTemp(baseData);
			_ai.AllAISkill = createSkillAIHolderFromTemp(baseData);
			return _ai;
		}


		/// <summary>
		/// 创建怪物技能动�?
		/// </summary>
		/// <param name="baseData">
		/// @return </param>
		public static BaseMonsterSkillAction createMonsterSkillAction(BattleMonsterSkillMsgAction baseData)
		{
			BaseMonsterSkillAction _action = new BaseMonsterSkillAction();

			List<ISkillTargetGetter> _enemyTargetGetterList = createTargetGetter(true, baseData.EnemyTargetList);
			List<ISkillTargetGetter> _friendTargetGetterList = createTargetGetter(false, baseData.FriendTargetList);
			List<ISkillEffect> _effectList = createEffectList(baseData.EffectList);

			_action.SkillId = baseData.Id;
			_action.ExtraCD = baseData.ExtraCDRound;
			_action.ExtraCDDesc = baseData.ExtraCDDesc;
			_action.addEnemyTargetGetter(_enemyTargetGetterList);
			_action.addFriendTargetGetter(_friendTargetGetterList);
			_action.addEffectList(_effectList);

			return _action;
		}

		/// <summary>
		/// 创建Hero的战斗基础动作，普通和回血
		/// </summary>
		/// <param name="enemySide">
		/// @return </param>
		public static BaseHeroBattleSkillAction createDefaultNormalAction(bool enemySide)
		{
			BaseHeroBattleSkillAction _action = new BaseHeroBattleSkillAction();
			_action.addCondition(createDefaultCondition((int) BattleConstants.BATTLE_RATIO_BASE));
			_action.addEnemyTargetGetter(createTargetGetter(enemySide, null));
			_action.addFriendTargetGetter(createTargetGetter(enemySide, null));
			_action.addEffectList(createDefaultAction(enemySide));
			return _action;
		}

		/// <summary>
		/// 从模板取出数据构建SkillAction
		/// </summary>
		/// <param name="baseData">
		/// @return </param>
		public static BaseHeroBattleSkillAction createHeroSkillAction(BattleHeroSkillMsgAction baseData)
		{
			BaseHeroBattleSkillAction _action = new BaseHeroBattleSkillAction();
			List<ISkillCondition> _conditionList = createCondition(baseData);
			List<ISkillTargetGetter> _enemyTargetGetterList = createTargetGetter(true, baseData.EnemyTargetList);
			List<ISkillTargetGetter> _friendTargetGetterList = createTargetGetter(false, baseData.FriendTargetList);
			List<ISkillEffect> _effectList = createEffectList(baseData.EffectList);
			// 装入数据
			_action.SkillId = baseData.Id;
			_action.TriggerId = baseData.TriggerId;
			_action.CostMp = baseData.CostMp;
			_action.addCondition(_conditionList);
			_action.addEnemyTargetGetter(_enemyTargetGetterList);
			_action.addFriendTargetGetter(_friendTargetGetterList);
			_action.addEffectList(_effectList);

			return _action;
		}


		/// <summary>
		/// 创建技能[ID--概率]二元�?
		/// </summary>
		/// <param name="baseData">
		/// @return </param>
		protected internal static List<SkillRoulettePair> createDefaultSkillFromTemp(BattleMonsterAIMsgAction baseData)
		{
			List<SkillRoulettePair> _skillList = new List<SkillRoulettePair>();
			List<SkillRatePairMsgData> _dataList = baseData.DefaultSkills;
			foreach (SkillRatePairMsgData _dataPair in _dataList)
			{
				_skillList.Add(createRottlePairFromData(_dataPair));
			}
			return _skillList;
		}

		protected internal static SkillRoulettePair createRottlePairFromData(SkillRatePairMsgData pairData)
		{
			return createRottlePairFromParam(pairData.SkillId, pairData.RouletteRate);
		}

		protected internal static List<SkillAIHolder> createSkillAIHolderFromTemp(BattleMonsterAIMsgAction baseData)
		{
			List<SkillAIHolder> _holderList = new List<SkillAIHolder>();
			List<MonsterSkillAIMsgData> _aiDataList = baseData.AiSkills;
			Dictionary<int, SkillAIHolder> _holderMap = new Dictionary<int, SkillAIHolder>();

			if (ArrayUtils.isEmpty(_aiDataList))
			{
				_aiDataList = new List<MonsterSkillAIMsgData>();
				//TODO: 太糙，以后重�?
			}

			foreach (MonsterSkillAIMsgData _data in _aiDataList)
			{
				SkillAI _ai = createSkillAIFromData(_data);
				int _proiority = _ai.Priority;
				SkillAIHolder _holder = getOrCreateAIHolder(_proiority, _holderMap);
				_holder.addData(_ai);
				_holderMap[_proiority] = _holder; // 为了代码清楚
			}

			return _holderList;
		}

		/// <summary>
		/// 创建一个优先级的技能AI
		/// </summary>
		/// <param name="proiority"> </param>
		/// <param name="holderMap">
		/// @return </param>
		protected internal static SkillAIHolder getOrCreateAIHolder(int proiority, Dictionary<int, SkillAIHolder> holderMap)
		{
			SkillAIHolder _holder = null;
			if (!holderMap.ContainsKey(proiority))
			{
				_holder = new SkillAIHolder(proiority);
			}
			else
			{
				_holder = holderMap[proiority];
			}
			return _holder;
		}

		/// <summary>
		/// 创建单个技能AI
		/// </summary>
		/// <param name="aiData">
		/// @return </param>
		protected internal static SkillAI createSkillAIFromData(MonsterSkillAIMsgData aiData)
		{
			SkillAI _skillAI = new SkillAI();
			int _priority = aiData.Weight;
			SkillRoulettePair _skillPair = createRottlePairFromParam(aiData.SkillId, aiData.RouletteRate);
			IAICondition _condition = createAIConditionFromData(aiData);

			_skillAI.Priority = _priority;
			_skillAI.setSkillInfo(_skillPair);
			_skillAI.Condition = _condition;

			return _skillAI;
		}

		protected internal static SkillRoulettePair createRottlePairFromParam(int skillId, int rouletteRate)
		{
			SkillRoulettePair _pair = new SkillRoulettePair();
			_pair.SkillId = skillId;
			_pair.RottleRate = rouletteRate;
			return _pair;
		}

		/// <summary>
		/// 创建AI条件
		/// </summary>
		/// <param name="aiData">
		/// @return </param>
		protected internal static IAICondition createAIConditionFromData(MonsterSkillAIMsgData aiData)
		{
			AIConditionEnum _conditionType = AIConditionEnum.values()[aiData.ConditionType];
			IAICondition _condition = null;
			if (_conditionType != null)
			{
				_condition = (IAICondition)_conditionType.createInfo(aiData.ConditionParam);
			}
			else
			{
				Logger.Log("cannot.get.ai.condition.type = " + aiData.ConditionType);
			}
			return _condition;
		}



		/// <summary>
		/// 生成默认的条件列表，目前概率RATE是默认的固有条件
		/// </summary>
		/// <param name="baseData">
		/// @return </param>
		protected internal static List<ISkillCondition> createDefaultCondition(int param)
		{
			List<ISkillCondition> _defaultList = new List<ISkillCondition>();
			SkillConditionEnum _rate = SkillConditionEnum.RATE;
			ISkillCondition _condition = (ISkillCondition)_rate.createInfo(param);
			_defaultList.Add(_condition);
			return _defaultList;

		}


		/// <summary>
		/// 通过模板构建条件列表
		/// </summary>
		/// <param name="baseData">
		/// @return </param>
		public static List<ISkillCondition> createCondition(BattleHeroSkillMsgAction baseData)
		{
			List<ISkillCondition> _allCondition = new List<ISkillCondition>();
			List<ISkillCondition> _defaultCondition = createDefaultCondition(baseData.TriggerRate);
			List<ISkillCondition> _conditionList = createConditionListFromList(baseData.ConditionList);

			_allCondition.AddRange(_defaultCondition);
			_allCondition.AddRange(_conditionList);

			return _allCondition;
		}

		/// <summary>
		/// 生成技能条件列�?
		/// </summary>
		/// <param name="conditionDataList">
		/// @return </param>
		public static List<ISkillCondition> createConditionListFromList(List<BattleSkillMsgCondition> conditionDataList)
		{

			SkillPartInfoCreater<ISkillCondition, SkillConditionEnum, BattleSkillMsgCondition> _creater = new SkillPartInfoCreaterAnonymousInnerClassHelper();
			return _creater.createInfoList(conditionDataList, SkillConditionEnum.values());
		}

		private class SkillPartInfoCreaterAnonymousInnerClassHelper : SkillPartInfoCreater<ISkillCondition, SkillConditionEnum, BattleSkillMsgCondition>
		{
			public SkillPartInfoCreaterAnonymousInnerClassHelper()
			{
			}


			internal override ISkillCondition createObj(SkillConditionEnum[] values, BattleSkillMsgCondition obj)
			{
				return (ISkillCondition)createSingleCondition(values, obj.ConditionKey, obj.ConditionVal);
			}
		}

		public static List<ISkillTargetGetter> createTargetGetter(bool enemySide, List<SkillTargetGetterMsgData> targetDataList)
		{
			List<ISkillTargetGetter> _targetGetterList = new List<ISkillTargetGetter>();
			if (ArrayUtils.isEmpty(targetDataList))
			{
				_targetGetterList.AddRange(createDefaultTarget(enemySide));
			}
			else
			{
				_targetGetterList.AddRange(createTargetGetterFromList(targetDataList));
			}
			return _targetGetterList;

		}

		protected internal static List<ISkillTargetGetter> createDefaultTarget(bool enemySide)
		{
			List<ISkillTargetGetter> _defaultList = new List<ISkillTargetGetter>();
			SkillTargetEnum _defaultType = enemySide ? SkillTargetEnum.DEFAULT_TARGET : SkillTargetEnum.SELF_TARGET;
			_defaultList.Add((ISkillTargetGetter)_defaultType.createInfo());
			return _defaultList;
		}

		/// <summary>
		/// 生成技能目标选择列表
		/// </summary>
		/// <param name="targetDataList">
		/// @return </param>
		public static List<ISkillTargetGetter> createTargetGetterFromList(List<SkillTargetGetterMsgData> targetDataList)
		{

			SkillPartInfoCreater<ISkillTargetGetter, SkillTargetEnum, SkillTargetGetterMsgData> _creater = new SkillPartInfoCreaterAnonymousInnerClassHelper2();

			return _creater.createInfoList(targetDataList, SkillTargetEnum.values());
		}

		private class SkillPartInfoCreaterAnonymousInnerClassHelper2 : SkillPartInfoCreater<ISkillTargetGetter, SkillTargetEnum, SkillTargetGetterMsgData>
		{
			public SkillPartInfoCreaterAnonymousInnerClassHelper2()
			{
			}


			internal override ISkillTargetGetter createObj(SkillTargetEnum[] values, SkillTargetGetterMsgData obj)
			{
				return (ISkillTargetGetter)createSingleCondition(values, obj.TargetType, obj.TargetValue);
			}
		}

		/// <summary>
		/// 生成技能效果列�?
		/// </summary>
		/// <param name="effectDataList">
		/// @return </param>
		public static List<ISkillEffect> createEffectList(List<SkillBattleEffectMsgData> effectDataList)
		{
			SkillPartInfoCreater<ISkillEffect, SkillEffectEnum, SkillBattleEffectMsgData> _creater = new SkillPartInfoCreaterAnonymousInnerClassHelper3();
			return _creater.createInfoList(effectDataList, SkillEffectEnum.values());
		}

		private class SkillPartInfoCreaterAnonymousInnerClassHelper3 : SkillPartInfoCreater<ISkillEffect, SkillEffectEnum, SkillBattleEffectMsgData>
		{
			public SkillPartInfoCreaterAnonymousInnerClassHelper3()
			{
			}


			internal override ISkillEffect createObj(SkillEffectEnum[] values, SkillBattleEffectMsgData obj)
			{
				return (ISkillEffect)createSingleCondition(values, obj.BattleEffectType, obj.BattleEffectParam1, obj.BattleEffectParam2, obj.BattleEffectParam3);
			}
		}

		public static List<ISkillEffect> createDefaultAction(bool enemySide)
		{
			List<ISkillEffect> _effectList = new List<ISkillEffect>();
			SkillEffectEnum _defaultEffect = enemySide ? SkillEffectEnum.NORMAL_HERO_HIT : SkillEffectEnum.NORMAL_HERO_RECOVER;
			ISkillEffect _effect = (ISkillEffect)_defaultEffect.createInfo();
			_effectList.Add(_effect);
			return _effectList;
		}

		/// <summary>
		/// 创建技能组成列表的模板方法，配合BaseFactoryEnum使用，需要实现创建自身对象，创建时调用BaseFactoryEnum. createSingleCondition，传入不同参数即�?
		/// 
		/// @author liyuan2
		/// </summary>
		/// @param <T> </param>
		/// @param <E> </param>
		/// @param <DATA> </param>
		private abstract class SkillPartInfoCreater<T, E, DATA> where T : ISkillPartInfo where E : BaseSkillFactoryEnum
		{

			internal abstract T createObj(E[] values, DATA obj);

			internal virtual List<T> createInfoList(List<DATA> baseDataList, E[] values)
			{
				List<T> _resultList = new List<T>();
				if (ArrayUtils.isEmpty(baseDataList))
				{
					return _resultList;
				}
				foreach (DATA _data in baseDataList)
				{
					T _result = createObj(values, _data);
					if (_result != null)
					{
						_resultList.Add(_result);
					}
				}
				return _resultList;
			}

		}



		protected internal static ISkillPartInfo createSingleCondition(BaseSkillFactoryEnum[] values, int key, params int[] param)
		{
			ISkillPartInfo _infoObj = null;
			if (!ArrayUtils.isRightArrayIndex(key, values))
			{
				return _infoObj;
			}
			BaseSkillFactoryEnum _enumType = values[key];
			if (_enumType == null)
			{
				return _infoObj;
			}
			_infoObj = _enumType.createInfo(param);
			return _infoObj;
		}

	}

}
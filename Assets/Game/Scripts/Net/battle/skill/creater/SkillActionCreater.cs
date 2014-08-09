using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.creater
{


	using MonsterAI = com.kx.sglm.gs.battle.share.ai.MonsterAI;
	using SkillAI = com.kx.sglm.gs.battle.share.ai.SkillAI;
	using SkillAIHolder = com.kx.sglm.gs.battle.share.ai.SkillAIHolder;
	using SkillRoulettePair = com.kx.sglm.gs.battle.share.ai.SkillRoulettePair;
	using IBuffAction = com.kx.sglm.gs.battle.share.buff.IBuffAction;
	using AbstractBuffEffect = com.kx.sglm.gs.battle.share.buff.effect.AbstractBuffEffect;
	using BuffEffectEnum = com.kx.sglm.gs.battle.share.buff.enums.BuffEffectEnum;
	using BaseBattleFactoryEnum = com.kx.sglm.gs.battle.share.enums.BaseBattleFactoryEnum;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;
	using BaseHeroBattleSkillAction = com.kx.sglm.gs.battle.share.skill.action.BaseHeroBattleSkillAction;
	using BaseMonsterSkillAction = com.kx.sglm.gs.battle.share.skill.action.BaseMonsterSkillAction;
	using AIConditionEnum = com.kx.sglm.gs.battle.share.skill.enums.AIConditionEnum;
	using SkillConditionEnum = com.kx.sglm.gs.battle.share.skill.enums.SkillConditionEnum;
	using SkillEffectEnum = com.kx.sglm.gs.battle.share.skill.enums.SkillEffectEnum;
	using SkillTargetEnum = com.kx.sglm.gs.battle.share.skill.enums.SkillTargetEnum;
	using ArrayUtils = com.kx.sglm.gs.battle.share.utils.ArrayUtils;

	/// <summary>
	/// 技能动作生成类
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class SkillActionCreater
	{

		/// <summary>
		/// 创建BUFFAction
		/// </summary>
		/// <param name="msgDataList">
		/// @return </param>
		public static List<IBuffAction> createBuffActions(List<Template.Auto.Buff.BattleBuffTemplate> msgDataList)
		{
			SkillPartInfoCreater<IBuffAction, BuffEffectEnum, Template.Auto.Buff.BattleBuffTemplate> _creater = new SkillPartInfoCreaterAnonymousInnerClassHelper();
			return _creater.createInfoList(msgDataList, BuffEffectEnum.values());
		}

		private class SkillPartInfoCreaterAnonymousInnerClassHelper : SkillPartInfoCreater<IBuffAction, BuffEffectEnum, Template.Auto.Buff.BattleBuffTemplate>
		{
			public SkillPartInfoCreaterAnonymousInnerClassHelper()
			{
			}


			internal override IBuffAction createObj(BuffEffectEnum[] values, Template.Auto.Buff.BattleBuffTemplate obj)
			{
				AbstractBuffEffect _buffAction = (AbstractBuffEffect) values[obj.BuffKey].createInstance();
				_buffAction.Id = obj.Id;
				_buffAction.BuffFlag = obj.BuffFlag;
				_buffAction.TypeA = obj.TypeA;
				_buffAction.TypeB = obj.TypeB;
				_buffAction.CDRound = obj.Round;
				_buffAction.Priority = obj.Proiority;
				_buffAction.MaxStackingCount = obj.StackingCount;
				_buffAction.BuffShowId = obj.BuffShowId;
				_buffAction.build(obj.BuffParams);
				return _buffAction;
			}
		}

		/// <summary>
		/// 创建怪物AI，和技能一样也是理论上单例的，不持有战斗对象状态数据
		/// </summary>
		/// <param name="baseData">
		/// @return </param>
		public static MonsterAI createMonsterAI(Template.Auto.Monster.MonsterBattleAITemplate baseData)
		{
			MonsterAI _ai = new MonsterAI();
			_ai.AiId = baseData.Id;
			_ai.ShieldBuffId = baseData.ShieldBuffId;
			_ai.AllDefaultSkill = createDefaultSkillFromTemp(baseData);
			_ai.AllAISkill = createSkillAIHolderFromTemp(baseData);
			return _ai;
		}

		/// <summary>
		/// 创建怪物技能动作
		/// </summary>
		/// <param name="baseData">
		/// @return </param>
		public static BaseMonsterSkillAction createMonsterSkillAction(Template.Auto.Skill.MonsterBattleSkillTemplate baseData)
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
		public static BaseHeroBattleSkillAction createDefaultNormalAction(bool enemySide, bool all)
		{
			BaseHeroBattleSkillAction _action = new BaseHeroBattleSkillAction();
			_action.addCondition(createDefaultCondition(Convert.ToString((int) BattleConstants.BATTLE_RATIO_BASE)));
			_action.addEnemyTargetGetter(createTargetGetter(enemySide, null, all));
			_action.addFriendTargetGetter(createTargetGetter(enemySide, null, all));
			_action.addEffectList(createDefaultAction(enemySide));
			return _action;
		}

		/// <summary>
		/// 从模板取出数据构建SkillAction
		/// </summary>
		/// <param name="baseData">
		/// @return </param>
		public static BaseHeroBattleSkillAction createHeroSkillAction(Template.Auto.Skill.HeroBattleSkillTemplate baseData)
		{
			BaseHeroBattleSkillAction _action = new BaseHeroBattleSkillAction();
			List<ISkillCondition> _conditionList = createCondition(baseData);
			List<ISkillTargetGetter> _enemyTargetGetterList = createTargetGetter(true, baseData.EnemyTargetList);
			List<ISkillTargetGetter> _friendTargetGetterList = createTargetGetter(false, baseData.FriendTargetList);
			List<ISkillEffect> _effectList = createEffectList(baseData.EffectList);
			// 装入数据
			_action.SkillId = baseData.Id;
			_action.TriggerId = baseData.TriggerId;
			_action.CostMp = baseData.CostMP;
			_action.addCondition(_conditionList);
			_action.addEnemyTargetGetter(_enemyTargetGetterList);
			_action.addFriendTargetGetter(_friendTargetGetterList);
			_action.addEffectList(_effectList);

			return _action;
		}

		/// <summary>
		/// 创建技能[ID--概率]二元组
		/// </summary>
		/// <param name="baseData">
		/// @return </param>
		protected internal static List<SkillRoulettePair> createDefaultSkillFromTemp(Template.Auto.Monster.MonsterBattleAITemplate baseData)
		{
			List<SkillRoulettePair> _skillList = new List<SkillRoulettePair>();
			List<Template.Auto.Monster.SkillRatePairData> _dataList = baseData.DefaultSkills;
			foreach (Template.Auto.Monster.SkillRatePairData _dataPair in _dataList)
			{
				_skillList.Add(createRottlePairFromData(_dataPair));
			}
			return _skillList;
		}

		protected internal static SkillRoulettePair createRottlePairFromData(Template.Auto.Monster.SkillRatePairData pairData)
		{
			return createRottlePairFromParam(pairData.SkillId, pairData.RouletteRate);
		}

		protected internal static List<SkillAIHolder> createSkillAIHolderFromTemp(Template.Auto.Monster.MonsterBattleAITemplate baseData)
		{
			List<SkillAIHolder> _holderList = new List<SkillAIHolder>();
			List<Template.Auto.Monster.MonsterSkillAIData> _aiDataList = baseData.AiSkills;
			Dictionary<int, SkillAIHolder> _holderMap = new Dictionary<int, SkillAIHolder>();

			if (ArrayUtils.isEmpty(_aiDataList))
			{
				_aiDataList = new List<Template.Auto.Monster.MonsterSkillAIData>();
				// TODO: 太糙，以后重构
			}

			foreach (Template.Auto.Monster.MonsterSkillAIData _data in _aiDataList)
			{
				SkillAI _ai = createSkillAIFromData(_data);
				int _proiority = _ai.Priority;
				SkillAIHolder _holder = getOrCreateAIHolder(_proiority, _holderMap);
				_holder.addData(_ai);
				_holderMap[_proiority] = _holder; // 为了代码清楚
			}
			_holderList.AddRange(_holderMap.Values);
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
		protected internal static SkillAI createSkillAIFromData(Template.Auto.Monster.MonsterSkillAIData aiData)
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
		protected internal static IAICondition createAIConditionFromData(Template.Auto.Monster.MonsterSkillAIData aiData)
		{
			AIConditionEnum _conditionType = AIConditionEnum.values()[aiData.ConditionType];
			IAICondition _condition = null;
			if (_conditionType != null)
			{
				_condition = (IAICondition) _conditionType.createInfo(aiData.ConditionParam);
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
		protected internal static List<ISkillCondition> createDefaultCondition(string param)
		{
			List<ISkillCondition> _defaultList = new List<ISkillCondition>();
			SkillConditionEnum _rate = SkillConditionEnum.RATE;
			ISkillCondition _condition = (ISkillCondition) _rate.createInfo(param);
			_defaultList.Add(_condition);
			return _defaultList;

		}

		/// <summary>
		/// 通过模板构建条件列表
		/// </summary>
		/// <param name="baseData">
		/// @return </param>
		public static List<ISkillCondition> createCondition(Template.Auto.Skill.HeroBattleSkillTemplate baseData)
		{
			List<ISkillCondition> _allCondition = new List<ISkillCondition>();
			List<ISkillCondition> _defaultCondition = createDefaultCondition(Convert.ToString(baseData.TriggerRate));
			List<ISkillCondition> _conditionList = createConditionListFromList(baseData.SkillConditionList);

			_allCondition.AddRange(_defaultCondition);
			_allCondition.AddRange(_conditionList);

			return _allCondition;
		}

		/// <summary>
		/// 生成技能条件列表
		/// </summary>
		/// <param name="conditionDataList">
		/// @return </param>
		public static List<ISkillCondition> createConditionListFromList(List<Template.Auto.Skill.SkillBattleConditionData> conditionDataList)
		{

			SkillPartInfoCreater<ISkillCondition, SkillConditionEnum, Template.Auto.Skill.SkillBattleConditionData> _creater = new SkillPartInfoCreaterAnonymousInnerClassHelper2();
			return _creater.createInfoList(conditionDataList, SkillConditionEnum.values());
		}

		private class SkillPartInfoCreaterAnonymousInnerClassHelper2 : SkillPartInfoCreater<ISkillCondition, SkillConditionEnum, Template.Auto.Skill.SkillBattleConditionData>
		{
			public SkillPartInfoCreaterAnonymousInnerClassHelper2()
			{
			}


			internal override ISkillCondition createObj(SkillConditionEnum[] values, Template.Auto.Skill.SkillBattleConditionData obj)
			{
				return (ISkillCondition) createSingleInfo(values, obj.ConditionKey, obj.ConditionVal);
			}
		}

		public static List<ISkillTargetGetter> createTargetGetter(bool enemySide, List<Template.Auto.Skill.SkillTargetGetterData> targetDataList, bool all)
		{
			List<ISkillTargetGetter> _targetGetterList = new List<ISkillTargetGetter>();
			if (ArrayUtils.isEmpty(targetDataList))
			{
				_targetGetterList.AddRange(createDefaultTarget(enemySide, all));
			}
			else
			{
				_targetGetterList.AddRange(createTargetGetterFromList(targetDataList));
			}
			return _targetGetterList;
		}

		public static List<ISkillTargetGetter> createTargetGetter(bool enemySide, List<Template.Auto.Skill.SkillTargetGetterData> targetDataList)
		{
			return createTargetGetter(enemySide, targetDataList, false);
		}

		protected internal static List<ISkillTargetGetter> createDefaultTarget(bool enemySide, bool allTarget)
		{
			List<ISkillTargetGetter> _defaultList = new List<ISkillTargetGetter>();
			SkillTargetEnum _defaultType = allTarget ? SkillTargetEnum.ALL_TARGET : (enemySide ? SkillTargetEnum.DEFAULT_TARGET : SkillTargetEnum.SELF_TARGET);
			_defaultList.Add((ISkillTargetGetter) _defaultType.createInfo());
			return _defaultList;
		}

		/// <summary>
		/// 生成技能目标选择列表
		/// </summary>
		/// <param name="targetDataList">
		/// @return </param>
		public static List<ISkillTargetGetter> createTargetGetterFromList(List<Template.Auto.Skill.SkillTargetGetterData> targetDataList)
		{

			SkillPartInfoCreater<ISkillTargetGetter, SkillTargetEnum, Template.Auto.Skill.SkillTargetGetterData> _creater = new SkillPartInfoCreaterAnonymousInnerClassHelper3();

			return _creater.createInfoList(targetDataList, SkillTargetEnum.values());
		}

		private class SkillPartInfoCreaterAnonymousInnerClassHelper3 : SkillPartInfoCreater<ISkillTargetGetter, SkillTargetEnum, Template.Auto.Skill.SkillTargetGetterData>
		{
			public SkillPartInfoCreaterAnonymousInnerClassHelper3()
			{
			}


			internal override ISkillTargetGetter createObj(SkillTargetEnum[] values, Template.Auto.Skill.SkillTargetGetterData obj)
			{
				return (ISkillTargetGetter) createSingleInfo(values, obj.TargetType, obj.TargetValue);
			}
		}

		/// <summary>
		/// 生成技能效果列表
		/// </summary>
		/// <param name="effectDataList">
		/// @return </param>
		public static List<ISkillEffect> createEffectList(List<Template.Auto.Skill.SkillBattleEffectData> effectDataList)
		{
			SkillPartInfoCreater<ISkillEffect, SkillEffectEnum, Template.Auto.Skill.SkillBattleEffectData> _creater = new SkillPartInfoCreaterAnonymousInnerClassHelper4();
			return _creater.createInfoList(effectDataList, SkillEffectEnum.values());
		}

		private class SkillPartInfoCreaterAnonymousInnerClassHelper4 : SkillPartInfoCreater<ISkillEffect, SkillEffectEnum, Template.Auto.Skill.SkillBattleEffectData>
		{
			public SkillPartInfoCreaterAnonymousInnerClassHelper4()
			{
			}


			internal override ISkillEffect createObj(SkillEffectEnum[] values, Template.Auto.Skill.SkillBattleEffectData obj)
			{
				ISkillEffect _effect = (ISkillEffect) createSingleInfo(values, obj.BattleEffectType, obj.BattleEffectParam1, obj.BattleEffectParam2, obj.BattleEffectParam3);
				_effect.Ratio = obj.BattleEffectRatio;
				return _effect;
			}
		}

		public static List<ISkillEffect> createDefaultAction(bool enemySide)
		{
			List<ISkillEffect> _effectList = new List<ISkillEffect>();
			SkillEffectEnum _defaultEffect = enemySide ? SkillEffectEnum.NORMAL_HERO_HIT : SkillEffectEnum.NORMAL_HERO_RECOVER;
			ISkillEffect _effect = (ISkillEffect) _defaultEffect.createInfo();
			_effect.Ratio = BattleConstants.BATTLE_RATIO_BASE;
			_effectList.Add(_effect);
			return _effectList;
		}

		/// <summary>
		/// 创建技能组成列表的模板方法，配合BaseFactoryEnum使用，需要实现创建自身对象，创建时调用BaseFactoryEnum. createSingleCondition，传入不同参数即可<br>
		/// 其实之前这里是全部用泛型实现的，但是C#的泛型与Java不兼容，所以还是使用了类型强转
		/// 
		/// @author liyuan2
		/// </summary>
		/// @param <T> </param>
		/// @param <E> </param>
		/// @param <DATA> </param>
		private abstract class SkillPartInfoCreater<T, E, DATA> where T : com.kx.sglm.gs.battle.share.enums.IBattlePartInfo where E : com.kx.sglm.gs.battle.share.enums.BaseBattleFactoryEnum
		{

			/// <summary>
			/// 创建Object
			/// </summary>
			/// <param name="values"> </param>
			/// <param name="obj">
			/// @return </param>
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

		protected internal static IBattlePartInfo createSingleInfo(BaseBattleFactoryEnum[] values, int key, params string[] param)
		{
			IBattlePartInfo _infoObj = null;
			if (!ArrayUtils.isRightArrayIndex(key, values))
			{
				return _infoObj;
			}
			BaseBattleFactoryEnum _enumType = values[key];
			if (_enumType == null)
			{
				return _infoObj;
			}
			_infoObj = _enumType.createInfo(param);
			return _infoObj;
		}

	}

}
using System.Collections.Generic;

namespace com.kx.sglm.gs.role.properties.amend
{


	using RoleAProperty = com.kx.sglm.gs.hero.properties.RoleAProperty;
	using AbstractAmendCalcRule = com.kx.sglm.gs.role.properties.amend.model.AbstractAmendCalcRule;
	using Amend = com.kx.sglm.gs.role.properties.amend.model.Amend;
	using AmendAddRule = com.kx.sglm.gs.role.properties.amend.model.AmendAddRule;
	using AmendMultRule = com.kx.sglm.gs.role.properties.amend.model.AmendMultRule;
	using AmendPercentRule = com.kx.sglm.gs.role.properties.amend.model.AmendPercentRule;
	using AmendTriple = com.kx.sglm.gs.role.properties.amend.model.AmendTriple;

	/// <summary>
	/// 属性修正管理器
	/// 
	/// @author fangyong
	/// @version 2014�?�?�?
	/// 
	/// </summary>
	public class AmendManagerImpl : IAmendManager
	{

		private readonly Dictionary<string, Amend> PROP_AMEND = new Dictionary<string, Amend>();

		private static readonly AbstractAmendCalcRule add = new AmendAddRule();
		private static readonly AbstractAmendCalcRule mult = new AmendMultRule();
		private static readonly AbstractAmendCalcRule percent = new AmendPercentRule();

		/// <summary>
		/// Amend规则配置的正则表达式 </summary>
		private readonly System.Text.RegularExpressions.Regex amendRulePattern;

		/// <summary>
		/// 构造属性修正规�?P+属性索�?
		/// 
		/// </summary>
		public AmendManagerImpl()
		{
			string _amendPrefix = getAmendName(RoleAProperty.TYPE);
			for (int i = 0; i < RoleAProperty._SIZE; i++)
			{
				string _amendSymbol = _amendPrefix + i;
				PROP_AMEND[_amendSymbol] = new Amend(RoleAProperty.TYPE, i, _amendSymbol);
			}
			string _calculateRulePattern = System.Text.RegularExpressions.Regex.Escape(add.Symbol) + "|" + System.Text.RegularExpressions.Regex.Escape(mult.Symbol) + "|" + System.Text.RegularExpressions.Regex.Escape(percent.Symbol);
			this.amendRulePattern = new System.Text.RegularExpressions.Regex("([A-Z]\\d+)(" + _calculateRulePattern + ")");

		}

		public virtual Amend getAmendByName(string name)
		{
			Amend _amend = PROP_AMEND[name];
			if (_amend == null)
			{
				throw new System.ArgumentException("#GS.AmendManagerImpl.getAmendByName unknown");
			}
			return _amend;
		}

		public virtual Amend getAmendByType(int type, int index)
		{
			string _amendPrefix = getAmendName(type);
			Amend _amend = PROP_AMEND[_amendPrefix + index];
			if (_amend == null)
			{
				throw new System.ArgumentException("GS.AmendManagerImpl.getAmendByType unknown");
			}
			return _amend;
		}

		public virtual string[] getAmendConfig(string content)
		{
			System.Text.RegularExpressions.Match _matcher = this.amendRulePattern.Match(content);
			if (_matcher.Success)
			{
				string[] _configs = new string[2];
				_configs[0] = _matcher.Groups[1].ToString();
				_configs[1] = _matcher.Groups[2].ToString();
				return _configs;
			}
			else
			{
				return null;
			}
		}

		public virtual AbstractAmendCalcRule getAmendRule(string operation)
		{
			if (add.Symbol.Equals(operation))
			{
				return add;
			}
			else if (mult.Symbol.Equals(operation))
			{
				return mult;
			}
			else if (percent.Symbol.Equals(operation))
			{
				return percent;
			}
			else
			{
				throw new System.ArgumentException(operation);
			}
		}

		public virtual AbstractAmendCalcRule AmendAddRule
		{
			get
			{
				return add;
			}
		}

		private string getAmendName(int type)
		{
			char _amendPrefix = (char)0;
			switch (type)
			{
			case PropertyTypeConstants.ROLE_PROP_TYPE_A:
				_amendPrefix = 'P';
				break;
			default:
				throw new System.ArgumentException("unknown amend type");
			}
			return _amendPrefix + "";
		}

		public virtual AbstractAmendCalcRule AmendMultRule
		{
			get
			{
				return mult;
			}
		}

		public virtual AbstractAmendCalcRule AmendPercentRule
		{
			get
			{
				return percent;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public com.kx.sglm.gs.role.properties.amend.model.AmendTriple createAmendTriple(String amendConfigStr, float amendVal) throws IllegalArgumentException
		public virtual AmendTriple createAmendTriple(string amendConfigStr, float amendVal)
		{
			if (null == amendConfigStr || 0 == (amendConfigStr = amendConfigStr.Trim()).Length)
			{
				return null;
			}
			// 解析config
			string[] _amendConfigs = this.getAmendConfig(amendConfigStr);
			if (null == _amendConfigs)
			{
				throw new System.ArgumentException("GS.AmendManagerImpl.createAmendTriple " + string.Format("illegal v:{0}", amendConfigStr));
			}
			// 如果修正参数的值是0,则忽略此列的配置
			if (0 == amendVal)
			{
				return null;
			}
			Amend _amend = this.getAmendByName(_amendConfigs[0]);
			if (_amend == null)
			{
				throw new System.ArgumentException("GS.AmendManagerImpl.createAmendTriple " + string.Format("type not exist v:{0}", amendConfigStr));
			}
			return new AmendTriple(_amend, this.getAmendRule(_amendConfigs[1]), amendVal);
		}
	}

}
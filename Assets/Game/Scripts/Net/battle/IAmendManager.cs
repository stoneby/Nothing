namespace com.kx.sglm.gs.role.properties.amend
{

	using AbstractAmendCalcRule = com.kx.sglm.gs.role.properties.amend.model.AbstractAmendCalcRule;
	using Amend = com.kx.sglm.gs.role.properties.amend.model.Amend;
	using AmendTriple = com.kx.sglm.gs.role.properties.amend.model.AmendTriple;

	/// <summary>
	/// Amend配置管理�?
	/// 
	/// @author fangyong
	/// @version 2014�?�?�?
	/// 
	/// </summary>
	public interface IAmendManager
	{
		/// <summary>
		/// 根据Amend的名称取得对应的Amend
		/// </summary>
		/// <param name="name">
		/// @return </param>
		Amend getAmendByName(string name);

		/// <summary>
		/// 根据属性的类型和属性索引取得对应的Amend
		/// </summary>
		/// <param name="type"> </param>
		/// <param name="index">
		/// @return </param>
		Amend getAmendByType(int type, int index);

		/// <summary>
		/// 取得用于实现加法的AmendRule
		/// 
		/// @return
		/// </summary>
		AbstractAmendCalcRule AmendAddRule {get;}

		/// <summary>
		/// 获取配置属性修正规则的配置�?
		/// </summary>
		/// <param name="content"> 属性修正规则配�?格式Amend+Operation,如A1+,A1*,A1%
		/// @return </param>
		string[] getAmendConfig(string content);

		/// <summary>
		/// 根据操作符取得修正规则的计算规则
		/// </summary>
		/// <param name="operation">
		/// @return </param>
		AbstractAmendCalcRule getAmendRule(string operation);

		AbstractAmendCalcRule AmendMultRule {get;}

		AbstractAmendCalcRule AmendPercentRule {get;}

		/// <summary>
		/// 创建一个属性修正规则三元组
		/// </summary>
		/// <param name="amendConfigStr"> 修正规则的字符串表示 </param>
		/// <param name="amendVal"> 修正的�? </param>
		/// <returns> 当amendVal==0时返回null </returns>
		/// <exception cref="IllegalArgumentException"> 当amendConfigStr不合法时 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public com.kx.sglm.gs.role.properties.amend.model.AmendTriple createAmendTriple(String amendConfigStr, float amendVal) throws IllegalArgumentException;
		AmendTriple createAmendTriple(string amendConfigStr, float amendVal);
	}

}
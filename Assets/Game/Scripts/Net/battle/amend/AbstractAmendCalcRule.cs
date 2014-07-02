namespace com.kx.sglm.gs.role.properties.amend.model
{

	using FloatNumberPropertyObject = com.kx.sglm.core.model.FloatNumberPropertyObject;
	using IntNumberPropertyObject = com.kx.sglm.core.model.IntNumberPropertyObject;

	/// <summary>
	/// 数值修正计算规则基�?
	/// 
	/// @author fangyong
	/// @version 2014�?�?�?
	/// 
	/// </summary>
	public abstract class AbstractAmendCalcRule
	{
		/// <summary>
		/// 检查该规则是否可以接受
		/// </summary>
		/// <param name="amend"> </param>
		/// <param name="intNumberPropertyObject">
		/// @return </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected boolean isAccept(final Amend amend, final com.kx.sglm.core.model.IntNumberPropertyObject intNumberPropertyObject)
		protected internal virtual bool isAccept(Amend amend, IntNumberPropertyObject intNumberPropertyObject)
		{
			if (amend.propertyType != intNumberPropertyObject.PropertyType)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// 检查该规则是否可以接受
		/// </summary>
		/// <param name="amend"> </param>
		/// <param name="floatNumberPropertyObject">
		/// @return </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected boolean isAccept(final Amend amend, final com.kx.sglm.core.model.FloatNumberPropertyObject floatNumberPropertyObject)
		protected internal virtual bool isAccept(Amend amend, FloatNumberPropertyObject floatNumberPropertyObject)
		{
			if (amend.propertyType != floatNumberPropertyObject.PropertyType)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// 计算该修正规则作用于numberPropertyObject后的结果
		/// </summary>
		/// <param name="intNumberPropertyObject"> </param>
		/// <param name="amendValue"> 修正�? </param>
		/// <returns> 返回该规则应用于numberPropertyObject后所产生的改变�? </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public abstract int calculate(final Amend amend, final com.kx.sglm.core.model.IntNumberPropertyObject intNumberPropertyObject, final int amendValue);
		public abstract int calculate(Amend amend, IntNumberPropertyObject intNumberPropertyObject, int amendValue);

		/// 
		/// <param name="floatNumberPropertyObject"> </param>
		/// <param name="amendValue">
		/// @return </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public abstract float calculate(final Amend amend, final com.kx.sglm.core.model.FloatNumberPropertyObject floatNumberPropertyObject, final float amendValue);
		public abstract float calculate(Amend amend, FloatNumberPropertyObject floatNumberPropertyObject, float amendValue);

		/// <summary>
		/// 获取修正计算规则所表示的符�?
		/// 
		/// @return
		/// </summary>
		public abstract string Symbol {get;}

	//	public String toString() {
	//		return StringUtils.obj2String(this, null);
	//	}
	}

}
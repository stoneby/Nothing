namespace com.kx.sglm.gs.role.properties.amend.model
{

	using FloatNumberPropertyObject = com.kx.sglm.core.model.FloatNumberPropertyObject;
	using IntNumberPropertyObject = com.kx.sglm.core.model.IntNumberPropertyObject;

	/// <summary>
	/// 加法修正规则
	/// 
	/// @author fangyong
	/// @version 2014�?�?�?
	/// 
	/// </summary>
	public class AmendAddRule : AbstractAmendCalcRule
	{
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public float calculate(final Amend amend, com.kx.sglm.core.model.FloatNumberPropertyObject floatNumberPropertyObject, float amendValue)
		public override float calculate(Amend amend, FloatNumberPropertyObject floatNumberPropertyObject, float amendValue)
		{
			if (!this.isAccept(amend, floatNumberPropertyObject))
			{
				throw new System.ArgumentException("property type is diffrent");
			}
			return amendValue;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public int calculate(final Amend amend, final com.kx.sglm.core.model.IntNumberPropertyObject intNumberPropertyObject, final int amendValue)
		public override int calculate(Amend amend, IntNumberPropertyObject intNumberPropertyObject, int amendValue)
		{
			if (!this.isAccept(amend, intNumberPropertyObject))
			{
				throw new System.ArgumentException("property type is diffrent");
			}
			return amendValue;
		}

		public override string Symbol
		{
			get
			{
				return "+";
			}
		}
	}

}
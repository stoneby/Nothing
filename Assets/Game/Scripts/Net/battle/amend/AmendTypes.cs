using System.Collections.Generic;

namespace com.kx.sglm.gs.role.properties.amend.model
{


	using RoleAProperty = com.kx.sglm.gs.hero.properties.RoleAProperty;

	/// <summary>
	/// 定义供策划填表用的各种修正类型，提供了策划定义的修正key与系统的一级、二级可抗性属性的映射关系
	/// 
	/// 与生成给客户端使用的RoleProperties.as的具体值一�?<seealso cref="RolePropertiesGenerator"/>
	/// 
	/// @author hongfu1
	/// 
	/// </summary>
	public sealed class AmendTypes
	{

		public static readonly int ALL_PROPERTY_COUNT = RoleAProperty._SIZE;

		/// <summary>
		/// 定义系统对角色一级属性的修正类型 </summary>
		private static readonly Dictionary<int, Amend> propertyAmends = new Dictionary<int, Amend>();

		static AmendTypes()
		{
			for (int j = 1; j < RoleAProperty._SIZE; j++)
			{
				int genKey = PropertyTypeConstants.genPropertyKey(j, RoleAProperty.TYPE);
				propertyAmends[genKey] = new Amend(RoleAProperty.TYPE, j, "P" + j);
			}
		}

		/// <summary>
		/// 根据指定的key值取得相应的修正
		/// </summary>
		/// <param name="genKey"> 该值是策划填表时使用的修正key，即段基�?偏移�?
		/// @return </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Amend getAmend(final int genKey)
		public static Amend getAmend(int genKey)
		{
			if (!propertyAmends.ContainsKey(genKey))
			{
				throw new System.ArgumentException("Not a valid amend genKey[" + genKey + "]");
			}
			return propertyAmends[genKey];
		}

	}

}
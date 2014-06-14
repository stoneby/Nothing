namespace com.kx.sglm.gs.role.properties
{

	public class PropertyTypeConstants
	{

		/// <summary>
		/// 武将的一级属�? </summary>
		public const int HERO_PROP_TYPE_A = 1;

		/// <summary>
		/// 基础属性（角色，武将）数字类型 int </summary>
		public const int BASE_ROLE_PROPS_INT = 2;

		/// <summary>
		/// 基础属性（角色，武将）非数值类�? </summary>
		public const int BASE_ROLE_PROPS_STR = 3;

		/// <summary>
		/// 角色属性：Object类型，存放在游戏过程中不改变，或者无需告知客户端的各种类型属�? </summary>
		public const int ROLE_PROPS_FINAL = 4;


		/// <summary>
		/// 产生属性的Key值，用于服务器和客户端之间数据发送接�?
		/// </summary>
		/// <param name="index"> 属性在property类中的索�? </param>
		/// <param name="propertyType"> property类的类型
		/// @return </param>
		public static int genPropertyKey(int index, int propertyType)
		{
			return propertyType * 100 + index;
		}
	}

}
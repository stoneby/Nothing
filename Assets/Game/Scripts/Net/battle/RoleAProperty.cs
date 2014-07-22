using System.Collections.Generic;

namespace com.kx.sglm.gs.hero.properties
{




	using FloatNumberPropertyObject = com.kx.sglm.core.model.FloatNumberPropertyObject;
	using PropertyTypeConstants = com.kx.sglm.gs.role.properties.PropertyTypeConstants;

	public sealed class RoleAProperty : FloatNumberPropertyObject
	{
		/// <summary>
		/// 一级属性索引�? </summary>
		private static int _INDEX = 0;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Comment(content = "攻击") @Type(Integer.class) public static final int ATK = _INDEX++;
		public static readonly int ATK = _INDEX++;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Comment(content = "HP") @Type(Integer.class) public static final int HP = _INDEX++;
		public static readonly int HP = _INDEX++;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Comment(content = "回复") @Type(Integer.class) public static final int RECOVER = _INDEX++;
		public static readonly int RECOVER = _INDEX++;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Comment(content = "MP") @Type(Integer.class) public static final int MP = _INDEX++;
		public static readonly int MP = _INDEX++;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Comment(content = "防御") @Type(Integer.class) public static final int DEFENSE = _INDEX++;
		public static readonly int DEFENSE = _INDEX++;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Comment(content = "减伤") @Type(Integer.class) public static final int DECRDAMAGE = _INDEX++;
		public static readonly int DECRDAMAGE = _INDEX++;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Comment(content = "加伤") @Type(Integer.class) public static final int INCRDAMAGE = _INDEX++;
		public static readonly int INCRDAMAGE = _INDEX++;

		/// <summary>
		/// 一级属性个�? </summary>
		public static readonly int _SIZE = _INDEX;
		public static readonly int TYPE = PropertyTypeConstants.ROLE_PROP_TYPE_A;

		public RoleAProperty() : base(_SIZE, TYPE)
		{
		}

		public static bool isValidIndex(int index)
		{
			return index >= 0 && index < RoleAProperty._SIZE;
		}

		public Dictionary<int, int> PropMaps
		{
			get
			{
				Dictionary<int, int> _propMaps = new Dictionary<int, int>();
				for (int i = 0; i < _SIZE; i++)
				{
					_propMaps[i] = this.getAsInt(i);
				}
    
				return _propMaps;
			}
		}

	}

}
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.utils
{


	using PropertyTypeConstants = com.kx.sglm.gs.role.properties.PropertyTypeConstants;

	public class FighterAProperty
	{

		//TODO: 这些常量以后使用生成器生成，暂时先编码在这个类里
		/// <summary>
		/// 一级属性索引值 </summary>
		private static int _INDEX = 0;
		/// <summary>
		/// 攻击 </summary>
		public static readonly int ATK = getBattlePropKey(_INDEX++);
		/// <summary>
		/// HP </summary>
		public static readonly int HP = getBattlePropKey(_INDEX++);
		/// <summary>
		/// 回复 </summary>
		public static readonly int RECOVER = getBattlePropKey(_INDEX++);
		/// <summary>
		/// MP </summary>
		public static readonly int MP = getBattlePropKey(_INDEX++);
		/// <summary>
		/// 防御 </summary>
		public static readonly int DEFENSE = getBattlePropKey(_INDEX++);
		/// <summary>
		/// 减伤 </summary>
		public static readonly int DECRDAMAGE = getBattlePropKey(_INDEX++);

		/// <summary>
		/// 一级属性个数 </summary>
		public static readonly int _SIZE = _INDEX;

		private Dictionary<int?, int?> props;

		public FighterAProperty()
		{
			props = new Dictionary<int?, int?>();
		}


		public static Dictionary<int?, int?> copyPropMap(FighterAProperty aProp)
		{
			return new Dictionary<int?, int?>(aProp.props);
		}

		/// <summary>
		/// 取得指定索引的int值（四舍五入的int值）
		/// </summary>
		/// <param name="index"> 属性索引
		/// @return </param>
		public int get(int index)
		{
			object _val = props[index];
			return _val == null ? 0 : (int)_val;
		}

		public virtual void addAllProp(IDictionary<int?, int?> propMap)
		{
			foreach (KeyValuePair<int?, int?> _entry in propMap)
			{
				props[_entry.Key] = _entry.Value;
			}
		}


		/// <summary>
		/// 设定指定索引的float值
		/// </summary>
		/// <param name="index"> 属性索引 </param>
		/// <param name="value"> 新值 </param>
		/// <returns> true,值被修改;false,值未修改 </returns>
		/// <exception cref="IllegalStateException"> 如果该对象处于只读状态 </exception>
		public virtual void set(int index, int value)
		{
			props[index] = value;
		}


		public static int getBattlePropKey(int key)
		{
			return PropertyTypeConstants.genPropertyKey(key, PropertyTypeConstants.HERO_PROP_TYPE_A);
		}


	}

}
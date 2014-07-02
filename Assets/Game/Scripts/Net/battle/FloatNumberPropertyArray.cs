using System;

namespace com.kx.sglm.core.model
{

	using IntFloatPair = com.kx.sglm.core.util.IntFloatPair;
	using MathUtils = com.kx.sglm.core.util.MathUtils;


	/// <summary>
	/// 浮点值属性数�?
	/// 
	/// @author fangyong
	/// @version 2014-3-4
	/// 
	/// </summary>
	public sealed class FloatNumberPropertyArray : AbstractProperty, ICloneable
	{
		/// <summary>
		/// 保存数�? </summary>
		private readonly float[] values;

		/// <summary>
		/// 创建一个有size个数据的数值属性集�?
		/// </summary>
		/// <param name="size"> 数据的个�? </param>
		public FloatNumberPropertyArray(int size) : base(size)
		{
			values = new float[size];
		}

		public FloatNumberPropertyArray(FloatNumberPropertyArray set) : base(set.size())
		{
			values = new float[set.size()];
			Array.Copy(set.values, 0, values, 0, values.Length);
			this.bitSet.set(0, values.Length, true);
		}

		/// <summary>
		/// 从指定的数值对象src拷贝数据到本实例
		/// </summary>
		/// <param name="src"> 数据的来�? </param>
		public void copyFrom(FloatNumberPropertyArray src)
		{
			Array.Copy(src.values, 0, values, 0, values.Length);
			this.bitSet.set(0, values.Length, true);
		}

		/// <summary>
		/// 设置index对应的值为value
		/// </summary>
		/// <param name="index"> </param>
		/// <param name="value"> </param>
		/// <returns> 返回值是是否确实被修�?true,修改;false,未修�? </returns>
		/// <exception cref="ArrayIndexOutOfBoundsException"> 如果index<0 或�?index>=size时会抛出此异�? </exception>
		public bool set(int index, float value)
		{
			float _o = values[index];
			if (!MathUtils.floatEquals(_o, value))
			{
				values[index] = value;
				bitSet.set(index);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 取得index对应的int�?
		/// </summary>
		/// <param name="index">
		/// @return </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> 如果index<0 或�?index>=size时会抛出此异�? </exception>
		public float get(int index)
		{
			return values[index];
		}

		/// <summary>
		/// 增加index对应的�?
		/// </summary>
		/// <param name="index"> 属性的索引 </param>
		/// <param name="value"> 将要增加的�? </param>
		/// <returns> 增加后的�? </returns>
		public float add(int index, float value)
		{
			float _o = values[index];
			if (value != 0.0f)
			{
				float _n = _o + value;
				values[index] = _n;
				bitSet.set(index);
			}
			return values[index];
		}

		/// <summary>
		/// 是否有修�?
		/// 
		/// @return
		/// </summary>
		public bool PropChanged
		{
			get
			{
				return !this.bitSet.Empty;
			}
		}

		/// <summary>
		/// 重置修改,将所有的修改标识清空
		/// </summary>
		public void resetChanged()
		{
			this.bitSet.clear();
		}

		/// <summary>
		/// 取得被修改过的的属性索�?效率较好的一个实�?
		/// 
		/// @return
		/// </summary>
		public object[] Changed
		{
			get
			{
				int[] _indexes = new int[bitSet.cardinality()];
				float[] _values = new float[bitSet.cardinality()];
				object[] changed = new object[2];
				changed[0] = _indexes;
				changed[1] = _values;
				for (int i = bitSet.nextSetBit(0), j = 0; i >= 0; i = bitSet.nextSetBit(i + 1), j++)
				{
					_indexes[j] = i;
					_values[j] = this.values[i];
				}
				return changed;
			}
		}

		/// <summary>
		/// 取得被修改过的的属性索�?
		/// 
		/// @return
		/// </summary>
		public IntFloatPair[] ChangedPair
		{
			get
			{
				IntFloatPair[] changedPairs = new IntFloatPair[bitSet.cardinality()];
				for (int i = bitSet.nextSetBit(0); i >= 0; i = bitSet.nextSetBit(i + 1))
				{
					changedPairs[i] = new IntFloatPair(Convert.ToInt32(i), Convert.ToSingle(this.values[i]));
				}
				return changedPairs;
			}
		}

		public IntFloatPair[] IndexValuePairs
		{
			get
			{
				IntFloatPair[] indexValuePairs = IntFloatPair.newKeyValuePairArray(values.Length);
				for (int i = 0; i < indexValuePairs.Length; i++)
				{
					indexValuePairs[i] = new IntFloatPair(Convert.ToInt32(i), Convert.ToSingle(values[i]));
				}
				return indexValuePairs;
			}
		}

		/// <summary>
		/// 取得属性的个数
		/// 
		/// @return
		/// </summary>
		public int size()
		{
			return this.values.Length;
		}

		/// <summary>
		/// 清空状�?将values重置�?;将所有属性都设置为changed
		/// </summary>
		public void clear()
		{
			for (int i = 0; i < values.Length; i++)
			{
				if (!MathUtils.floatEquals(values[i], 0.0f))
				{
					this.bitSet.set(i);
				}
				values[i] = 0.0f;
			}
		}

		/// <summary>
		/// 计算该数据对象的所有数值的�?
		/// 
		/// @return
		/// </summary>
		public float sum()
		{
			float _sum = 0;
			for (int i = 0; i < this.values.Length; i++)
			{
				_sum += this.values[i];
			}
			return _sum;
		}

		/// <summary>
		/// 计算由指定的索引数组标识的属性数值的�?
		/// </summary>
		/// <param name="indexs"> 被计算的属性的索引数组
		/// @return </param>
		public float sum(int[] indexs)
		{
			float _sum = 0;
			for (int i = 0; i < indexs.Length; i++)
			{
				_sum += this.values[indexs[i]];
			}
			return _sum;
		}

		/// <summary>
		/// 计算除了指定的索引数组标识的以外的属性数值的�?
		/// </summary>
		/// <param name="exceptIndexs"> 被排除的属性的索引数组
		/// @return </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public float sumExcept(final int[] exceptIndexs)
		public float sumExcept(int[] exceptIndexs)
		{
			float _sum = 0;
			for (int i = 0; i < values.Length; i++)
			{
				_sum += this.values[i];
			}
			for (int i = 0; i < exceptIndexs.Length; i++)
			{
				_sum -= this.values[exceptIndexs[i]];
			}
			return _sum;
		}

		/// <returns> the values </returns>
		public float[] Values
		{
			get
			{
				return values;
			}
		}


		public FloatNumberPropertyArray clone()
		{
			FloatNumberPropertyArray newGuy = new FloatNumberPropertyArray(this);
			return newGuy;
		}

		//for cshap
		public object Clone()
		{
			return null;
		}


	}

}
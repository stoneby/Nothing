namespace com.kx.sglm.core.model
{

	using IntFloatPair = com.kx.sglm.core.util.IntFloatPair;
	using MathUtils = com.kx.sglm.core.util.MathUtils;

	/// <summary>
	/// 浮点值属性对�?
	/// 
	/// @author fangyong
	/// @version 2014-3-4
	/// 
	/// </summary>
	public class FloatNumberPropertyObject
	{
		protected internal readonly FloatNumberPropertyArray props;
		protected internal readonly int propertyType;

		/// <summary>
		/// 是否可以修改 </summary>
		private bool isReadOnly = false;

		/// <summary>
		/// 属性的个数
		/// </summary>
		/// <param name="size"> </param>
		public FloatNumberPropertyObject(int size, int propertyType)
		{
			this.props = new FloatNumberPropertyArray(size);
			this.propertyType = propertyType;
		}

		/// <summary>
		/// 从指定的参数中拷贝数据到本身<br>
		/// 注：此方法change全部的Bit位，如果需要精确比较，请使�?<seealso cref="#copyFromAndCompare(FloatNumberPropertyObject)"/>
		/// </summary>
		/// <param name="src"> </param>
		/// <exception cref="IllegalArgumentException"> 如果src的对象类型与该类型不一�? </exception>
		/// <exception cref="IllegalStateException"> 如果该对象处于只读状�? </exception>
		public virtual void copyFrom(FloatNumberPropertyObject src)
		{
			if (src.propertyType != this.propertyType)
			{
				throw new System.ArgumentException("Not the same property type.");
			}
			if (!isReadOnly)
			{
				this.props.copyFrom(src.props);
			}
			else
			{
				throw new System.NotSupportedException("PROP_READONLY");
			}
		}

		/// <summary>
		/// 向指定的参数拷贝数据
		/// </summary>
		/// <param name="target"> </param>
		/// <exception cref="IllegalArgumentException"> 如果target的对象类型与该类型不一�? </exception>
		public virtual void copyTo(FloatNumberPropertyObject target)
		{
			if (target.propertyType != this.propertyType)
			{
				throw new System.ArgumentException("Not the same property type.");
			}
			target.props.copyFrom(this.props);
		}

		/// <summary>
		/// 将本对象的指定的属性拷贝对目标对象�?
		/// </summary>
		/// <param name="target"> 目标对象 </param>
		/// <param name="props"> 属性索�? </param>
		public virtual void copyTo(FloatNumberPropertyObject target, int[] props)
		{
			for (int i = 0; i < props.Length; i++)
			{
				target.set(props[i], this.get(props[i]));
			}
		}

		/// <summary>
		/// 将指定参数中的数据加到本身对应索引中
		/// </summary>
		/// <param name="src"> </param>
		/// <exception cref="IllegalArgumentException"> 如果src的对象类型与该类型不一�? </exception>
		/// <exception cref="IllegalStateException"> 如果该对象处于只读状�? </exception>
		public virtual void add(FloatNumberPropertyObject src)
		{
			addBySign(src, 1);
		}

		/// <summary>
		/// 从本身减去将指定参数中的数据
		/// </summary>
		/// <param name="src"> </param>
		/// <exception cref="IllegalArgumentException"> 如果src的对象类型与该类型不一�? </exception>
		/// <exception cref="IllegalStateException"> 如果该对象处于只读状�? </exception>
		public virtual void dec(FloatNumberPropertyObject src)
		{
			addBySign(src, -1);
		}

		/// <summary>
		/// 为本身加上或减去将指定参数中的数�?
		/// </summary>
		/// <param name="src"> </param>
		/// <param name="sign"> 1 or -1 (�?�? </param>
		/// <exception cref="IllegalArgumentException"> 如果src的对象类型与该类型不一�? </exception>
		/// <exception cref="IllegalStateException"> 如果该对象处于只读状�? </exception>
		private void addBySign(FloatNumberPropertyObject src, int sign)
		{
			if (src.propertyType != this.propertyType)
			{
				throw new System.ArgumentException("Not the same property type.");
			}
			if (!isReadOnly)
			{
				for (int i = 0; i < size(); i++)
				{
					add(i, sign * src.get(i));
				}
			}
			else
			{
				throw new System.NotSupportedException("PROP_READONLY");
			}
		}

		/// <summary>
		/// 被修改过的属性索引及其�?
		/// 
		/// @return
		/// </summary>
		public virtual object[] Changed
		{
			get
			{
				return props.Changed;
			}
		}

		/// <summary>
		/// 被修改过的属性索引及其�?
		/// 
		/// @return
		/// </summary>
		public virtual IntFloatPair[] ChangedPair
		{
			get
			{
				return props.ChangedPair;
			}
		}

		public virtual IntFloatPair[] IndexValuePairs
		{
			get
			{
				return props.IndexValuePairs;
			}
		}

		public virtual bool PropChanged
		{
			get
			{
				return props.PropChanged;
			}
		}

		public virtual bool isPropChanged(int index)
		{
			return props.isChanged(index);
		}

		/// <summary>
		/// 属性的个数
		/// 
		/// @return
		/// </summary>
		public int size()
		{
			return props.size();
		}

		/// <summary>
		/// 取得指定索引的float�?
		/// </summary>
		/// <param name="index"> 属性索�?
		/// @return </param>
		public float get(int index)
		{
			return props.get(index);
		}

		/// <summary>
		/// 取得指定索引的int值（四舍五入的int值）
		/// </summary>
		/// <param name="index"> 属性索�?
		/// @return </param>
		public int getAsInt(int index)
		{
			return MathUtils.float2Int(props.get(index));
		}

		/// <summary>
		/// 设定指定索引的float�?
		/// </summary>
		/// <param name="index"> 属性索�? </param>
		/// <param name="value"> 新�? </param>
		/// <returns> true,值被修改;false,值未修改 </returns>
		/// <exception cref="IllegalStateException"> 如果该对象处于只读状�? </exception>
		public bool set(int index, float value)
		{
			if (!isReadOnly)
			{
				return props.set(index, value);
			}
			else
			{
				throw new System.NotSupportedException("PROP_READONLY");
			}
		}

		/// <summary>
		/// 清空所有的属�?将所有的属性设置为0
		/// </summary>
		/// <exception cref="IllegalStateException"> 如果该对象处于只读状�? </exception>
		public virtual void clear()
		{
			if (!isReadOnly)
			{
				this.props.clear();
			}
			else
			{
				throw new System.NotSupportedException("PROP_READONLY");
			}
		}

		/// <summary>
		/// 将指定索�?tt>index</tt>的属性值加<tt>value</tt>
		/// </summary>
		/// <param name="index"> </param>
		/// <param name="value"> </param>
		/// <returns> 返回相加后的结果 </returns>
		public float add(int index, float value)
		{
			if (!isReadOnly)
			{
				return props.add(index, value);
			}
			else
			{
				throw new System.NotSupportedException("PROP_READONLY");
			}
		}

		/// <summary>
		/// 将修改标志重新置�?
		/// </summary>
		public void resetChanged()
		{
			props.resetChanged();
		}

		/// <summary>
		/// 获取该对象的类型
		/// 
		/// @return
		/// </summary>
		public int PropertyType
		{
			get
			{
				return propertyType;
			}
		}

		/// <summary>
		/// 将所有的属性以int类型相加
		/// 
		/// @return
		/// </summary>
		public virtual float sum()
		{
			return this.props.sum();
		}

		/// <summary>
		/// 将由属性索引数组index指定的属性相�?
		/// </summary>
		/// <param name="index"> 属性的索引
		/// @return </param>
		public virtual float sum(int[] index)
		{
			return this.props.sum(index);
		}

		/// <summary>
		/// 计算除了指定的索引数组标识的以外的属性数值的�?
		/// </summary>
		/// <param name="exceptIndexs"> 被排除的属性索引数�?
		/// @return </param>
		public virtual float sumExcept(int[] exceptIndexs)
		{
			return props.sumExcept(exceptIndexs);
		}

		/// <summary>
		/// 将该数值对象设置为只读状�?从调用此方法的一刻起,该数值对象的值将不能够再被修�?适用于对象的值一旦设置完成后,而且以后不会再进行修�?
		/// </summary>
		public virtual void readOnly()
		{
			this.isReadOnly = true;
		}

		/// <summary>
		/// 拷贝并比�?br>
		/// 只设置改变的值的Bit�?如不关心change，请使用 <seealso cref="#copyFrom(FloatNumberPropertyObject)"/>
		/// </summary>
		/// <param name="src"> </param>
		/// <exception cref="IllegalArgumentException"> 如果src的对象类型与该类型不一�? </exception>
		/// <exception cref="IllegalStateException"> 如果该对象处于只读状�? </exception>
		public virtual void copyFromAndCompare(FloatNumberPropertyObject src)
		{
			if (src.propertyType != this.propertyType)
			{
				throw new System.ArgumentException("Not the same property type.");
			}
			int _size = this.size();
			for (int i = 0; i < _size; i++)
			{
				this.set(i, src.get(i));
			}
		}

		/// <summary>
		/// 将当前的修改标识填充到toPropObj�?
		/// <para>
		/// <b>注意：此方法只在战斗中使用，其它情况“不建议”使�?/b>
		/// </para>
		/// </summary>
		/// <param name="toPropObj"> </param>
		public virtual void fillChangedBit(FloatNumberPropertyObject toPropObj)
		{
			if (toPropObj.propertyType != this.propertyType)
			{
				throw new System.ArgumentException("Not the same property type.");
			}
			if (!isReadOnly)
			{
				this.props.fillChangedBit(toPropObj.props);
			}
			else
			{
				throw new System.NotSupportedException("PROP_READONLY");
			}
		}

		public override string ToString()
		{
			return props.ToString();
		}

		/// <summary>
		/// 总值计算产生变�?
		/// </summary>
		/// <param name="index"> </param>
		public virtual void totalValueChange(int index)
		{
			props.totalValueChange(index);
		}

	}

}
namespace com.kx.sglm.core.model
{


	/// <summary>
	/// 整形属性对�?
	/// 
	/// @author fangyong
	/// @version 2014-3-4
	/// 
	/// </summary>
	public class IntNumberPropertyObject
	{
		protected internal readonly IntNumberPropertyArray props;
		protected internal readonly int propertyType;

		/// <summary>
		/// 是否可以修改 </summary>
		private bool isReadOnly = false;

		/// <summary>
		/// 属性的个数
		/// </summary>
		/// <param name="size"> </param>
		public IntNumberPropertyObject(int size, int propertyType)
		{
			this.props = new IntNumberPropertyArray(size);
			this.propertyType = propertyType;
		}

		/// <summary>
		/// 从指定的参数中拷贝数据到本身
		/// </summary>
		/// <param name="src"> </param>
		/// <exception cref="IllegalArgumentException"> 如果src的对象类型与该类型不一�? </exception>
		/// <exception cref="IllegalStateException"> 如果该对象处于只读状�? </exception>
		public void copyFrom(IntNumberPropertyObject src)
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
		public void copyTo(IntNumberPropertyObject target)
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
		public void copyTo(IntNumberPropertyObject target, int[] props)
		{
			for (int i = 0; i < props.Length; i++)
			{
				target.set(props[i], this.get(props[i]));
			}
		}

		/// <summary>
		/// 是否有修�?
		/// 
		/// @return
		/// </summary>
		public virtual bool PropChanged
		{
			get
			{
				return props.PropChanged;
			}
		}

		/// <summary>
		/// 被修改过的属性索引及其�? 优化修改过的索引及值，返回统一的Object[]
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
		/// 属性的个数
		/// 
		/// @return
		/// </summary>
		public int size()
		{
			return props.size();
		}

		/// <summary>
		/// 取得指定索引的int�?
		/// </summary>
		/// <param name="index"> 属性索�?
		/// @return </param>
		public int get(int index)
		{
			return props.get(index);
		}

		/// <summary>
		/// 设定指定索引的int�?
		/// </summary>
		/// <param name="index"> 属性索�? </param>
		/// <param name="value"> 新�? </param>
		/// <returns> true,值被修改;fase,值未被修�? </returns>
		/// <exception cref="IllegalStateException"> 如果该对象处于只读状�? </exception>
		public bool set(int index, int value)
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
		public int add(int index, int value)
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
		/// 将指定参数中的数据加到本身对应索引中
		/// </summary>
		/// <param name="src"> </param>
		/// <exception cref="IllegalArgumentException"> 如果src的对象类型与该类型不一�? </exception>
		/// <exception cref="IllegalStateException"> 如果该对象处于只读状�? </exception>
		public void add(IntNumberPropertyObject src)
		{
			addBySign(src, 1);
		}

		/// <summary>
		/// 从本身减去将指定参数中的数据
		/// </summary>
		/// <param name="src"> </param>
		/// <exception cref="IllegalArgumentException"> 如果src的对象类型与该类型不一�? </exception>
		/// <exception cref="IllegalStateException"> 如果该对象处于只读状�? </exception>
		public void dec(IntNumberPropertyObject src)
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
		private void addBySign(IntNumberPropertyObject src, int sign)
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
		/// 将修改标志重新置�?
		/// </summary>
		public virtual void resetChanged()
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
		public virtual int sum()
		{
			return this.props.sum();
		}

		/// <summary>
		/// 将由属性索引数组index指定的属性相�?
		/// </summary>
		/// <param name="index"> 属性的索引
		/// @return </param>
		public virtual int sum(int[] index)
		{
			return this.props.sum(index);
		}

		/// <summary>
		/// 计算除了指定的索引数组标识的以外的属性数值的�?
		/// </summary>
		/// <param name="exceptIndexs"> 被排除的属性索引数�?
		/// @return </param>
		public virtual int sumExcept(int[] exceptIndexs)
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

		public override string ToString()
		{
			return props.ToString();
		}
	}

}
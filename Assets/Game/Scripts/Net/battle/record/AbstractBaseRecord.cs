using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.data.record
{

	/// <summary>
	/// 基础记录
	/// @author liyuan2
	/// 
	/// </summary>
	public abstract class AbstractBaseRecord : IBattleRecord
	{

		protected internal Dictionary<int?, object> prop;


		public AbstractBaseRecord()
		{
			prop = new Dictionary<int?, object>();
		}

		public virtual void addProp(int key, int value)
		{
			prop[key] = value;
		}

		public virtual void addProp(int key, string value)
		{
			prop[key] = value;
		}

		public virtual int getIntProp(int key)
		{
			return Convert.ToInt32(getStrProp(key));
		}

		public virtual object getProp(int key)
		{
			return prop[key];
		}

		public virtual string getStrProp(int key)
		{
			return Convert.ToString(getProp(key));
		}

		public virtual string toReportStr()
		{
			// TODO Auto-generated method stub
			return null;
		}

	}

}
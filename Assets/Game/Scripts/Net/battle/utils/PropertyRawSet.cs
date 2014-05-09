using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.utils
{


	public class PropertyRawSet
	{

		private readonly IDictionary<int, object> p;

		public PropertyRawSet()
		{
			p = new Dictionary<int, object>(0);
		}

		public PropertyRawSet(PropertyRawSet pset)
		{
			p = new Dictionary<int, object>(pset.p);
		}

		public PropertyRawSet(IDictionary<int, object> pset)
		{
			p = new Dictionary<int, object>(pset);
		}

		public virtual void set(int key, object val)
		{
			p[key] = val;
		}

		public virtual object get(int key)
		{
			return p[key];
		}

		public virtual int getInt(int key, int defVal)
		{
			if (!contains(key))
			{
				return defVal;
			}
			return (int)get(key);
		}

		public virtual long getLong(int key, long defVal)
		{
			if (!contains(key))
			{
				return defVal;
			}
			return (long)get(key);
		}

		public virtual float getFloat(int key, float defVal)
		{
			if (!contains(key))
			{
				return defVal;
			}
			return (float)get(key);

		}

		public virtual string getString(int key, string defVal)
		{
			if (!contains(key))
			{
				return defVal;
			}
			return get(key).ToString();
		}

		public virtual short getShort(int key, short defVal)
		{
			if (!contains(key))
			{
				return defVal;
			}
			return (short)get(key);
		}

		public virtual bool contains(int key)
		{
			return p.ContainsKey(key);
		}

		public virtual object remove(int key)
		{
			return p.Remove(key);
		}

		public virtual int size()
		{
			return p.Count;
		}



	}

}
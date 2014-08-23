using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.buff.effect
{


	public abstract class AbstractBuffEffect : IBuffAction
	{

		private int id;
		private int buffFlag;
		private int typeA;
		private int typeB;
		private int priority_Renamed;
		private int cdRound;
		private int buffShowId;
		private int sceneClear;
		private int maxStackingCount;



		public override int Id
		{
			set
			{
				this.id = value;
			}
			get
			{
				return id;
			}
		}

		public override int BuffFlag
		{
			set
			{
				this.buffFlag = value;
			}
		}

		public override int TypeA
		{
			set
			{
				this.typeA = value;
			}
			get
			{
				return typeA;
			}
		}

		public override int TypeB
		{
			set
			{
				this.typeB = value;
			}
			get
			{
				return typeB;
			}
		}

		public override int Priority
		{
			set
			{
				this.priority_Renamed = value;
			}
		}

		public override int CDRound
		{
			set
			{
				this.cdRound = value;
			}
			get
			{
				return cdRound;
			}
		}




		public override int priority()
		{
			return priority_Renamed;
		}



		public override bool Buff
		{
			get
			{
				return buffFlag > 0;
			}
		}


		public override int BuffShowId
		{
			get
			{
				return buffShowId;
			}
			set
			{
				this.buffShowId = value;
			}
		}



		public override int MaxStackingCount
		{
			get
			{
				return maxStackingCount;
			}
			set
			{
				this.maxStackingCount = value;
			}
		}



		public override int SceneClear
		{
			get
			{
				return sceneClear;
			}
			set
			{
				this.sceneClear = value;
			}
		}


		public override bool SceneClearBuff
		{
			get
			{
				return SceneClear > 0;
			}
		}

		public virtual void build(List<string> param)
		{
			if (param == null)
			{
				param = new List<string>();
			}
			string[] _paramArr = new string[param.Count];
			for (int _i = 0; _i < param.Count; _i++)
			{
				_paramArr[_i] = param[_i];
			}
			build(_paramArr);
		}

	}

}
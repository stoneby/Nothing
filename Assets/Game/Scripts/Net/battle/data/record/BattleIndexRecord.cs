using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.data.record
{

	/// <summary>
	/// 战斗玩家顺序
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleIndexRecord : BaseViewRecord<SingleActionRecord>
	{

		/// <summary>
		/// 攻击者ID顺序列表，战斗内的Index指这个
		/// </summary>
		private List<PointRecord> allPointList;
		/// <summary>
		/// 作用队伍
		/// </summary>
		private int targetSide;
		/// <summary>
		/// 颜色列表
		/// </summary>
		private List<PointRecord> fillPointList;


		public BattleIndexRecord()
		{
			allPointList = new List<PointRecord>();
			fillPointList = new List<PointRecord>();
		}


		public virtual List<PointRecord> FillPointList
		{
			get
			{
				return fillPointList;
			}
		}

		public virtual List<PointRecord> AllPointList
		{
			get
			{
				return allPointList;
			}
		}

		public virtual void addFillPointList(int fighterIndex, int fighterColor)
		{
			addPoint(fighterIndex, fighterColor, fillPointList);
			setModified();
		}

		public virtual void addPointList(int fighterIndex, int fighterColor)
		{
			addPoint(fighterIndex, fighterColor, allPointList);
			setModified();
		}

		protected internal virtual void addPoint(int fighterIndex, int fighterColor, List<PointRecord> list)
		{
			PointRecord _record = new PointRecord(fighterIndex, fighterColor);
			list.Add(_record);
			setModified();
		}


		public virtual int TargetSide
		{
			get
			{
				return targetSide;
			}
			set
			{
				this.targetSide = value;
				setModified();
			}
		}


		public override void show(IBattleView viewObj)
		{
			viewObj.showBattleIndexRecord(this);
		}

		internal override SingleActionRecord createRecord()
		{
			return new SingleActionRecord();
		}

		public override string toReportStr()
		{
			// TODO Auto-generated method stub
			return null;
		}

	}

}
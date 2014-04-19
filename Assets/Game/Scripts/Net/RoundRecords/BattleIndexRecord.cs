using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.data.record
{

	/// <summary>
	/// 战斗玩家顺序
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleIndexRecord : BaseViewRecord
	{

		/// <summary>
		/// 攻击者ID顺序列表，战斗内的Index指这个
		/// </summary>
		private List<int?> fighterIndexList;
		/// <summary>
		/// 带有技能效果的攻击者列表
		/// </summary>
		private List<int?> skillFighterList;
		/// <summary>
		/// 颜色列表
		/// </summary>
		private List<int?> colorIndexList;

		public virtual List<int?> getFighterIndexList()
		{
			return fighterIndexList;
		}

		public virtual void setFighterIndexList(int fighterIndex)
		{
			this.fighterIndexList.Add(fighterIndex);
		}

		public virtual List<int?> SkillFighterList
		{
			get
			{
				return skillFighterList;
			}
		}

		public virtual void addSkillFighterList(int skillFighter)
		{
			this.skillFighterList.Add(skillFighter);
		}

		public virtual List<int?> getColorIndexList()
		{
			return colorIndexList;
		}

		public virtual void setColorIndexList(int colorIndex)
		{
			this.colorIndexList.Add(colorIndex);
		}

		public override void show(IBattleView viewObj)
		{
			viewObj.showBattleIndexRecord(this);
		}

	}

}
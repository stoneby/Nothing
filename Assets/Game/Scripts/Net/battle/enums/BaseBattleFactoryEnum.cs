namespace com.kx.sglm.gs.battle.share.enums
{


	public abstract class BaseBattleFactoryEnum : BaseEnum
	{

		public BaseBattleFactoryEnum(int index) : base(index)
		{
		}

		public abstract IBattlePartInfo createInstance();

		public virtual IBattlePartInfo createInfo(params string[] param)
		{
			IBattlePartInfo _inst = createInstance();
			_inst.build(param);
			return _inst;
		}

	}

}
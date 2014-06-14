namespace com.kx.sglm.gs.battle.share.skill.creater
{

	using BaseEnum = com.kx.sglm.gs.battle.share.enums.BaseEnum;

	public abstract class BaseSkillFactoryEnum : BaseEnum
	{

		public BaseSkillFactoryEnum(int index) : base(index)
		{
		}

		internal abstract ISkillPartInfo createInstance();

		public virtual ISkillPartInfo createInfo(params int[] param)
		{
			ISkillPartInfo _inst = createInstance();
			_inst.build(param);
			return _inst;
		}

	}

}
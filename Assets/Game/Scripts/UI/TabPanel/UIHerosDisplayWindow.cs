using KXSGCodec;
using UnityEngine;

public class UIHerosDisplayWindow : TabPanelBase
{
    #region Public Fields

    //public static bool IsCreateOne;

    #endregion 

    #region Private Fields

    private sbyte cachedOrderType;
    private SCHeroList scHeroList;
    //private UIEventListener createOneLis;

    #endregion

    #region Window

    public override void OnEnter()
    {
        base.OnEnter();
        //createOneLis = UIEventListener.Get(Utils.FindChild(transform, "Button-CreateOne").gameObject);
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        cachedOrderType = scHeroList.OrderType;
        //IsCreateOne = false;
    }

    public override void OnExit()
    {
        base.OnExit();
        if (scHeroList.OrderType != cachedOrderType)
        {
            var csmsg = new CSHeroChangeOrder { OrderType = scHeroList.OrderType };
            NetManager.SendMessage(csmsg);
        }
    }

    #endregion

    #region Private Methods

    #endregion
}

using KXSGCodec;
using UnityEngine;

public class UIHerosDisplayWindow : TabPanelBase
{
    #region Public Fields

    public static bool IsCreateOne;

    #endregion 

    #region Private Fields

    private sbyte cachedOrderType;
    private SCHeroList scHeroList;
    private UIEventListener createOneLis;

    #endregion

    #region Window

    public override void OnEnter()
    {
        base.OnEnter();
        createOneLis = UIEventListener.Get(Utils.FindChild(transform, "Button-CreateOne").gameObject);
        createOneLis.onClick = OnCreateOne;
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        cachedOrderType = scHeroList.OrderType;
        IsCreateOne = false;
    }

    public override void OnExit()
    {
        base.OnExit();
        createOneLis.onClick = null;
        if (scHeroList.OrderType != cachedOrderType)
        {
            var csmsg = new CSHeroChangeOrder { OrderType = scHeroList.OrderType };
            NetManager.SendMessage(csmsg);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// The create one button click handler.
    /// </summary>
    /// <param name="go">The sender of click event.</param>
    private void OnCreateOne(GameObject go)
    {
        IsCreateOne = true;
        var csmsg = new CSHeroCreateOne();
        NetManager.SendMessage(csmsg);
    }

    #endregion
}

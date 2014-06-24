using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class LoginServersWindow : Window
{
    private GameObject BtnReturn;
    private GameObject AllServerContainer;
    private GameObject UsedServerContainer;

    private GameObject ServerItemPrefab;


    private UIEventListener BtnAllServerEventListener;
    private UIEventListener BtnCloseUIEventListener;

    private bool HaveInit = false;
    #region Window

    public override void OnEnter()
    {
        BtnCloseUIEventListener.onClick += OnCloseButtonClick;

        if (HaveInit) return;
        HaveInit = true;
        var arr = ServiceManager.GetUsedServers();
        if (arr != null)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                var item = NGUITools.AddChild(UsedServerContainer, ServerItemPrefab);
                var col = item.GetComponent<ServerItemControl>();
                col.SetData(arr[i]);
            }
        }

        for (int i = 0; i < ServiceManager.AllServerArray.Count; i++)
        {
            var item = NGUITools.AddChild(AllServerContainer, ServerItemPrefab);
            var col = item.GetComponent<ServerItemControl>();
            col.SetData(ServiceManager.AllServerArray[i]);
        }
    }

    public override void OnExit()
    {
        if (BtnCloseUIEventListener != null) BtnCloseUIEventListener.onClick -= OnCloseButtonClick;
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        BtnReturn = transform.FindChild("Image Button - back").gameObject;
        BtnReturn.AddComponent<CloseServersHandler>();

        AllServerContainer = transform.FindChild("Scroll View/AllItemContainer").gameObject;
        UsedServerContainer = transform.FindChild("Scroll View Used/UsedItemContainer").gameObject;
        ServerItemPrefab = Resources.Load("Prefabs/Component/ServerItem") as GameObject;

        //BtnAllServerEventListener = UIEventListener.Get(BtnRegister);
        BtnCloseUIEventListener = UIEventListener.Get(BtnReturn);
    }

    #endregion

    private void OnCloseButtonClick(GameObject game)
    {
        WindowManager.Instance.Show(typeof(LoginMainWindow), true);
    }
}

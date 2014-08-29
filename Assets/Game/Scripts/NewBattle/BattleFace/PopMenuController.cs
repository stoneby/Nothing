using Assets.Game.Scripts.Common.Model;
using KXSGCodec;
using UnityEngine;

public class PopMenuController : MonoBehaviour
{
    public bool IsVisible
    {
        get { return gameObject.activeSelf; }
    }

    public void Show(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void OnButtonCancel()
    {
        gameObject.SetActive(false);
    }

    public void OnButtonExit()
    {
        // clean up all battle fields.
        BattleResultHelper.Cleanup();

        var msg = new CSBattlePveFinishMsg
        {
            Uuid = BattleModelLocator.Instance.Uuid,
            BattleResult = 0,
            Star = 0
        };

        PersistenceHandler.IsRaidFinish = true;
        
        //Battle persistence
        PersistenceHandler.Instance.StoreBattleEndMessage(msg);

        NetManager.SendMessage(msg);
        MtaManager.TrackEndPage(MtaType.BattleScreen);

        //Battle persistence:Check battle end succeed.
        
        gameObject.SetActive(false);
        MissionModelLocator.Instance.ShowRaidWindow();
    }
}

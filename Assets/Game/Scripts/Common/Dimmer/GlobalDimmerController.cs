using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// Global dimmer controller, used for dim under parent game object and return it back.
/// </summary>
public class GlobalDimmerController : Singleton<GlobalDimmerController>
{
    #region Public Fields

    public UIWidget Dimmer;
    public UIWidget Dimmer2D;
    public GameObject DetectObject;

    public Vector3 LeftMoveVec;

    public bool Transparent
    {
        set
        {
            Dimmer.color = new Color(Dimmer.color.r, Dimmer.color.g, Dimmer.color.b, ((value) ? TransparentAlpha : defaultAlpha));
            Dimmer2D.color = new Color(Dimmer2D.color.r, Dimmer2D.color.g, Dimmer2D.color.b, ((value) ? TransparentAlpha : defaultAlpha));
        }
    }

    #endregion

    #region Private Fields

    private float defaultAlpha;
    private const float TransparentAlpha = 0.01f;
    private UICamera.EventType eventType;
    private UICamera uiCamera;


    #endregion

    #region Public Methods

    public void Show(bool flag)
    {
        if (uiCamera.eventType != UICamera.EventType.UI)
        {
            gameObject.SetActive(flag);
            Dimmer2D.gameObject.SetActive(flag);
            Dimmer.gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(flag);
            Dimmer2D.gameObject.SetActive(true);
            Dimmer.gameObject.SetActive(flag);
        }

        if (flag)
        {
            // "NormalBlink" case which is more generic like button click.
            if (GreenHandGuideHandler.Instance.ConfigMode != "NormalMove")
            {
                if (!DetectObject)
                {
                    return;
                }
                var listener = UIEventListener.Get(DetectObject);
                listener.onClick += OnDetectClick;
            }
            else
            {
                Dimmer.transform.localPosition -= LeftMoveVec;
            }
        }
    }
    public void MouseCheck()
    {
        // Cast a ray into the screen
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (uiCamera.eventType != UICamera.EventType.UI)
        {
            var hit2DList = Physics2D.RaycastAll(ray.origin, ray.direction);
            foreach (var hit2D in hit2DList)
            {
                Debug.LogWarning("Hit: " + hit2D.collider.gameObject.name);
                var hitGO = hit2D.collider.gameObject;
                if (hitGO == DetectObject)
                {
                    Debug.LogWarning("Hit YOU!!!!: " + hit2D.collider.gameObject.name);

                    // release current handle for MouseCheck callback, since the current null check in onClick() callback in EventTrigger.
                    UIEventTrigger.current = null;
                    DetectObject.SendMessage("OnClick", null, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        else if (uiCamera.eventType == UICamera.EventType.UI)
        {
            var hitList = Physics.RaycastAll(ray);
            foreach (var hit in hitList)
            {
                Debug.LogWarning("Hit: " + hit.collider.gameObject.name);
                var hitGO = hit.collider.gameObject;
                if (hitGO == DetectObject)
                {
                    Debug.LogWarning("Hit YOU!!!!: " + hit.collider.gameObject.name);

                    // release current handle for MouseCheck callback, since the current null check in onClick() callback in EventTrigger.
                    UIEventTrigger.current = null;
                    DetectObject.SendMessage("OnClick", null, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }

    #endregion

    #region Private Fields

    private void OnDetectClick(GameObject go)
    {
        var listener = UIEventListener.Get(DetectObject);
        listener.onClick -= OnDetectClick;

        DetectObject = null;
        //Show(false);
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        defaultAlpha = Dimmer.alpha;
        uiCamera = Camera.main.GetComponent<UICamera>();
    }

    #endregion
}

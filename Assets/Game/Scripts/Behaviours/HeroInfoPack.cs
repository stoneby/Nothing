using UnityEngine;

public class HeroInfoPack : MonoBehaviour
{
    [HideInInspector]
    public int TemplateId;

    private void Start()
    {
        var heroInfoLis = UIEventListener.Get(gameObject);
        heroInfoLis.onClick += OnHeroInfoClicked;
    }

    private void OnHeroInfoClicked(GameObject go)
    {
        UIHeroInfoWindow.TemplateId = TemplateId;
        WindowManager.Instance.Show(typeof(UIHeroInfoWindow), true);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class ExtendBag : MonoBehaviour
{
    private string extendContentKey;
    public string ExtendContentKey
    {
        set
        {
            extendContentKey = value;
            extendContentLabel.text = LanguageManager.Instance.GetTextValue(value);
        }
        get { return extendContentKey; }
    }

    private string extendLimitKey;
    public string ExtendLimitKey
    {
        set
        {
            extendContentKey = value;
            extendLimitLabel.text = LanguageManager.Instance.GetTextValue(value);
        }
        get { return extendContentKey; }
    }

    private int extendPerTime;
    private int extendCounts;
    private int extendTimes;
    private int diamond;
    private int maxExtendSize;
    private UIEventListener okLis;
    private UIEventListener cancelLis;
    private UIEventListener increaseLis;
    private UIEventListener decreaseLis;

    private UILabel diamondLabel;
    private UILabel bagCountLabel;
    private UILabel titleBagCountLabel;
    private Dictionary<int, int> costDict;
    private UILabel extendContentLabel;
    private UILabel extendLimitLabel;

    public event UIEventListener.VoidDelegate OkClicked;
    public event UIEventListener.VoidDelegate CancelClicked;

    private short extendSize;

    public short ExtendSize
    {
        get { return extendSize; }

        private set
        {
            if(value != extendSize)
            {
                extendCounts = extendPerTime * value;
                bagCountLabel.text = extendCounts.ToString();
                titleBagCountLabel.text = extendCounts.ToString();
                if(extendSize < value)
                {
                    for(int i = extendSize + 1; i <= value; i++)
                    {
                        diamond += costDict[i + extendTimes];
                    }
                }
                if(value < extendSize)
                {
                    for(int i = value + 1; i <= extendSize; i++)
                    {
                        diamond -= costDict[i + extendTimes];
                    }
                }
                diamondLabel.text = diamond.ToString();
                extendSize = value;
            }
        }
    }

    public void Init(int extendTimes, int extendPerTime, Dictionary<int, int> costDict)
    {
        this.extendTimes = extendTimes;
        this.extendPerTime = extendPerTime;
        this.costDict = costDict;
        maxExtendSize = costDict.Count - extendTimes;
        Reset();
    }

    private void Reset()
    {
        ExtendSize = 1;
        decreaseLis.GetComponent<UIButton>().isEnabled = false;
    }

    private void Awake()
    {
        okLis = UIEventListener.Get(transform.Find("OK").gameObject);
        cancelLis = UIEventListener.Get(transform.Find("Cancel").gameObject);
        increaseLis = UIEventListener.Get(transform.Find("Increase").gameObject);
        decreaseLis = UIEventListener.Get(transform.Find("Decrease").gameObject);
        diamondLabel = Utils.FindChild(transform, "Diamond").GetComponent<UILabel>();
        bagCountLabel = Utils.FindChild(transform, "BagCount").GetComponent<UILabel>();
        titleBagCountLabel = Utils.FindChild(transform, "TitleBagCount").GetComponent<UILabel>();
        extendContentLabel = Utils.FindChild(transform, "ExtendContent").GetComponent<UILabel>();
        extendLimitLabel = Utils.FindChild(transform, "ExtendLimit").GetComponent<UILabel>();
    }

    private void InstallHandlers()
    {
        okLis.onClick = OnOk;
        cancelLis.onClick = OnCancel;
        increaseLis.onClick = OnIncrease;
        decreaseLis.onClick = OnDecrease;
    }

    private void UnInstallHandlers()
    {
        okLis.onClick = null;
        cancelLis.onClick = null;
        increaseLis.onClick = null;
        decreaseLis.onClick = null;
    }

    private void OnEnable()
    {
        InstallHandlers();
    }

    private void OnDestory()
    {
        UnInstallHandlers();
    }

    private void OnOk(GameObject go)
    {
        if(OkClicked != null)
        {
            OkClicked(go);
        }
        Destroy(gameObject);
    }

    private void OnCancel(GameObject go)
    {
        if (CancelClicked != null)
        {
            CancelClicked(go);
        }
        Destroy(gameObject);
    }

    private void OnIncrease(GameObject go)
    {
        ExtendSize++;
        if (ExtendSize >= maxExtendSize)
        {
            increaseLis.GetComponent<UIButton>().isEnabled = false;
        }
        if (ExtendSize == 2)
        {
            decreaseLis.GetComponent<UIButton>().isEnabled = true;
        }
    }

    private void OnDecrease(GameObject go)
    {
        ExtendSize--;
        if (ExtendSize < maxExtendSize)
        {
            increaseLis.GetComponent<UIButton>().isEnabled = true;
        }
        if (ExtendSize == 1)
        {
            decreaseLis.GetComponent<UIButton>().isEnabled = false;
        }
    }
}

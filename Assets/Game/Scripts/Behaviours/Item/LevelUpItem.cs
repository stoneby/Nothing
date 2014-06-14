using KXSGCodec;

public class LevelUpItem : ItemBase 
{
    public void InitItem(ItemInfo itemInfo)
    {
        BagIndex = itemInfo.BagIndex;
        Quality = ItemModeLocator.Instance.GetQuality(itemInfo.TmplId);
    }
}

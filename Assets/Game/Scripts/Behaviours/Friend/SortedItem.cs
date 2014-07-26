using System.Collections.Generic;
using KXSGCodec;

public class SortedItem : FriendItem
{
    public List<string> ThreeWinnerSpriteNames; 
    private UILabel curPass;
    private UILabel highestHit;
    private UILabel loserLabel;
    private UISprite winerSprite;

    protected override void Awake()
    {
        base.Awake();
        curPass = transform.Find("CurPassTitle/CurPass").GetComponent<UILabel>();
        highestHit = transform.Find("MaxHitTitle/MaxHit").GetComponent<UILabel>();
        loserLabel = transform.Find("Ranking/Loser").GetComponent<UILabel>();
        winerSprite = transform.Find("Ranking/Winner").GetComponent<UISprite>();
    }

    public override void Init(FriendInfo info)
    {
        base.Init(info);
    }

    public void Init(FriendInfo info, int rank)
    {
        Init(info);
        if(rank < 3)
        {
            loserLabel.text = "";
            winerSprite.enabled = true;
            winerSprite.spriteName = ThreeWinnerSpriteNames[rank];
        }
        else
        {
            winerSprite.enabled = false;
            loserLabel.text = "" + rank;
        }
    }
}

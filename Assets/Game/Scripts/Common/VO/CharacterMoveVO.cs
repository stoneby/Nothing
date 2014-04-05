using UnityEngine;

public class CharacterMoveVO
{
    public string Name;
    public Vector3 From;
    public Vector3 To;
    public Vector3 DestPosition;
    public float Duration;
    public float StartTime;
    public int FrameCount;
    public int CurrentFrame = -1;

    private float deltTime;
    private int pastFrames;

    public float Init(string thename, Vector3 thefrom, Vector3 theto, int thecount = 4)
    {
        Name = thename;
        From = thefrom;
        DestPosition = theto;
        FrameCount = thecount;
        CurrentFrame = 0;
        StartTime = Time.time;
        pastFrames = -1;
        var xx = (From.x - theto.x) * (From.x - theto.x);
        var yy = (From.y - theto.y) * (From.y - theto.y);
        xx = Mathf.Sqrt(xx + yy);
        Duration = 0.1f * xx / 120;
        const float x0 = 300;
        var y0 = x0 * (theto.y - From.y) / (theto.x - From.x);
        To = new Vector3(theto.x + x0, theto.y + y0, theto.z);

        var attracktime = Duration * (theto.x - From.x) / (To.x - From.x);
        deltTime = attracktime / FrameCount;
        return Duration;
    }

    public string ScriptName()
    {
        pastFrames++;

        var tt = Time.time - StartTime;
        if (tt > Duration)
        {
            return "finish";
        }

        var k = Mathf.FloorToInt(tt / deltTime);

        if (k != CurrentFrame && k < FrameCount)
        {
            CurrentFrame = k;
        }
        var str = Name + "3_" + CurrentFrame.ToString();
        return str;
    }

    public Vector3 GetCurrentPosition()
    {
        var tt = Time.time - StartTime;
        return new Vector3(From.x + (To.x - From.x) * tt / Duration, From.y + (To.y - From.y) * tt / Duration, 0);
    }
}

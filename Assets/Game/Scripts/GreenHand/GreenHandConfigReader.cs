using UnityEngine;
using System.Collections;

public interface GreeenHandConfigReader
{
    string GetBattleType(int index);

    bool ReadConfig(GreenHandGuideHandler handler, int configIndex);
}

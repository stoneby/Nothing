using UnityEngine;
using System.Collections;

public sealed class PlayerModelLocator
{
    private static volatile PlayerModelLocator instance;
    private static readonly object SyncRoot = new Object();
    private PlayerModelLocator() { }
    public static PlayerModelLocator Instance
    {
        get
        {
            if (instance == null)
            {
                lock (SyncRoot)
                {
                    if (instance == null)
                        instance = new PlayerModelLocator();
                }
            }
            return instance;
        }
    }

    public short HeroId;
    public string Name;
    public short Level;
    public int HeadIconId;
    public int Exp;
    public long Diamond;
    public long Gold;
    public int Sprit;
    public int Energy;
//    public 
	public int HeroMax;
	public int ItemMax;
	public int ExtendHeroTimes;
	public int ExtendItemTimes;
}
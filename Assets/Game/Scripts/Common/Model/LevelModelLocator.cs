using Template;
using UnityEngine;

public sealed class LevelModelLocator
{
    private static volatile LevelModelLocator instance;
    private static readonly object SyncRoot = new Object();

    private LevelUp levelUpTemplates;

    private const string LevelUpTemlatePath = "Templates/LevelUp";

    private LevelModelLocator()
    {
    }

    public static LevelModelLocator Instance
    {
        get
        {
            if (instance == null)
            {
                lock (SyncRoot)
                {
                    if (instance == null)
                        instance = new LevelModelLocator();
                }
            }
            return instance;
        }
    }

    public LevelUp LevelUpTemplates
    {
        get { return levelUpTemplates ?? (levelUpTemplates = Utils.Decode<LevelUp>(LevelUpTemlatePath)); }
    }

    public LevelUpTemplate GetLevelByTemplateId(int templateid)
    {
        if (LevelUpTemplates != null && LevelUpTemplates.LevelUpTmpl != null && LevelUpTemplates.LevelUpTmpl.ContainsKey(templateid))
        {
            return LevelUpTemplates.LevelUpTmpl[templateid];
        }
        return null;
    }
}
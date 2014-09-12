using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EffectInfo = EffectSequnce.EffectInfo;

[CustomEditor(typeof(EffectSequnce))]
public class EffectSequnceEditor : Editor
{
    private EffectSequnce effectSequnce;
    private readonly string[] effectTypes = {"None", "Particle"};

    public override void OnInspectorGUI()
    {
        effectSequnce = target as EffectSequnce;
        if(effectSequnce == null)
        {
            return;
        }
        effectSequnce.RenderQ = EditorGUILayout.IntField("RenderQ", effectSequnce.RenderQ);
        if(GUILayout.Button("+"))
        {
            var effectInfo = new EffectInfo();
            effectSequnce.EffectInfos.Add(effectInfo);
        }
        var effectsToRemove = new List<EffectInfo>();
        for (int i = 0; i < effectSequnce.EffectInfos.Count; i++)
        {
            GUILayout.Label("Effect"+ (i+1));
            var effectInfo = effectSequnce.EffectInfos[i];
            GUILayout.BeginHorizontal();
            effectInfo.EffectObj =
                (GameObject) EditorGUILayout.ObjectField("EffectObj", effectInfo.EffectObj, typeof(GameObject), true);
            if(GUILayout.Button("-"))
            {
                effectsToRemove.Add(effectInfo);
            }
            GUILayout.EndHorizontal();
            var index = effectInfo.EffectType == EffectSequnce.EffectType.None ? 0 : 1;
            index = EditorGUILayout.Popup("EffectType", index, effectTypes);
            effectInfo.EffectType = index == 0 ? EffectSequnce.EffectType.None : EffectSequnce.EffectType.Particle;
            //GUILayout.BeginHorizontal();
            effectInfo.StartTime = EditorGUILayout.FloatField("StartTime", effectInfo.StartTime);
            effectInfo.Douration = EditorGUILayout.FloatField("Douration", effectInfo.Douration);
            //GUILayout.EndHorizontal();
            effectInfo.PositonFrom = EditorGUILayout.Vector3Field("PositionFrom", effectInfo.PositonFrom);
            effectInfo.PositionTo = EditorGUILayout.Vector3Field("PositionTo", effectInfo.PositionTo);
            effectInfo.IsLocal = GUILayout.Toggle(effectInfo.IsLocal, "UseLocal");
            EditorGUILayout.LabelField("-------------------------------------------------------------------------------------------");
            GUILayout.Space(5);
        }
        effectSequnce.EffectInfos.RemoveAll(effectsToRemove.Contains);
    }

}

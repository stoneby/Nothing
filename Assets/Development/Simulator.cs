using UnityEditor;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Simulator : MonoBehaviour
{
    public ParticleSystem Particle;
    public bool Open;

    private float time;

    private void CallbackFunction()
    {
        Logger.Log("updating...");

        if (Open)
        {
            EditorUtility.SetDirty(gameObject);
        }

        DoUpdate();
    }

    void OnEnable()
    {
        EditorApplication.update += CallbackFunction;
    }

    void OnDisable()
    {
        EditorApplication.update -= CallbackFunction;
    }

    [ContextMenu("Play")]
    public void Play()
    {
        Logger.Log("Play");
        //Particle.Play(true);
        Particle.Simulate(10f);
        Logger.Log(Particle.time);
        Logger.Log("" + Particle.duration);
    }

    public void OnGUI()
    {
        if (GUILayout.Button("test"))
        {
            Logger.Log("Hi");
        }
    }

    void DoUpdate()
    {
        Logger.Log("hi");
        if (time > Particle.duration)
        {
            time = 0f;
        }
        time += Time.fixedDeltaTime;
        Logger.Log("Time : " + time);
        Particle.Simulate(time + Time.deltaTime);
    }
}

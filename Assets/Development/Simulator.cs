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
        Debug.Log("updating...");

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
        Debug.Log("Play");
        //Particle.Play(true);
        Particle.Simulate(10f);
        Debug.Log(Particle.time);
        Debug.Log("" + Particle.duration);
    }

    public void OnGUI()
    {
        if (GUILayout.Button("test"))
        {
            Debug.Log("Hi");
        }
    }

    void DoUpdate()
    {
        Debug.Log("hi");
        if (time > Particle.duration)
        {
            time = 0f;
        }
        time += Time.fixedDeltaTime;
        Debug.Log("Time : " + time);
        Particle.Simulate(time + Time.deltaTime);
    }
}

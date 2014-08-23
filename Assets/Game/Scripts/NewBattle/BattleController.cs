using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BattleController : MonoBehaviour
{
    public TeamSelectController SelectController;
    public List<GameObject> EnemyList;

    public Transform AttackStackPoint;
    public int ColorIndex;

    public float RunTime = 0.2f;
    public float AttackTime = 0.3f;

    private void OnSelected(GameObject selectObject)
    {
        
    }

    private void OnStart()
    {
        
    }

    private void OnStop(bool attack)
    {
        Attack();
    }

    private void Attack()
    {
        StartCoroutine(DoAttack());
    }

    private IEnumerator DoAttack()
    {
        var enemy = EnemyList[0];
        for (var i = 0; i < SelectController.SelectedCharacterList.Count; ++i)
        {
            var item = SelectController.SelectedCharacterList[i].GetComponent<Character>();
            var originalPosition = item.transform.position;

            // set run state.
            item.PlayState(Character.State.Run, false);
            // run to enemy.
            iTween.MoveTo(item.gameObject, AttackStackPoint.position, RunTime);
            yield return new WaitForSeconds(RunTime);

            // set attack state.
            item.PlayState(Character.State.Attack, false);
            yield return new WaitForSeconds(AttackTime);

            // enemy hurt.
            var enemyControll = enemy.GetComponent<EnemyControl>();
            enemyControll.PlayAttack();
            enemyControll.PlayShake();

            // set run state.
            item.PlayState(Character.State.Run, true);
            // run back.
            iTween.MoveTo(item.gameObject, originalPosition, RunTime);
            yield return new WaitForSeconds(RunTime);

            // set idle state.
            item.PlayState(Character.State.Idle, true);
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Idle"))
        {
            PlayState(Character.State.Idle, true);
        }

        if (GUILayout.Button("Run"))
        {
            PlayState(Character.State.Run, true);
        }

        if (GUILayout.Button("Attack"))
        {
            PlayState(Character.State.Attack, false);
        }

        if (GUILayout.Button("Hurt"))
        {
            PlayState(Character.State.Hurt, false);
        }
    }

    private void PlayState(Character.State state, bool loop)
    {
        SelectController.CharacterList.ForEach(item =>
        {
            var characterControll = item.GetComponent<Character>();
            characterControll.PlayState(state, loop);
        });
    }

    void Start()
    {
        SelectController.OnSelect += OnSelected;
        SelectController.OnStart += OnStart;
        SelectController.OnStop += OnStop;

        SelectController.CharacterList.ForEach(item =>
        {
            item.ColorIndex = ColorIndex;
        });
    }
}

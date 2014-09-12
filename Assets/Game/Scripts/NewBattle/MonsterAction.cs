using UnityEngine;
using System.Collections;

public class MonsterAction : MonoBehaviour
{
    public TeamSelectController TeamController;

    public float AttackTime = 1f;

    public void Attack(GameObject attackObject)
    {
        Debug.Log("Attacker: " + attackObject.name);
        StartCoroutine(DoAttack(attackObject));

    }

    private IEnumerator DoAttack(GameObject attackObject)
    {
        // enemy attack.
        var enemyControll = attackObject.GetComponent<MonsterControl>();
        enemyControll.PlayAttack();
        enemyControll.PlayShake();

        yield return new WaitForSeconds(AttackTime);

        // character hurt.
        TeamController.CharacterList.ForEach(item =>
        {
            var characterControll = item.GetComponent<Character>();
            characterControll.PlayState(Character.State.Hurt, false);
        });
    }

    void Start()
    {
        Debug.Log("Screen height: " + Screen.height + ", width: " + Screen.width);
    }
}

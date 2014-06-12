using System.Collections;
using UnityEngine;

public class AttackSimulator3D : AttackSimulator
{
    public override void Attack(GameObject target)
    {
        StartCoroutine(DoAttack(target));
    }

    private IEnumerator DoAttack(GameObject target)
    {
        var selectList = TeamController.SelectedCharacterList;
        Logger.Log("Attack target: " + target.name);
        var targetAnimator = target.GetComponent<CharacterControl>().Animator;
        foreach (var selectCharacter in selectList)
        {
            Logger.Log("Figher: " + selectCharacter.name);
            var animator = selectCharacter.GetComponent<CharacterControl>().Animator;
            animator.SetTrigger("Run");

            yield return StartCoroutine(DoRun(animator, targetAnimator));
        }
    }

    private IEnumerator DoRun(Animator selectAnimator, Animator targetAnimator)
    {
        // remember the original position to get back.
        var originalPosition = selectAnimator.transform.position;

        // move to target and attack.
        iTween.MoveTo(selectAnimator.gameObject, targetAnimator.transform.position, RunTime);
        yield return new WaitForSeconds(RunTime / 3);
        selectAnimator.SetTrigger("Attack");
        targetAnimator.SetTrigger("Hurt");

        yield return new WaitForSeconds(0.5f);

        // move back to original position and idle.
        iTween.MoveTo(selectAnimator.gameObject, originalPosition, ReturnTime);
        yield return new WaitForSeconds(ReturnTime / 2);
        selectAnimator.SetTrigger("Idle");
    }

    #region Mono

    void Start()
    {

    }

    #endregion
}

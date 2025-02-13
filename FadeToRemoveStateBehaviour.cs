using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeToRemoveStateBehaviour : StateMachineBehaviour
{
    public float timeToRemove = 1f;

    GameObject objToRemove;
    SpriteRenderer spriteRenderer;
    Color fadeStartColor;
    float timeElapsed;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        objToRemove = animator.gameObject;
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        fadeStartColor = spriteRenderer.color;
        timeElapsed = 0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed += Time.deltaTime;

        // Fade towards and then remove
        if (timeElapsed > timeToRemove)
        {
            Destroy(objToRemove);
        } else
        {
            float portionFaded = (1 - timeElapsed / timeToRemove);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, fadeStartColor.a * portionFaded);
        }
        
    }
}

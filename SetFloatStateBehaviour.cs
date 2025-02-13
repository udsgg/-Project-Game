using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFloatStateBehaviour : StateMachineBehaviour
{
    public string floatName;
    public bool changeValueOnEnter = true, changeValueOnExit = true;
    public float valueOnStateEnter, valueOnStateExit;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (changeValueOnEnter)
            animator.SetFloat(floatName, valueOnStateEnter);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (changeValueOnExit)
            animator.SetFloat(floatName, valueOnStateExit);
    }
}

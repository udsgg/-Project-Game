using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadcastMessageStateBehaviour : StateMachineBehaviour
{
    public string methodName;
    public bool sendOnEnter, sendOnExit;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (sendOnEnter)
        {
            animator.gameObject.BroadcastMessage(methodName, SendMessageOptions.RequireReceiver);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (sendOnExit)
        {
            animator.gameObject.BroadcastMessage(methodName, SendMessageOptions.RequireReceiver);
        }
    }
}

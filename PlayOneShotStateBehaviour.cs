using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOneShotStateBehaviour : StateMachineBehaviour
{
    public AudioClip clip;
    public float volume = 1;
    public bool playOnEnter = true, playOnExit = false, playOnDelay = false;
    public float delayedPlayTime = 0.25f;

    private float timeSinceEntered = 0f;
    private bool hasPlayedDelayedSound = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playOnEnter)
            AudioSource.PlayClipAtPoint(clip, animator.gameObject.transform.position, volume);

        timeSinceEntered = 0f;
        hasPlayedDelayedSound = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playOnDelay && !hasPlayedDelayedSound)
        {
            if(timeSinceEntered >= delayedPlayTime)
            {
                AudioSource.PlayClipAtPoint(clip, animator.gameObject.transform.position, volume);
                hasPlayedDelayedSound = true;
            } else
            {
                timeSinceEntered += Time.deltaTime;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playOnExit)
            AudioSource.PlayClipAtPoint(clip, animator.gameObject.transform.position, volume);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseAction : StateMachineBehaviour
{
    [SerializeField]
    string boolName = "inAction";
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, false);
    }

}

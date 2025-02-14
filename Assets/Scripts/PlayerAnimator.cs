using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    private const string IS_WALK�NG = "IsWalking";
    [SerializeField] public Player player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
    }
    private void Update()
    {
        animator.SetBool(IS_WALK�NG, player.IsWalking());
    }
}

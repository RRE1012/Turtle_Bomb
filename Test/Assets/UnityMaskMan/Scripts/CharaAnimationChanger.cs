using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class CharaAnimationChanger : MonoBehaviour
{
	private Animator animator;
	private AnimatorStateInfo currentState;
	private AnimatorStateInfo prevState;

	public void Start()
	{
		animator = GetComponent<Animator>();
		currentState = animator.GetCurrentAnimatorStateInfo(0);
		prevState = currentState;
	}

	public void Update()
	{
		if (animator)
		{
			if (Input.GetKeyDown("right"))
			{
				animator.SetBool("Next", true);
			}
			if (Input.GetKeyDown("left"))
			{
				animator.SetBool("Prev", true);
			}
			if (animator.GetBool("Next"))
			{
				currentState = animator.GetCurrentAnimatorStateInfo(0);
				if (currentState.nameHash != prevState.nameHash)
				{
					animator.SetBool("Next", false);
					prevState = currentState;
				}
			}
			if (animator.GetBool("Prev"))
			{
				currentState = animator.GetCurrentAnimatorStateInfo(0);
				if (currentState.nameHash != prevState.nameHash)
				{
					animator.SetBool("Prev", false);
					prevState = currentState;
				}
			}
		}
	}
}

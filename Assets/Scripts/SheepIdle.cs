using System;
using UnityEngine;

public class SheepIdle : MonoBehaviour
{
	[Header("Animations")]
	Animator anim;
	//float time = 0.0f;
	int randInt;

	[Header("Particles")]
	public ParticleSystem eatEffect;

	void Start()
	{
		anim = GetComponent<Animator>();
		randInt = UnityEngine.Random.Range(1, 2);
	}

	void Update()
	{
		// Handle idle animation
		anim.SetBool("isIdle", true);
		/*if (time > 5)
		{
			if (randInt == 1)
			{
				//anim.CrossFade("isLooking", 0.2f);
				anim.SetTrigger("isLooking");
			}
			else if (randInt == 2)
			{
				//anim.CrossFade("isEating", 0.2f);
				anim.SetTrigger("isEating");
				eatEffect.Play();
			}
			anim.SetBool("isIdle", true);
			randInt = UnityEngine.Random.Range(1, 2);
			time = 0;
		}
		time += Time.deltaTime;*/
	}
}
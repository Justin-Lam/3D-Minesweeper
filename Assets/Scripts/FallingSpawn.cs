using System.Collections;
using UnityEngine;

public class FallingSpawn : MonoBehaviour
{
	public IEnumerator FallIntoPlace(float startingHeight, float startingScale, bool lerpRotation, float duration)
	{
		// got help from ChatGPT to do this: "how do i use this easing function to make my block fall from above to its starting place?"

		Vector3 startPosition = transform.position + new Vector3(0, startingHeight, 0);
		Vector3 targetPosition = transform.position;
		Quaternion startRotation = Random.rotation;
		Quaternion targetRotation = transform.rotation;
		Vector3 startScale = new Vector3(startingScale, startingScale, startingScale);
		Vector3 targetScale = transform.localScale;

		transform.position = startPosition;
		if (lerpRotation)
		{
			transform.rotation = startRotation;
		}
		transform.localScale = startScale;
		gameObject.GetComponent<BoxCollider>().enabled = false;
		gameObject.SetActive(true);

		float counter = 0;
		while (counter <= duration)
		{
			float counter_normalized = counter / duration;  // since easeOutBack requires start to be 0 and end to be 1
			transform.position = Vector3.LerpUnclamped(startPosition, targetPosition, easeOutBack(counter_normalized)); // use unclamped so t can go past 1
			if (lerpRotation)
			{
				transform.rotation = Quaternion.LerpUnclamped(startRotation, targetRotation, easeOutBack(counter_normalized/3));
			}
			transform.localScale = Vector3.LerpUnclamped(startScale, targetScale, easeOutBack(counter_normalized/3));	// divide by 3 so it happens slower than usual
			counter += Time.deltaTime;
			yield return null;
		}

		// Ensure game object goes exactly back to how it started
		transform.position = targetPosition;    
		transform.rotation = targetRotation;
		transform.localScale = targetScale;
		gameObject.GetComponent<BoxCollider>().enabled = true;

		enabled = false;	// done spawning, so disable script
	}
	float easeOutBack(float x)
	{
		// copied from https://easings.net/#easeOutBack
		const float c1 = 1.70158f;
		const float c3 = c1 + 1;

		return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
	}
}

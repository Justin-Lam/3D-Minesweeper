using System.Collections;
using UnityEngine;

public class FallingSpawn : MonoBehaviour
{
	public IEnumerator FallIntoPlace(float startingHeight, float duration)
	{
		// got help from ChatGPT to do this: "how do i use this easing function to make my block fall from above to its starting place?"

		Vector3 startPosition = transform.position + new Vector3(0, startingHeight, 0);
		Vector3 targetPosition = transform.position;

		transform.position = startPosition;
		gameObject.SetActive(true);

		float counter = 0;
		while (counter <= duration)
		{
			float counter_normalized = counter / duration;  // since easeOutBack requires start to be 0 and end to be 1
			transform.position = Vector3.LerpUnclamped(startPosition, targetPosition, easeOutBack(counter_normalized));	// use unclamped so t can go past 1
			counter += Time.deltaTime;
			yield return null;
		}

		transform.position = targetPosition;    // ensure game object goes exactly back to where it started
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

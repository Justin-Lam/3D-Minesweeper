using UnityEngine;

public class Explodable : MonoBehaviour
{
	[SerializeField] bool changeColliderOnExplode = false;
	[SerializeField] Vector3 newCenter;
	[SerializeField] Vector3 newSize;
	BoxCollider boxColllider;
	Rigidbody rb;

    void Start()
    {
        boxColllider = GetComponent<BoxCollider>();
		rb = GetComponent<Rigidbody>();
    }
	void OnEnable()
	{
		GameManager.OnLoseGame += OnLoseGame;
	}
	void OnDisable()
	{
		GameManager.OnLoseGame -= OnLoseGame;
	}
	void OnLoseGame()
	{
        if (changeColliderOnExplode)
        {
			boxColllider.center = newCenter;
			boxColllider.size = newSize;
		}
		rb.isKinematic = false;
	}
}

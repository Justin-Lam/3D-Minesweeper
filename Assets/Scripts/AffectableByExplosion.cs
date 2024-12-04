using UnityEngine;

public class AffectableByExplosion : MonoBehaviour
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
	public void OnAffectedByExplosion()
	{
        if (changeColliderOnExplode)
        {
			boxColllider.center = newCenter;
			boxColllider.size = newSize;
		}
		rb.isKinematic = false;
	}
}

using UnityEngine;

public class Block : MonoBehaviour
{
	public enum Type { GRASS, MINE };

	[Header("Type")]
	[SerializeField] Type type;
    int x;  // position in grid
    int y;  // position in grid

	[Header("Explosion")]
	[SerializeField] float radius = 5f;
	[SerializeField] float upwardsModifier = 3f;
	[SerializeField] float power = 500f;

	[Header("Materials")]
	[SerializeField] Material dirt;
	[SerializeField] Material mine;
    Renderer rr;

    void Start()
    {
        // Get renderer
        rr = GetComponent<Renderer>();
    }

    public void Initialize()
    {

    }

    public void OnEat()
    {
        if (type == Type.GRASS)
        {
            Debug.Log("You ate grass!");
            rr.material = dirt;
        }
        else
        {
            Debug.Log("BOOM");
            rr.material = mine;

            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddExplosionForce(power, transform.position, radius, upwardsModifier, ForceMode.Impulse);
                    rb.useGravity = true;

                    if (rb.gameObject.CompareTag("Player"))
                    {
						Vector3 explosionXZ = new Vector3(Random.Range(-1f, 1f) * power, 0, Random.Range(-1f, 1f) * power);
						rb.AddForce(explosionXZ, ForceMode.Impulse);
					}
                }
            }
        }
    }
}

using UnityEngine;

public class Block : MonoBehaviour
{
	public enum Type { GRASS, MINE };

	[Header("Data")]
	Type type = Type.GRASS;     public Type GetBlockType() {  return type; }
    int x;
    int y;

	[Header("Explosion")]
	[SerializeField] float radius = 5f;
	[SerializeField] float upwardsModifier = 3f;
	[SerializeField] float power = 500f;

	[Header("Materials")]
	[SerializeField] Material dirt;
	[SerializeField] Material mine;
    MeshRenderer mr;

    void Start()
    {
		// Get renderer
		mr = GetComponent<MeshRenderer>();
    }

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public void SetType(Type type)
    {
        this.type = type;
    }

	public void OnEat()
    {
        if (type == Type.GRASS)
        {
            Debug.Log("You ate grass!");
			mr.material = dirt;
        }
        else
        {
            Debug.Log("BOOM");
			mr.material = mine;

            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddExplosionForce(power, transform.position, radius, upwardsModifier, ForceMode.Impulse);

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

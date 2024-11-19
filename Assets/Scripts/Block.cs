using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
	public enum Type { GRASS, MINE };

	[Header("Data")]
	Type type = Type.GRASS;
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
    public bool IsGrass()
    {
        return type == Type.GRASS;
    }
    public void BecomeMine()
    {
        type = Type.MINE;
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

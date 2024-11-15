using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Material dirtColor;
    public Material mineColor;
    public enum Type {GRASS, MINE};
    public Type type;
    public GameObject player;
    public float radius = 5.0F;
    public float power = 500.0F;

    private Renderer blockRenderer;
    private Rigidbody playerRb;

    void Start()
    {
        blockRenderer = this.GetComponent<Renderer>();
        playerRb = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
    }

    public void OnEat()
    {
        if (type == Type.GRASS)
        {
            Debug.Log("You ate grass!");
            blockRenderer.material = dirtColor;
        }
        else if (type == Type.MINE)
        {
            Debug.Log("BOOM");
            blockRenderer.material = mineColor;

            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddExplosionForce(power, transform.position, radius, 3.0F, ForceMode.Impulse);
                    rb.useGravity = true;
                }
            }

            Vector3 explosionXZ = new Vector3(Random.Range(-1.0f, 1.0f) * power, 0, Random.Range(-1.0f, 1.0f) * power);
            playerRb.AddForce(explosionXZ, ForceMode.Impulse);
        }
    }
}

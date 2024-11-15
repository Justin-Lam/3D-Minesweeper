using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Material dirtColor;
    public Material mineColor;
    public enum Type {GRASS, MINE};
    public Type type;

    private Renderer blockRenderer;

    void Start()
    {
        blockRenderer = this.GetComponent<Renderer>();
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
        }
    }
}

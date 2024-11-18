using UnityEngine;

public class GameManager : MonoBehaviour
{
	// Singleton Pattern
	private static GameManager instance;
	public static GameManager Instance {  get { return instance; } }
	void Singleton_SetInstance()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}
	}

	void Awake()
	{
		Singleton_SetInstance();
	}
}

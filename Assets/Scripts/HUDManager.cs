using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
	[Header("HUD Elements")]
	[SerializeField] TMP_Text grassLeftText;
	[SerializeField] TMP_Text winLossText;
	[SerializeField] Button restartButton;
	[SerializeField] Button toMainMenuButton;

	[Header("Singleton Pattern")]
	private static HUDManager instance;
	public static HUDManager Instance { get { return instance; } }
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
	void OnEnable()
	{
		GameManager.OnWinGame += OnWinGame;
		GameManager.OnLoseGame += OnLoseGame;
	}
	void OnDisable()
	{
		GameManager.OnWinGame -= OnWinGame;
		GameManager.OnLoseGame -= OnLoseGame;
	}

	public void SetGrassLeftText(int grassLeft)
	{
		grassLeftText.text = "Grass left: " + grassLeft;
	}

	void OnWinGame()
	{
		// Show win text
		winLossText.gameObject.SetActive(true);
		winLossText.text = "You Win!";
		toMainMenuButton.gameObject.SetActive(true);
		restartButton.gameObject.SetActive(true);
		Cursor.lockState = CursorLockMode.None;
	}
	void OnLoseGame()
	{
		// Show lose text
		winLossText.gameObject.SetActive(true);
		winLossText.text = "You Lose...";
		toMainMenuButton.gameObject.SetActive(true);
		restartButton.gameObject.SetActive(true);
		Cursor.lockState = CursorLockMode.None;
	}

	public void ToMainMenu()
	{
		SceneManager.LoadScene("TitleScreen", LoadSceneMode.Single);
	}

	public void RestartLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
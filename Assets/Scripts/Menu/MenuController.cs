using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
	[SerializeField] private Button _playButton;
	[SerializeField] private Button _exitButton;
	
	void Start() {
		InitPlayButton(_playButton);
		InitExitButton(_exitButton);
	}

	private void InitPlayButton(Button button) {
		button.onClick.AddListener(delegate {
			StartGame();
		});
	}

	private void StartGame() {
		// TODO: Level selector
		SceneManager.LoadScene(1);
	}

	private void InitExitButton(Button button) {
		button.onClick.AddListener(delegate {
			ExitGame();
			Debug.Log("Exiting... bye");
		});
	}

	private void ExitGame() {
		Application.Quit();
	}
}

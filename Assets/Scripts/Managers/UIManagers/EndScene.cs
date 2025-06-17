using Core.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    public GameObject BackToWM;
    public GameObject BackToMM;
    private LoadSceneManager _loadSceneManager;

    public void Start() {
        _loadSceneManager = LoadSceneManager.Instance;   
    }

    public void BackToWorldMap() {
        _loadSceneManager.LoadWorldMapScene();
        // SceneManager.LoadScene("WorldMap");
    }

    public void BackToMainMenu() {
        _loadSceneManager.LoadMainMenuScene();
        // SceneManager.LoadScene("MainMenu");
    }
}

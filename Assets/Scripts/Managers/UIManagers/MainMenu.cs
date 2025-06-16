using Core.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject mainMenu;
    private LoadSceneManager _loadSceneManager;


    void Start() {
        _loadSceneManager = LoadSceneManager.Instance;
    }

    public void OpenOptionsPanel() {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OpenMainMenuPanel(){
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }


    public void QuitGame(){
        Application.Quit();
    }

    public void PlayGame(){
        if (_loadSceneManager == null) return;
        else _loadSceneManager.LoadWorldMapScene(false);
        // SceneManager.LoadScene("WorldMap");
    }
}

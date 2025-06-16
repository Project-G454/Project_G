using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    public GameObject BackToWM;
    public GameObject BackToMM;
    


    public void BackToWorldMap(){
        SceneManager.LoadScene("WorldMap");
    }

    public void BackToMainMenu(){
        SceneManager.LoadScene("MainMenu");
    }
}
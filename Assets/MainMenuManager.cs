using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    public void PlayGame(){
        SceneManager.LoadScene(1);
}
    public void Opensetting()
    {
        Debug.Log("Ayarlar menüüs açıldı");
    }
    public void QuitGame()
    {
        Debug.Log("Çıkış yapıldı");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

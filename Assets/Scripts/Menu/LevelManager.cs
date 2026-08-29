using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager:MonoBehaviour
{
    public static void SaveProgress(int levelIndex)
    {
        int savedlevel = PlayerPrefs.GetInt("SavedLevel", 1);

        if (levelIndex > savedlevel)
        {
            PlayerPrefs.SetInt("SavedLevel", levelIndex);
            PlayerPrefs.Save();
        }
    }
    public static void LoadSavedGame()
    {
        int targetLevel = PlayerPrefs.GetInt("SavedLevel", 1);
        SceneManager.LoadScene(targetLevel);
    }

    public static void ReserProgress()
    {
        PlayerPrefs.DeleteKey("SavedLevel");
        SceneManager.LoadScene(1);
    }
}

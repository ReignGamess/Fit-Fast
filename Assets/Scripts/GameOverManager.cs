using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Sahne Ayarları")]
    [SerializeField] private int mainMenuSceneIndex = 0; // Kendi ana menü sahnenin adını buraya yaz

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // TODO (Kişi 9): Burada oyun sonu sesi/efekti çalınacak
        Time.timeScale = 0f; // Oyunu duraklat, düşmanlar/saldırılar da dursun
    }

    public void OnReturnToMenuButtonPressed()
    {
        Time.timeScale = 1f; // Sahne değişmeden önce zamanı normale döndür
        SceneManager.LoadScene(mainMenuSceneIndex);
    }
}
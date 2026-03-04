using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance { get; private set; }

    [Header("Panel Game Over")]
    [SerializeField] private GameObject gameOverPanel; // Panel con los botones

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Panel oculto al inicio
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // Llamar este método cuando el jugador muere
    public void ShowGameOver()
    {
        StartCoroutine(ShowGameOverSequence());
    }

    private IEnumerator ShowGameOverSequence()
    {
        // Esperar un poco antes de mostrar el panel
        yield return new WaitForSeconds(0f);

        // Pausar el juego
        Time.timeScale = 0f;

        // Mostrar panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // Botón Reintentar
    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Botón Menú Principal
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Index 0 = Menu Principal
    }

    // Botón Salir
    public void ExitGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
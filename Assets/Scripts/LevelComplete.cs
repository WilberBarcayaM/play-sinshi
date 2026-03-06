using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelComplete : MonoBehaviour
{
    public static LevelComplete Instance { get; private set; }

    [Header("Panel Victoria")]
    [SerializeField] private GameObject levelCompletePanel;

    [Header("Contador")]
    [SerializeField] private TextMeshProUGUI countdownText; // Texto "Volviendo al menú en X..."
    [SerializeField] private float countdownTime = 5f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
    }

    // Llamar este método cuando el jugador gane
    public void ShowLevelComplete()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        // Pausar el juego
        Time.timeScale = 0f;

        StartCoroutine(CountdownToMenu());
    }

    private IEnumerator CountdownToMenu()
    {
        float timeLeft = countdownTime;

        while (timeLeft > 0)
        {
            if (countdownText != null)
                countdownText.text = $"Volviendo al menú en {Mathf.CeilToInt(timeLeft)}...";

            // Usar unscaledDeltaTime porque el juego está pausado
            yield return new WaitForSecondsRealtime(1f);
            timeLeft--;
        }

        GoToMainMenu();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Index 0 = Menu Principal
    }
}
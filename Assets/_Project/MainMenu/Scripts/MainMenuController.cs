using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private const string StorySceneName = "Story";
    private const string WorldMapSceneName = "WorldMap";

    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject aboutPanel;

    private void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (aboutPanel != null)
        {
            aboutPanel.SetActive(false);
        }
    }

    public void OpenSettings()
    {
        if (aboutPanel != null)
        {
            aboutPanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void OpenAbout()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (aboutPanel != null)
        {
            aboutPanel.SetActive(true);
        }
    }

    public void CloseAbout()
    {
        if (aboutPanel != null)
        {
            aboutPanel.SetActive(false);
        }
    }

    public void StartNewGame()
    {
        LoadSceneIfAvailable(StorySceneName);
    }

    public void ContinueGame()
    {
        LoadSceneIfAvailable(WorldMapSceneName);
    }

    private void LoadSceneIfAvailable(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning(
                $"A cena '{sceneName}' ainda não foi adicionada ao Build Profile."
            );
        }
    }
}

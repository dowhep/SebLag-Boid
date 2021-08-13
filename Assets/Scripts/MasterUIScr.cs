using UnityEngine;

public class MasterUIScr : MonoBehaviour
{

    #region Public Fields
    public GameObject PauseMenu;
    public GameObject EditMenu;
    public GameObject StatsMenu;
    #endregion

    #region Unity Methods
    void Start()
    {
        CamControlScr.Instance.PauseEvent += TogglePauseScreen;
        CamControlScr.Instance.EditEvent += ToggleEditScreen;
        CamControlScr.Instance.StatsEvent += ToggleStatsScreen;
    }
    #endregion
 
    public void QuitApplication()
    {
        Application.Quit();
    }

    #region Private Methods
    private void TogglePauseScreen(bool ispause)
    {
        PauseMenu.SetActive(ispause);
    }
    private void ToggleEditScreen(bool isedit)
    {
        EditMenu.SetActive(isedit);
    }
    private void ToggleStatsScreen(bool displaystats)
    {
        StatsMenu.SetActive(displaystats);
    }
    private void OnDestroy()
    {
        CamControlScr.Instance.PauseEvent -= TogglePauseScreen;
        CamControlScr.Instance.EditEvent -= ToggleEditScreen;
        CamControlScr.Instance.StatsEvent -= ToggleStatsScreen;
    }
    #endregion
}

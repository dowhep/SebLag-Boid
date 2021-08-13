using TMPro;
using UnityEngine;

public class MainUIScr : MonoBehaviour
{

    #region Public Fields
    public TextMeshProUGUI statsText;
    #endregion
 
    private Transform playerTrans;
    private float lastDeltaTime;
    private float elapseTime;

    #region Unity Methods

    private void Start()
    {
        playerTrans = CamControlScr.Instance.playerTrans;
    }

    private void Update()
    {
        if (CamControlScr.Instance.isPaused) return;
        
        elapseTime += Time.unscaledDeltaTime;
        if (elapseTime > 0.5f)
        {
            elapseTime = 0f;
            lastDeltaTime = Time.unscaledDeltaTime;
        }
    }
    void FixedUpdate()
    {
        statsText.text = "FPS: " + (1f / lastDeltaTime).ToString("F2") + " (" + lastDeltaTime.ToString("F2") + "ms)\n" +
            "Coordinates: (" + playerTrans.position.x.ToString("F2") + ", " + playerTrans.position.y.ToString("F2") + ", " + playerTrans.position.z.ToString("F2") + ")";
    }
 
    #endregion
 
}

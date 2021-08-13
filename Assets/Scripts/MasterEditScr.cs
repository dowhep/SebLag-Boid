using UnityEngine;

public class MasterEditScr : MonoBehaviour
{

    #region Public Fields
    public SliderScr BoidsAmtSl;
    public SliderScr SepSl;
    public SliderScr AliSl;
    public SliderScr CohSl;
    public SliderScr CenSl;
    public SliderScr TimeSl;
    public SliderScr SpeedSl;
    public SliderScr ScaleSl;
    public SliderScr ChaosSl;
    #endregion
 
    #region Unity Methods
 
    void Start()
    {
        BoidsAmtSl.OnRealValueChanged += BoidAmtChange;
        SepSl.OnRealValueChanged += SepChange;
        AliSl.OnRealValueChanged += AliChange;
        CohSl.OnRealValueChanged += CohChange;
        CenSl.OnRealValueChanged += CenChange;
        TimeSl.OnRealValueChanged += TimeChange;
        SpeedSl.OnRealValueChanged += SpeedChange;
        ScaleSl.OnRealValueChanged += ScaleChange;
        ChaosSl.OnRealValueChanged += ChaosChange;
    }
    public void cancelEdit()
    {
        CamControlScr.Instance.isEditing = false;
    }
    #endregion
 
    #region Private Methods
    private void BoidAmtChange(float value)
    {
        MasterScript.Instance.boidsToSpawn = (int)value;
    }
    private void SepChange(float value)
    {
        MasterScript.Instance.separationBias = value;
    }
    private void AliChange(float value)
    {
        MasterScript.Instance.alignmentBias = value;
    }
    private void CohChange(float value)
    {
        MasterScript.Instance.cohesionBias = value;
    }
    private void CenChange(float value)
    {
        MasterScript.Instance.targetBias = value;
    }
    private void TimeChange(float value)
    {
        Time.timeScale = value;
    }
    private void SpeedChange(float value)
    {
        MasterScript.Instance.maxSpeed = value;
    }
    private void ScaleChange(float value)
    {
        MasterScript.Instance.changeBoidsSize(value);
    }
    private void ChaosChange(float value)
    {
        MasterScript.Instance.boidSep = value;
    }

    #endregion
}

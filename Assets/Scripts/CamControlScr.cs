using System;
using UnityEngine;

public class CamControlScr : MonoBehaviour
{
    public static CamControlScr Instance;

    #region Public Fields
    public float mouseSensitivity = 100f;
    public float moveSpeed = 10f;
    public float smoothTime = 1f;
    public float defaultFOV = 60f;
    public float zoomInFOV = 40f;

    public event Action<bool> PauseEvent;
    public event Action<bool> EditEvent;
    public event Action<bool> StatsEvent;
    public Transform playerTrans;
    #endregion
    public bool isPaused { get { return _isPaused; } set { TogglePause(value); } }
    public bool isEditing { get { return _isEditing; } set { ToggleEditing(value); } }
    public bool displayStats { get { return _isDisplayingStats; } set { ToggleStats(value); } }

    private bool isPlayable { get { return !isPaused && !isEditing; } }
    private bool _isPaused = false;
    private bool _isEditing = false;
    private bool _isDisplayingStats = true;
    private float localXRot;
    private Vector3 refVel;
    private Camera cam;

    #region Unity Methods

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        displayStats = true;
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        CheckUICalls();

        Cursor.lockState = isPlayable ? CursorLockMode.Locked : CursorLockMode.None;
        if (isPlayable)
        {
            CheckPlayerControls();

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.unscaledDeltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.unscaledDeltaTime;

            localXRot = Mathf.Clamp(localXRot - mouseY, -90f, 90f);

            transform.localRotation = Quaternion.Euler(localXRot, 0f, 0f);
            playerTrans.Rotate(Vector3.up * mouseX);
        }
    }


    #endregion

    #region Private Methods
    private void CheckUICalls()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
        }
        if ((isPlayable && Input.GetMouseButtonDown(0)) || Input.GetKeyDown(KeyCode.E))
        {
            isEditing = !isEditing;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            displayStats = !displayStats;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Screenshots");
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/Screenshots/" + DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss") + ".png", 4);
        }
    }
    private void CheckPlayerControls()
    {
        cam.fieldOfView = Input.GetKey(KeyCode.C) ? zoomInFOV : defaultFOV;

        float hMove = Input.GetAxisRaw("Horizontal");
        float vMove = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = (hMove == 0f && vMove == 0f) ? Vector3.zero : Vector3.Normalize(transform.forward * vMove + transform.right * hMove) * moveSpeed * Time.unscaledDeltaTime;

        playerTrans.position = Vector3.SmoothDamp(playerTrans.position, playerTrans.position + moveDir, ref refVel, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);

        playerTrans.position += transform.forward * Input.mouseScrollDelta.y;

    }

    private void TogglePause(bool newPause)
    {
        if (newPause != _isPaused)
        {
            _isPaused = !_isPaused;
            PauseEvent?.Invoke(_isPaused);
        }
    }
    private void ToggleEditing(bool newEdit)
    {
        if (newEdit != _isEditing)
        {
            _isEditing = !_isEditing;
            EditEvent?.Invoke(_isEditing);
        }
    }
    private void ToggleStats(bool newDisplay)
    {
        if (newDisplay != _isDisplayingStats)
        {
            _isDisplayingStats = !_isDisplayingStats;
            StatsEvent?.Invoke(_isDisplayingStats);
        }
    }
    #endregion
}

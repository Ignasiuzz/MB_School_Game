using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class JoystickPanelManager : MonoBehaviour
{
    [Header("Joystick")]
    public GameObject joystick;

    [Header("UI Panels")] // Panels that can be clicked out of
    public GameObject[] panels;

    [Header("Buttons")]
    public GameObject menuButton;

    [Header("Interaction Detector")]
    public InteractionDetector interactionDetector;

    [Header("Close Settings")]
    public float clickBlockDuration = 0.15f; // prevents instant close after opening

    private float lastPanelOpenTime = -1f;

    void Update()
    {
        if (joystick == null || panels == null || panels.Length == 0) return;

        bool anyPanelActive = false;

        foreach (var panel in panels)
        {
            if (panel != null && panel.activeSelf)
            {
                anyPanelActive = true;
                break;
            }
        }

        // Toggle joystick & menu button
        joystick.SetActive(!anyPanelActive);

        if (menuButton != null)
            menuButton.SetActive(!anyPanelActive);

        if (interactionDetector != null)
            interactionDetector.panelsOpen = anyPanelActive;

        // Handle click outside to close
        if (anyPanelActive && IsPointerDown())
        {
            // Prevent instant close right after opening
            if (Time.time - lastPanelOpenTime < clickBlockDuration)
                return;

            for (int i = panels.Length - 1; i >= 0; i--)
            {
                var panel = panels[i];

                if (panel != null && panel.activeSelf)
                {
                    if (ClickedOutsidePanel(panel))
                    {
                        panel.SetActive(false);
                        Time.timeScale = 1f;
                    }

                    break;
                }
            }
        }
    }

    private bool IsPointerDown()
    {
        if (Input.GetMouseButtonDown(0))
            return true;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            return true;

        return false;
    }

    private bool ClickedOutsidePanel(GameObject panel)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.transform.IsChildOf(panel.transform))
                return false;
        }

        return true;
    }
}
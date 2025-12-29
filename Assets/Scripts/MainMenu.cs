using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Animator screenCover;
    [SerializeField] GameObject btnGroup, mainGroup, settingsGroup, creditsGroup;
    [SerializeField] GameObject pcControls, controllerControls;
    public UnityEngine.InputSystem.PlayerInput playerInput;
    bool usingController = false;

    [SerializeField] RectTransform settingsBtn, exitBtn;

    void Start()
    {
        OpenMain();

        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            // Executable
            settingsBtn.anchoredPosition3D = new Vector3(0,-77,0);
            exitBtn.anchoredPosition3D = new Vector3(0,-231,0);
        } else
            {
                // WebGL
                exitBtn.anchoredPosition3D = new Vector3(0,-77,0);
                settingsBtn.gameObject.SetActive(false);
                exitBtn.anchoredPosition3D = settingsBtn.anchoredPosition3D;
                
                menuBtns.RemoveAt(1); // remove settings from list
            }

        Application.targetFrameRate = 75;
        btnGroup.SetActive(true);
    }

    [SerializeField] List<Button> menuBtns = new List<Button>();
    int mainIndex = 0;
    void Update()
    {
        if (usingController)
        {
            if (mainGroup.activeInHierarchy)
            {
                // new unity input system usage
                var gamepad = Gamepad.current;
                if (gamepad != null)
                {
                    if (gamepad.dpad.up.wasPressedThisFrame)
                    {
                        mainIndex = (mainIndex - 1 + menuBtns.Count) % menuBtns.Count;
                    } else if (gamepad.dpad.down.wasPressedThisFrame)
                        {
                            mainIndex = ++mainIndex % menuBtns.Count;
                        }
                }

                // select a button
                EventSystem.current.SetSelectedGameObject(menuBtns[mainIndex].gameObject);

                if (PlayerInput.PressedJump())
                {
                    // interact with button
                    menuBtns[mainIndex].onClick.Invoke();
                }
            } else
                {
                    if (!mainGroup.activeInHierarchy && PlayerInput.PressedBack()) OpenMain();
                }
        }
    }

    void OnControlsChanged(UnityEngine.InputSystem.PlayerInput input)
    {
        usingController = input.currentControlScheme == "Gamepad";
        pcControls.SetActive(!usingController);
        controllerControls.SetActive(usingController);
    }

    void LoadLevel() { SceneManager.LoadScene(1); }

    public void PlayGame()
    {
        btnGroup.SetActive(false);
        screenCover.Play("toBlack");
        AudioManager.Instance.Transitioning();

        // give illusion of loading
        Invoke("LoadLevel", 3);
    }

    public void OpenMain()
    {
        mainGroup.SetActive(true);
        settingsGroup.SetActive(false);
        creditsGroup.SetActive(false);
    }

    public void OpenSettings()
    {
        mainGroup.SetActive(false);
        settingsGroup.SetActive(true);
        creditsGroup.SetActive(false);
    }

    public void OpenCredits()
    {
        mainGroup.SetActive(false);
        settingsGroup.SetActive(false);
        creditsGroup.SetActive(true);
    }

    public void ExitGame() { Application.Quit(); }
}//EndScript
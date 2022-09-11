/*
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  [SerializeField] private GameObject pauseMenuCanvas;
  [SerializeField] private Button quitButton;

  private static GameObject pauseMenu;
  private static Button _quitButton;
  private static bool isPaused;
  private static bool isInputInitialized;
  private static GameObject lastSelectedGameObject;

  private void Awake()
  {
    pauseMenu = pauseMenuCanvas;
    _quitButton = quitButton;

    inputActionMap = inputActionAsset.FindActionMap("UI");

    _quitButton.onClick.AddListener(Quit);

    DOTween.Init().SetCapacity(1250, 50);
  }

  private void OnApplicationFocus(bool hasFocus)
  {
    if (!hasFocus && !isPaused)
    {
      OnPause();
    }
  }

  private void Update()
  {
    if (!isInputInitialized)
    {
      ResetInput();
      isInputInitialized = true;
    }

    if (Keyboard.current.escapeKey.wasPressedThisFrame)
    {
      OnPause();
    }

    var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
    if (currentSelectedGameObject != lastSelectedGameObject)
    {
      AkSoundEngine.PostEvent("Play_UIMove", gameObject);
      lastSelectedGameObject = currentSelectedGameObject;
    }
  }

  private static void ResetInput()
  {
    var inputModule = (InputSystemUIInputModule) EventSystem.current.currentInputModule;
    inputModule.point = null;
    inputModule.leftClick = null;
    inputModule.move = InputActionReference.Create(inputActionMap.FindAction("Navigate"));
    inputModule.submit = InputActionReference.Create(inputActionMap.FindAction("Submit"));
  }

  private static void OnPause()
  {
    if (isPaused)
    {
      ResetInput();
      AudioEvents.ResumeMusic();
      Time.timeScale = 1;
      pauseMenu.SetActive(false);
      isPaused = false;
    }
    else
    {
      var inputModule = (InputSystemUIInputModule) EventSystem.current.currentInputModule;
      inputModule.point = InputActionReference.Create(inputActionMap.FindAction("Point"));
      inputModule.leftClick = InputActionReference.Create(inputActionMap.FindAction("Click"));
      inputModule.move = null;
      inputModule.submit = null;
      AudioEvents.PauseMusic();
      Time.timeScale = 0;
      pauseMenu.SetActive(true);
      isPaused = true;
    }
  }


  private static void Quit()
  {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
  }
}
*/

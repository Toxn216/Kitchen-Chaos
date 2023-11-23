using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{

    public static OptionsUI Instance {  get; private set; }

    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAltrenateButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gamepadInteractButton;
    [SerializeField] private Button gamepadInteractAltrenateButton;
    [SerializeField] private Button gamepadPauseButton;
    [SerializeField] private TextMeshProUGUI soundEffectsText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAltrenateText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI gamepadInteractText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAltrenateText;
    [SerializeField] private TextMeshProUGUI gamepadPauseText;
    [SerializeField] private Transform pressToRebindKeyTransform;

    private Action onCloseButtonAction;

    private void Awake()
    {

        Instance = this;

        soundEffectsButton.onClick.AddListener(() => 
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        musicButton.onClick.AddListener(() => 
        {
            MusicManager.Instanse.ChangeVolume();
            UpdateVisual();
        });
        closeButton.onClick.AddListener(() => 
        {
            Hide();
            onCloseButtonAction();
        });

        moveUpButton.onClick.AddListener(() => { RebindBindng(GameInput.Binding.Move_Up); });
        moveDownButton.onClick.AddListener(() => { RebindBindng(GameInput.Binding.Move_Down); });
        moveLeftButton.onClick.AddListener(() => { RebindBindng(GameInput.Binding.Move_Left); });
        moveRightButton.onClick.AddListener(() => { RebindBindng(GameInput.Binding.Move_Right); });
        interactButton.onClick.AddListener(() => { RebindBindng(GameInput.Binding.Interact); });
        interactAltrenateButton.onClick.AddListener(() => { RebindBindng(GameInput.Binding.InteractAlternate); });
        pauseButton.onClick.AddListener(() => { RebindBindng(GameInput.Binding.Pause); });
        gamepadInteractButton.onClick.AddListener(() => { RebindBindng(GameInput.Binding.Gamepad_Interact); });
        gamepadInteractAltrenateButton.onClick.AddListener(() => { RebindBindng(GameInput.Binding.Gamepad_InteractAlternate); });
        gamepadPauseButton.onClick.AddListener(() => { RebindBindng(GameInput.Binding.Gamepad_Pause); });
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnGameUnpaused += KitchenGameManager_OnGameUnpaused;
        UpdateVisual();

        HidePressToRebindKey();
        Hide();
    }

    private void KitchenGameManager_OnGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()//��������� ������ ���� ����������� ���������
    {
        soundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instanse.GetVolume() * 10f);

        moveUpText.text = GameInput.Instanse.GetBindingText(GameInput.Binding.Move_Up);
        moveDownText.text = GameInput.Instanse.GetBindingText(GameInput.Binding.Move_Down);
        moveLeftText.text = GameInput.Instanse.GetBindingText(GameInput.Binding.Move_Left);
        moveRightText.text = GameInput.Instanse.GetBindingText(GameInput.Binding.Move_Right);
        interactText.text = GameInput.Instanse.GetBindingText(GameInput.Binding.Interact);
        interactAltrenateText.text = GameInput.Instanse.GetBindingText(GameInput.Binding.InteractAlternate);
        pauseText.text = GameInput.Instanse.GetBindingText(GameInput.Binding.Pause);
        gamepadInteractText.text = GameInput.Instanse.GetBindingText(GameInput.Binding.Gamepad_Interact);
        gamepadInteractAltrenateText.text = GameInput.Instanse.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate);
        gamepadPauseText.text = GameInput.Instanse.GetBindingText(GameInput.Binding.Gamepad_Pause);
    }

    public void Show(Action onCloseButtonAction)
    {
        this.onCloseButtonAction = onCloseButtonAction;

        gameObject.SetActive(true);

        soundEffectsButton.Select();//������ �������� �������� � ���� 
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    private void ShowPressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }
    private void HidePressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }
    private void RebindBindng(GameInput.Binding binding)
    {
        ShowPressToRebindKey();
        GameInput.Instanse.RebindBinding(binding,() => 
        {
            HidePressToRebindKey();
            UpdateVisual();
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instanse { get; private set; }

    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    public event EventHandler OnInteractAction;//подрубаем взять предмет
    public event EventHandler OnInteractAlternateAction;//подрубаем интеракт (нарезать предмет)
    public event EventHandler OnPauseAction;
    public event EventHandler OnBindingRebind;

    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        InteractAlternate,
        Pause,
        Gamepad_Interact,
        Gamepad_InteractAlternate,
        Gamepad_Pause
    }

    private PlayerInputActions playerInputActions;//порубаем инпут менеджер который создали

    private void Awake()
    {
        Instanse = this;

        playerInputActions = new PlayerInputActions();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }

        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += Interact_performed;//Подрубаем кнопку взять предмет\положить предмет
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;//подрубаем нарезать предмет
        playerInputActions.Player.Pause.performed += Pause_performed;       
    }

    private void OnDestroy()//уничтожаем инпуті после перехода в меню и обратно ибо будет ошибка
    {
        playerInputActions.Player.Interact.performed -= Interact_performed;//Подрубаем кнопку взять предмет\положить предмет
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;//подрубаем нарезать предмет
        playerInputActions.Player.Pause.performed -= Pause_performed;

        playerInputActions.Dispose();//уничтожаем инпут єкшн что бі не кушал память
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)//событие для роботы кнопки нарезания
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)//отвалился перевод крч подрубается вместе с кнопкой без єтого рабоатать не будет
    {
        //if(onInteractAction != null)
        //{
        //    onInteractAction(this, EventArgs.Empty);вот єто єто тоже самое что
        //}
        OnInteractAction?.Invoke(this, EventArgs.Empty);//вот это
    }

    public Vector2 GetMovmentVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();// после равно мы даем ссылку на новый инпут менеджер
                                                                                  // тоесть название, акшн мапс(плеер) и экшнс (мув), тот что новый создали мы крч инпут
   
        inputVector = inputVector.normalized;//нормализируем что бы когда идем по горизонтали то движение было не быстрее обычного

        return inputVector;
    }

    public string GetBindingText(Binding binding)
    {
        switch(binding)
        {
            default:
            case Binding.Move_Up:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
               return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
            case Binding.Gamepad_Interact:
                return playerInputActions.Player.Interact.bindings[1].ToDisplayString();
            case Binding.Gamepad_InteractAlternate:
                return playerInputActions.Player.InteractAlternate.bindings[1].ToDisplayString();
            case Binding.Gamepad_Pause:
                return playerInputActions.Player.Pause.bindings[1].ToDisplayString();
        }
    }
    public void RebindBinding(Binding binding,Action onActionRebind)
    {
        playerInputActions.Player.Disable();
        InputAction inputAction;
        int bindingIndex;

        switch (binding)
        {
            default:
            case Binding.Move_Up:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = playerInputActions.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
            case Binding.Gamepad_Interact:
                inputAction = playerInputActions.Player.Interact;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_InteractAlternate:
                inputAction = playerInputActions.Player.InteractAlternate;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_Pause:
                inputAction = playerInputActions.Player.Pause;
                bindingIndex = 1;
                break;
        }
        
        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback =>
        {
            callback.Dispose();
            playerInputActions.Player.Enable();
            onActionRebind();

            //Debug.Log(playerInputActions.SaveBindingOverridesAsJson());
            PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();

            OnBindingRebind?.Invoke(this, EventArgs.Empty);
        })
        .Start();

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";//неоспаримая константа (буливое значение в анимации ходьбы)

    [SerializeField] private Player player;// подрубаем скрипт плеера

    private Animator animator;// подрубаем анимацию
    private void Awake()
    {
        animator = GetComponent<Animator>(); // берем анимацию  
    }
    private void Update()
    {
        animator.SetBool(IS_WALKING, player.IsWalking());// активируем анимацию через метод ис валкинг в скрипте плеера (там написано как)
    }
}

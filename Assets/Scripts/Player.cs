using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour,IKitchenObjectParent
{

    public static Player Instance { get; private set; }//Это определение статического свойства Instance в классе Player

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangeEventArgs> OnSelectedCounterChanged;//Это определение события OnSelectedCounterChanged. Это событие типа EventHandler, параметризированное пользовательским классом OnSelectedCounterChangeEventArgs. Событие объявлено с модификатором public, что позволяет другим частям кода подписываться на это событие.
    public class OnSelectedCounterChangeEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
        //то определение пользовательского класса OnSelectedCounterChangeEventArgs, который наследуется от
        //EventArgs.Обычно этот класс используется для передачи дополнительной информации о событии.
        //В данном случае, он содержит свойство selectedCounter типа ClearCounter,
        //представляющее измененный счетчик.
    }

    [SerializeField] private float moveSpeed;//скорость движения
    [SerializeField] private GameInput gameInput;//подрубаем гейм инпут
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform KitchenObjectHoldPoint;//точка где будут спаунится префабы китчена

    private bool isWalking;//бул ходьбы
    private Vector3 lastInteractDir;//проверка на то что мы до сих пор смотрим на обьект
    private BaseCounter selectedCounter;//скрипт клеар каунтер это селект каунтер
    private KitchenObject kitchenObject;


    private void Awake()
    {
        if(Instance != null)//если инстанс не ноль то ошибка иб игрок уже создан, если все ок то идем дальше( простая проверка)
        {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
        //это может использоваться для создания паттерна синглтона, где класс имеет только один экземпляр,
        //и этот экземпляр может быть получен из других частей кода через статическую переменную Instance.
    }
    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;//подрубаем интерактивное действие
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)//Если обьект с каунтером не ноль то выполняем интеракт
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)//код обработки интерактивного действия игрока
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)//Если обьект с каунтером не ноль то выполняем интеракт
        {
            selectedCounter.Interact(this);
        }      
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking()// метод анимации ходьбы
    {
        return isWalking;//возвращаем ис волкинг
    }
    private void HandleInteractions()//Рейкат который может интерактировать с обьктами
    {
        Vector2 inputVector = gameInput.GetMovmentVectorNormalized();//инпут вектору мы задаем координаты метода из гейминпут

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);//переводим в вектор 3

        if(moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }
        float interactDistance = 2f;
        if(Physics.Raycast(transform.position,lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if(raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))//получаем компонент каунтера вместо того что бы писать гет компонетнт<клеар каунтер>
            {
                //чистим каунтеры
                if(baseCounter != selectedCounter)//проверяем не равен ли клиар каунтер селект каунтеру, если не то идем дальше
                {
                    SetSelectedCounter(baseCounter);//присваиваем селект каунтеру слеар каунтер                
                }
            }
            else
            {
                SetSelectedCounter(null);//если условие не прошло то селект каунтер ничего            
            }
        }
        else
        {
            SetSelectedCounter(null);//если условие не прошло то селект каунтер ничего         
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovmentVectorNormalized();//инпут вектору мы задаем координаты метода из гейминпут

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);//Переводим в вектор 3 ибо по другому ошибка будет

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;//радиус нашего плеера
        float playerHeight = 2f;//высота нашего плеера
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);//касулкакаст(рейкаст) кидает луч в форме капсулы по заданым параметрам для поиска других обьектов которые находятся на полу

        if (!canMove)
        {
            //Нельзя двигатся к moveDir

            //попробуем двигатся только по Х
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                //Можем двигатся только по Х
                moveDir = moveDirX;
            }
            else
            {
                //Не можем двигатьс только по х
                //попробуем двигаться по Z
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                if (canMove)
                {
                    //можем двигатся только по Z
                    moveDir = moveDirZ;
                }
                else
                {
                    //Не можем двигаться ни в какую сторону
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;//скорость перемешения + вектор + дельта тайм для того что бы не зависить от фпс игры а двигатся при помощи мув спид(Дельта переехала наверх
        }

        isWalking = moveDir != Vector3.zero;//как я понял если вектор не 0 то возвращаем тру
        float rotateSpeed = 10f;//скорость поворота
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);//легко задаем направдение движения вместо дурноватых корутинов

    }
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangeEventArgs//Знак ? преотвращает вызов инвок если онСелектКаунтер равен нул
        {
            selectedCounter = selectedCounter//вызывается событие селкт каунтер если оно было инициализировано(не равно нул)
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return KitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if(kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}

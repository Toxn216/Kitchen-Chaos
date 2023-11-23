using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour,IKitchenObjectParent
{

    public static Player Instance { get; private set; }//��� ����������� ������������ �������� Instance � ������ Player

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangeEventArgs> OnSelectedCounterChanged;//��� ����������� ������� OnSelectedCounterChanged. ��� ������� ���� EventHandler, ������������������� ���������������� ������� OnSelectedCounterChangeEventArgs. ������� ��������� � ������������� public, ��� ��������� ������ ������ ���� ������������� �� ��� �������.
    public class OnSelectedCounterChangeEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
        //�� ����������� ����������������� ������ OnSelectedCounterChangeEventArgs, ������� ����������� ��
        //EventArgs.������ ���� ����� ������������ ��� �������� �������������� ���������� � �������.
        //� ������ ������, �� �������� �������� selectedCounter ���� ClearCounter,
        //�������������� ���������� �������.
    }

    [SerializeField] private float moveSpeed;//�������� ��������
    [SerializeField] private GameInput gameInput;//��������� ���� �����
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform KitchenObjectHoldPoint;//����� ��� ����� ��������� ������� �������

    private bool isWalking;//��� ������
    private Vector3 lastInteractDir;//�������� �� �� ��� �� �� ��� ��� ������� �� ������
    private BaseCounter selectedCounter;//������ ����� ������� ��� ������ �������
    private KitchenObject kitchenObject;


    private void Awake()
    {
        if(Instance != null)//���� ������� �� ���� �� ������ �� ����� ��� ������, ���� ��� �� �� ���� ������( ������� ��������)
        {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
        //��� ����� �������������� ��� �������� �������� ���������, ��� ����� ����� ������ ���� ���������,
        //� ���� ��������� ����� ���� ������� �� ������ ������ ���� ����� ����������� ���������� Instance.
    }
    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;//��������� ������������� ��������
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)//���� ������ � ��������� �� ���� �� ��������� ��������
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)//��� ��������� �������������� �������� ������
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)//���� ������ � ��������� �� ���� �� ��������� ��������
        {
            selectedCounter.Interact(this);
        }      
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking()// ����� �������� ������
    {
        return isWalking;//���������� �� �������
    }
    private void HandleInteractions()//������ ������� ����� ��������������� � ��������
    {
        Vector2 inputVector = gameInput.GetMovmentVectorNormalized();//����� ������� �� ������ ���������� ������ �� ���������

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);//��������� � ������ 3

        if(moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }
        float interactDistance = 2f;
        if(Physics.Raycast(transform.position,lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if(raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))//�������� ��������� �������� ������ ���� ��� �� ������ ��� ����������<����� �������>
            {
                //������ ��������
                if(baseCounter != selectedCounter)//��������� �� ����� �� ����� ������� ������ ��������, ���� �� �� ���� ������
                {
                    SetSelectedCounter(baseCounter);//����������� ������ �������� ����� �������                
                }
            }
            else
            {
                SetSelectedCounter(null);//���� ������� �� ������ �� ������ ������� ������            
            }
        }
        else
        {
            SetSelectedCounter(null);//���� ������� �� ������ �� ������ ������� ������         
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovmentVectorNormalized();//����� ������� �� ������ ���������� ������ �� ���������

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);//��������� � ������ 3 ��� �� ������� ������ �����

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;//������ ������ ������
        float playerHeight = 2f;//������ ������ ������
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);//�����������(�������) ������ ��� � ����� ������� �� ������� ���������� ��� ������ ������ �������� ������� ��������� �� ����

        if (!canMove)
        {
            //������ �������� � moveDir

            //��������� �������� ������ �� �
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                //����� �������� ������ �� �
                moveDir = moveDirX;
            }
            else
            {
                //�� ����� �������� ������ �� �
                //��������� ��������� �� Z
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                if (canMove)
                {
                    //����� �������� ������ �� Z
                    moveDir = moveDirZ;
                }
                else
                {
                    //�� ����� ��������� �� � ����� �������
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;//�������� ����������� + ������ + ������ ���� ��� ���� ��� �� �� �������� �� ��� ���� � �������� ��� ������ ��� ����(������ ��������� ������
        }

        isWalking = moveDir != Vector3.zero;//��� � ����� ���� ������ �� 0 �� ���������� ���
        float rotateSpeed = 10f;//�������� ��������
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);//����� ������ ����������� �������� ������ ���������� ���������

    }
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangeEventArgs//���� ? ������������ ����� ����� ���� ��������������� ����� ���
        {
            selectedCounter = selectedCounter//���������� ������� ����� ������� ���� ��� ���� ����������������(�� ����� ���)
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

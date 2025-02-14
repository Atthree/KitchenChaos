using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour,IKitchenObjectParents
{
    // Singleton instance, Player s�n�f�n�n tek bir �rne�i olacak
    public static Player Instance { get; private set; }

    // Static bir Player referans� daha (Instance yerine kullan�labilir)
    public static Player instanceField;

    public event EventHandler OnPickedSomething;
    // Se�ili olan counter'�n de�i�ti�i event
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    
    // Event arg�manlar� i�in bir s�n�f tan�ml�yoruz, burada se�ilen counter bilgisi yer alacak
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter; // Se�ilen counter'� tutacak
    }

    // Karakterin hareket h�z� (SerializeField ile Inspector'dan ayarlanabilir)
    [SerializeField] private float moveSpeed = 7f;

    // Oyuncu girdi s�n�f� referans� (Inspector'dan atanabilir)
    [SerializeField] private GameInput gameInput;

    // Raycast'in hangi katmanlar� hedefleyece�ini belirlemek i�in kullan�l�yor
    [SerializeField] private LayerMask counterLayerMask;

    [SerializeField] private Transform kitchenObjectHoldPoint;

    // Oyuncunun y�r�me durumu
    private bool isWalking;

    // Son etkile�im y�n�n� tutar
    private Vector3 lastInteractDir;

    // �u anda se�ili olan ClearCounter referans�
    private BaseCounter selectedCounter;

    private KitchenObject kitchenObject;

    private void Awake()
    {
        // E�er birden fazla Player instance'� varsa hata verecek
        if (Instance != null)
        {
            Debug.LogError("There is more than one player instance");
        }
        // Singleton pattern: Instance bu objeyi g�sterecek
        Instance = this;
    }

    private void Start()
    {
        // Oyuncunun etkile�im yapma aksiyonuna abone olunuyor
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
        
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;


        if (selectedCounter != null) // E�er se�ilen bir counter varsa
        {
            selectedCounter.InteractAlternate(this); // Etkile�imi ger�ekle�tir
        }
    }

    // Etkile�im yap�lmak istendi�inde tetiklenecek method
    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null) // E�er se�ilen bir counter varsa
        {
            selectedCounter.Interact(this); // Etkile�imi ger�ekle�tir
        }
    }

    private void Update()
    {
        // Her karede oyuncunun hareketini ve etkile�imlerini kontrol et
        HandleMovement();  // Hareket i�lemleri
        HandleInteractions();  // Etkile�im i�lemleri
    }

    // Oyuncu y�r�yor mu kontrol eden metod
    public bool IsWalking()
    {
        return isWalking;
    }

    // Etkile�imlerin y�netildi�i metod
    private void HandleInteractions()
    {
        // Oyuncu hareket girdisi al�n�yor
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // 2D vekt�r 3D hale getirilip hareket y�n� belirleniyor
        Vector3 movDir = new Vector3(inputVector.x, 0f, inputVector.y);

        // E�er hareket vekt�r� s�f�r de�ilse son etkile�im y�n� g�ncelleniyor
        if (movDir != Vector3.zero)
        {
            lastInteractDir = movDir;
        }

        // Raycast ile etkile�im mesafesi
        float interactDistance = 2f;

        // Raycast kullanarak kar��da bir nesne olup olmad��� kontrol ediliyor
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, counterLayerMask))
        {
            // E�er kar��da bir ClearCounter varsa
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // Yeni se�ilen counter daha �ncekiyle ayn� de�ilse g�ncelle
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter); // Yeni counter'� se�
                }
            }
            else
            {
                SetSelectedCounter(null); // E�er bir ClearCounter bulunamad�ysa null yap
            }
        }
        else
        {
            SetSelectedCounter(null); // Raycast bir �ey bulamad�ysa null yap
        }

        // Se�ilen counter'�n ne oldu�unu her karede logluyoruz
        Debug.Log(selectedCounter);
    }

    // Oyuncu hareketinin y�netildi�i metod
    private void HandleMovement()
    {
        // Hareket girdisi al�n�yor
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // Hareket y�n� belirleniyor
        Vector3 movDir = new Vector3(inputVector.x, 0f, inputVector.y);

        // Hareket mesafesi hesaplan�yor (h�z ile zaman �arp�m�)
        float moveDistance = moveSpeed * Time.deltaTime;

        // �arp��may� kontrol etmek i�in karakterin boyutu
        float playerRadius = .7f;
        float playerHeight = 2f;

        // Hareket y�n�nde �arp��ma olup olmad��� kontrol ediliyor
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movDir, moveDistance);

        // E�er �arp��ma varsa alternatif eksenlerde hareketi dene
        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(movDir.x, 0, 0).normalized;
            canMove = movDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
            if (canMove)
            {
                movDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, movDir.z).normalized;
                canMove = movDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    movDir = moveDirZ;
                }
            }
        }

        // E�er hareket edilebiliyorsa karakteri hareket ettir
        if (canMove)
        {
            transform.position += movDir * moveDistance;
        }

        // Karakterin y�r�yor olup olmad���n� belirliyor
        isWalking = movDir != Vector3.zero;

        // Karakterin y�n�n� hareket etti�i y�ne do�ru �evir
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, movDir, Time.deltaTime * rotateSpeed);
    }

    // Se�ilen counter'� ayarlayan metod
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter; // Se�ilen counter'� g�ncelle
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = selectedCounter }); // Event'i tetikle
    }
    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint; // Bu, counter'�n �st�ndeki nokta
    }

    // KitchenObject'i ayarlar
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this,EventArgs.Empty);
        }
    }

    // KitchenObject'i d�ner
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    // KitchenObject referans�n� temizler (null yapar)
    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    // Counter'da KitchenObject olup olmad���n� kontrol eder
    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}

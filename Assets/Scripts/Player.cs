using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour,IKitchenObjectParents
{
    // Singleton instance, Player sýnýfýnýn tek bir örneði olacak
    public static Player Instance { get; private set; }

    // Static bir Player referansý daha (Instance yerine kullanýlabilir)
    public static Player instanceField;

    public event EventHandler OnPickedSomething;
    // Seçili olan counter'ýn deðiþtiði event
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    
    // Event argümanlarý için bir sýnýf tanýmlýyoruz, burada seçilen counter bilgisi yer alacak
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter; // Seçilen counter'ý tutacak
    }

    // Karakterin hareket hýzý (SerializeField ile Inspector'dan ayarlanabilir)
    [SerializeField] private float moveSpeed = 7f;

    // Oyuncu girdi sýnýfý referansý (Inspector'dan atanabilir)
    [SerializeField] private GameInput gameInput;

    // Raycast'in hangi katmanlarý hedefleyeceðini belirlemek için kullanýlýyor
    [SerializeField] private LayerMask counterLayerMask;

    [SerializeField] private Transform kitchenObjectHoldPoint;

    // Oyuncunun yürüme durumu
    private bool isWalking;

    // Son etkileþim yönünü tutar
    private Vector3 lastInteractDir;

    // Þu anda seçili olan ClearCounter referansý
    private BaseCounter selectedCounter;

    private KitchenObject kitchenObject;

    private void Awake()
    {
        // Eðer birden fazla Player instance'ý varsa hata verecek
        if (Instance != null)
        {
            Debug.LogError("There is more than one player instance");
        }
        // Singleton pattern: Instance bu objeyi gösterecek
        Instance = this;
    }

    private void Start()
    {
        // Oyuncunun etkileþim yapma aksiyonuna abone olunuyor
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
        
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;


        if (selectedCounter != null) // Eðer seçilen bir counter varsa
        {
            selectedCounter.InteractAlternate(this); // Etkileþimi gerçekleþtir
        }
    }

    // Etkileþim yapýlmak istendiðinde tetiklenecek method
    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null) // Eðer seçilen bir counter varsa
        {
            selectedCounter.Interact(this); // Etkileþimi gerçekleþtir
        }
    }

    private void Update()
    {
        // Her karede oyuncunun hareketini ve etkileþimlerini kontrol et
        HandleMovement();  // Hareket iþlemleri
        HandleInteractions();  // Etkileþim iþlemleri
    }

    // Oyuncu yürüyor mu kontrol eden metod
    public bool IsWalking()
    {
        return isWalking;
    }

    // Etkileþimlerin yönetildiði metod
    private void HandleInteractions()
    {
        // Oyuncu hareket girdisi alýnýyor
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // 2D vektör 3D hale getirilip hareket yönü belirleniyor
        Vector3 movDir = new Vector3(inputVector.x, 0f, inputVector.y);

        // Eðer hareket vektörü sýfýr deðilse son etkileþim yönü güncelleniyor
        if (movDir != Vector3.zero)
        {
            lastInteractDir = movDir;
        }

        // Raycast ile etkileþim mesafesi
        float interactDistance = 2f;

        // Raycast kullanarak karþýda bir nesne olup olmadýðý kontrol ediliyor
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, counterLayerMask))
        {
            // Eðer karþýda bir ClearCounter varsa
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // Yeni seçilen counter daha öncekiyle ayný deðilse güncelle
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter); // Yeni counter'ý seç
                }
            }
            else
            {
                SetSelectedCounter(null); // Eðer bir ClearCounter bulunamadýysa null yap
            }
        }
        else
        {
            SetSelectedCounter(null); // Raycast bir þey bulamadýysa null yap
        }

        // Seçilen counter'ýn ne olduðunu her karede logluyoruz
        Debug.Log(selectedCounter);
    }

    // Oyuncu hareketinin yönetildiði metod
    private void HandleMovement()
    {
        // Hareket girdisi alýnýyor
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // Hareket yönü belirleniyor
        Vector3 movDir = new Vector3(inputVector.x, 0f, inputVector.y);

        // Hareket mesafesi hesaplanýyor (hýz ile zaman çarpýmý)
        float moveDistance = moveSpeed * Time.deltaTime;

        // Çarpýþmayý kontrol etmek için karakterin boyutu
        float playerRadius = .7f;
        float playerHeight = 2f;

        // Hareket yönünde çarpýþma olup olmadýðý kontrol ediliyor
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movDir, moveDistance);

        // Eðer çarpýþma varsa alternatif eksenlerde hareketi dene
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

        // Eðer hareket edilebiliyorsa karakteri hareket ettir
        if (canMove)
        {
            transform.position += movDir * moveDistance;
        }

        // Karakterin yürüyor olup olmadýðýný belirliyor
        isWalking = movDir != Vector3.zero;

        // Karakterin yönünü hareket ettiði yöne doðru çevir
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, movDir, Time.deltaTime * rotateSpeed);
    }

    // Seçilen counter'ý ayarlayan metod
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter; // Seçilen counter'ý güncelle
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = selectedCounter }); // Event'i tetikle
    }
    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint; // Bu, counter'ýn üstündeki nokta
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

    // KitchenObject'i döner
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    // KitchenObject referansýný temizler (null yapar)
    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    // Counter'da KitchenObject olup olmadýðýný kontrol eder
    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}

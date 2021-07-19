using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public bool isMouse;

    [SerializeField] private float speed = 6f;
    [SerializeField] private float dashCd = 1f;

    private IMoveStrategy moveStrategy;

    private float currentDashCd;

    private Camera mainCamera;
    private Vector3 moveDirection;

    public Vector2 InputVector { get; set; }
    public Vector2 Aim { get; set; }
    public Vector3 MoveDirection => moveDirection;
    public Rigidbody RB { get; private set; }

    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        currentDashCd = dashCd;
        SetMoveStrategy(UpgradeEnum.BasicMove);
    }

    private void FixedUpdate()
    {
        HandleInput();

        moveDirection = new Vector3(InputVector.x, 0f, InputVector.y);

        if (moveDirection.magnitude >= 0.1f)
            RB.AddForce(moveDirection * (speed * 2000));

        currentDashCd -= Time.deltaTime;
    }

    private void HandleInput()
    {
        if (isMouse)
        {
            HandleMouse();
            return;
        }

        HandleGamepad();
    }

    private void HandleGamepad()
    {
        if (!(Aim.magnitude > 0.01f)) return;
        Quaternion qTo = Quaternion.LookRotation(new Vector3(Aim.x, 0, Aim.y));
        qTo = Quaternion.Slerp(transform.rotation, qTo, 10 * Time.deltaTime);
        RB.MoveRotation(qTo);
    }

    private void HandleMouse()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        
        Ray ray = mainCamera.ScreenPointToRay(Aim);

        if (!playerPlane.Raycast(ray, out float hitDistance)) return;
        Vector3 targetPoint = ray.GetPoint(hitDistance);
        Quaternion qTo = Quaternion.LookRotation(targetPoint - transform.position);
        qTo = Quaternion.Slerp(transform.rotation,
            Quaternion.Euler(qTo.eulerAngles.x, qTo.eulerAngles.y, qTo.eulerAngles.z), 10 * Time.deltaTime);
        RB.MoveRotation(qTo);
    }

    public void OnDash()
    {
        if (!(currentDashCd <= 0)) return;
        StartCoroutine(moveStrategy.Dash());
        currentDashCd = dashCd;
    }

    public void SetMoveStrategy(UpgradeEnum upgradeEnum)
    {
        switch (upgradeEnum)
        {
            case UpgradeEnum.BasicMove:
                SetMoveStrategy(new BasicMove(this));
                break;
            case UpgradeEnum.DoubleDash:
                SetMoveStrategy(new DoubleDash(this));
                break;
        }
    }

    private void SetMoveStrategy(IMoveStrategy strategy)
    {
        moveStrategy = strategy;
    }
}
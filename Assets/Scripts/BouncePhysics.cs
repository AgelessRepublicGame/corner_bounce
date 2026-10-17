using UnityEngine;
using UnityEngine.EventSystems;

public class BouncePhysics : MonoBehaviour
{
    public static BouncePhysics Instance;

    [Header("Sprite")]
    public SpriteRenderer spriteRenderer;
    public Sprite bounceSprite;

    [Header("Movement")]
    public float launchMultiplier = 0.02f;
    public float friction = 0.995f;
    public float stopThreshold = 0.05f;
    public float minSwipeDistance = 25f;

    [Header("Corner Detection")]
    public float cornerDistance = 0.2f;

    private Vector2 velocity;
    private Vector2 swipeStart;
    private bool isTrackingSwipe;
    private bool isMoving;

    private float leftBoundary;
    private float rightBoundary;
    private float topBoundary;
    private float bottomBoundary;

    private bool cornerCooldown;
    private Camera gameCamera;
    private int lastScreenWidth;
    private int lastScreenHeight;
    private Rect lastSafeArea;
    private float lastOrthographicSize;
    private Vector3 lastCameraPosition;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (spriteRenderer != null && bounceSprite != null)
            spriteRenderer.sprite = bounceSprite;

        gameCamera = Camera.main;
        RecalculateBounds();
    }

    private void Update()
    {
        RecalculateBoundsIfNeeded();
        HandleSwipe();
        if (!isMoving)
            return;
        MoveSquare();
    }

    private void HandleSwipe()
    {
        if (isMoving || GameManager.Instance == null || !GameManager.Instance.IsPlaying)
        {
            isTrackingSwipe = false;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI())
                return;

            swipeStart = Input.mousePosition;
            isTrackingSwipe = true;
        }

        if (Input.GetMouseButtonUp(0) && isTrackingSwipe)
        {
            Vector2 swipeEnd = Input.mousePosition;
            Vector2 swipeDirection = swipeEnd - swipeStart;

            isTrackingSwipe = false;

            if (swipeDirection.sqrMagnitude < minSwipeDistance * minSwipeDistance)
                return;

            velocity = swipeDirection * launchMultiplier;
            isMoving = true;
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        return EventSystem.current.IsPointerOverGameObject();
    }

    private void MoveSquare()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);

        velocity *= friction;

        CheckWallBounce();
        CheckCorners();

        if (velocity.magnitude < stopThreshold)
        {
            velocity = Vector2.zero;
            isMoving = false;

            GameManager.Instance.GameOver();
        }
    }

    public void RecalculateBounds()
    {
        if (gameCamera == null)
            gameCamera = Camera.main;

        if (gameCamera == null)
            return;

        Rect safeArea = Screen.safeArea;
        float paddingPixels = SettingsManager.Instance != null ? SettingsManager.Instance.PlayAreaPaddingPixels : 0f;
        float maxPaddingPixels = Mathf.Min(safeArea.width, safeArea.height) * 0.5f;
        paddingPixels = Mathf.Min(paddingPixels, maxPaddingPixels);

        float cameraDistance = transform.position.z - gameCamera.transform.position.z;
        Vector3 bottomLeft = gameCamera.ScreenToWorldPoint(new Vector3(safeArea.xMin + paddingPixels, safeArea.yMin + paddingPixels, cameraDistance));
        Vector3 topRight = gameCamera.ScreenToWorldPoint(new Vector3(safeArea.xMax - paddingPixels, safeArea.yMax - paddingPixels, cameraDistance));
        Vector2 spriteExtents = GetSpriteExtents();

        leftBoundary = bottomLeft.x + spriteExtents.x;
        rightBoundary = topRight.x - spriteExtents.x;

        bottomBoundary = bottomLeft.y + spriteExtents.y;
        topBoundary = topRight.y - spriteExtents.y;

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        lastSafeArea = safeArea;
        lastOrthographicSize = gameCamera.orthographicSize;
        lastCameraPosition = gameCamera.transform.position;
    }

    private void RecalculateBoundsIfNeeded()
    {
        if (gameCamera == null)
            gameCamera = Camera.main;

        if (gameCamera == null)
            return;

        if (Screen.width != lastScreenWidth ||
            Screen.height != lastScreenHeight ||
            Screen.safeArea != lastSafeArea ||
            !Mathf.Approximately(gameCamera.orthographicSize, lastOrthographicSize) ||
            gameCamera.transform.position != lastCameraPosition)
        {
            RecalculateBounds();
        }
    }

    private Vector2 GetSpriteExtents()
    {
        if (spriteRenderer == null)
            return Vector2.zero;

        return spriteRenderer.bounds.extents;
    }

    private void CheckWallBounce()
    {
        Vector2 position = transform.position;

        if (position.x <= leftBoundary && velocity.x < 0f)
        {
            position.x = leftBoundary;
            velocity.x *= -1f;
        }

        if (position.x >= rightBoundary && velocity.x > 0f)
        {
            position.x = rightBoundary;
            velocity.x *= -1f;
        }

        if (position.y <= bottomBoundary && velocity.y < 0f)
        {
            position.y = bottomBoundary;
            velocity.y *= -1f;
        }

        if (position.y >= topBoundary && velocity.y > 0f)
        {
            position.y = topBoundary;
            velocity.y *= -1f;
        }

        transform.position = position;
    }

    private void CheckCorners()
    {
        if (cornerCooldown)
            return;

        Vector2 position = transform.position;

        Vector2 topLeft = new Vector2(leftBoundary, topBoundary);

        Vector2 topRight = new Vector2(rightBoundary, topBoundary);

        Vector2 bottomLeft = new Vector2(leftBoundary, bottomBoundary);

        Vector2 bottomRight = new Vector2(rightBoundary, bottomBoundary);

        if (Vector2.Distance(position, topLeft) <= cornerDistance)
            CornerHit(topLeft);

        else if (Vector2.Distance(position, topRight) <= cornerDistance)
            CornerHit(topRight);

        else if (Vector2.Distance(position, bottomLeft) <= cornerDistance)
            CornerHit(bottomLeft);

        else if (Vector2.Distance(position, bottomRight) <= cornerDistance)
            CornerHit(bottomRight);
    }

    private void CornerHit(Vector2 cornerPosition)
    {
        cornerCooldown = true;

        GameManager.Instance.AddScore();

        UIManager.Instance.SpawnCornerParticles(cornerPosition);

        if (SettingsManager.Instance.SoundEnabled)
        {
            UIManager.Instance.PlayCornerSound();
        }

        Invoke(nameof(ResetCornerCooldown), 0.25f);
    }

    private void ResetCornerCooldown()
    {
        cornerCooldown = false;
    }

    public void ResetSquare()
    {
        RecalculateBounds();

        transform.position = GetPlayAreaCenter();
        velocity = Vector2.zero;
        isTrackingSwipe = false;
        isMoving = false;
    }

    private Vector3 GetPlayAreaCenter()
    {
        return new Vector3(
            (leftBoundary + rightBoundary) * 0.5f,
            (bottomBoundary + topBoundary) * 0.5f,
            transform.position.z);
    }
}
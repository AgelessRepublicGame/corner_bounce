using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace stickyfrogdemo
{

    public class Player : MonoBehaviour
    {
        public float power = 10f;
        public LineRenderer trajectoryLine;
        public int lineSegmentCount = 20;
        public float dragMultiplier = 1.5f;
        private Vector2 dragStartPos;
        private Rigidbody2D rb;
        private bool isDragging = false;
        private Camera cam;
        bool isFixedToWall = false;
        public Transform face, body;
        public float faceMoveDistance = 0.3f;
        Vector3 originalScale;
        bool lost = false;
        public GameObject splatterGO, splatterEx;
        public Sprite[] splatterSprites;
        void Start()
        {
            originalScale = body.localScale;
            rb = GetComponent<Rigidbody2D>();
            cam = Camera.main;
            trajectoryLine.positionCount = lineSegmentCount;
            trajectoryLine.enabled = false;
        }

        void Update()
        {
            if (!lost && !IsPointerOverUIElement())
            {
                if (Input.GetMouseButtonDown(0) && isFixedToWall)
                {
                    isDragging = true;
                    dragStartPos = cam.ScreenToWorldPoint(Input.mousePosition);
                    trajectoryLine.enabled = true;
                }

                if (Input.GetMouseButton(0) && isDragging)
                {
                    Vector2 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 dragVector = dragStartPos - currentPos;
                    ShowTrajectory(dragVector * power * dragMultiplier);

                    if (dragVector.sqrMagnitude > 0.001f)
                    {
                        Vector2 direction = dragVector.normalized;
                        face.localPosition = direction * faceMoveDistance;
                    }
                    else
                    {
                        face.localPosition = Vector2.zero;
                    }
                }

                if (Input.GetMouseButtonUp(0) && isDragging)
                {
                    isDragging = false;
                    trajectoryLine.enabled = false;

                    Vector2 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 dragVector = dragStartPos - currentPos;
                    rb.linearVelocity = Vector2.zero;
                    rb.AddForce(dragVector * power * dragMultiplier, ForceMode2D.Impulse);
                    jumpAni();
                    if (isFixedToWall)
                        UnfixFromWall();
                }
            }

        }

        void ShowTrajectory(Vector2 force)
        {
            Vector2 startPos = rb.position;
            Vector2 velocity = force / rb.mass;
            float timeStep = Time.fixedDeltaTime;

            for (int i = 0; i < lineSegmentCount; i++)
            {
                float t = i * timeStep;
                Vector2 point = startPos + velocity * t + 0.5f * Physics2D.gravity * t * t;
                trajectoryLine.SetPosition(i, point);
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("wall") || other.gameObject.CompareTag("door"))
            {
                isFixedToWall = true;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("wall") || other.gameObject.CompareTag("door"))
            {
                UnfixFromWall();
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("wall") || other.gameObject.CompareTag("door"))
            {
                ContactPoint2D contact = other.GetContact(0);
                if (other.gameObject.CompareTag("wall"))
                {
                    this.transform.parent = other.transform;
                    GameObject go = Instantiate(splatterGO, contact.point, Quaternion.identity);
                    go.transform.parent = other.transform;
                    go.GetComponent<SpriteRenderer>().sprite = splatterSprites[Random.Range(0, splatterSprites.Length)];
                }
                Instantiate(splatterEx, contact.point, Quaternion.identity);
                FixToWall();
                bounceAni();
            }
        }

        private void FixToWall()
        {
            isFixedToWall = true;
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        private void UnfixFromWall()
        {
            isFixedToWall = false;
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 1f;
            if (this.transform.parent != null && transform.parent.gameObject.activeInHierarchy)
                this.transform.parent = null;
        }

        void bounceAni()
        {
            body.transform.localScale = Vector3.one;
            DOPunchScale(body.transform, new Vector2(Random.Range(-0.6f, -0.3f), Random.Range(-0.6f, -0.3f)), 0.3f, 1, 1);
        }

        void jumpAni()
        {
            body.transform.localScale = Vector3.one;
            DOPunchScale(body.transform, new Vector3(0.3f, 0.3f, 0), 0.3f, 1, 1);
        }

        public static bool IsPointerOverUIElement()
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults());
        }

        public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                    return true;
            }
            return false;
        }
        static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }



        public void PlayGooeyEffect(Transform body, Vector2 squashAmount, float duration)
        {
            AnimationCurve gooeyCurve = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.3f, 1.2f),
                new Keyframe(0.7f, 0.9f),
                new Keyframe(1f, 1f)
            );

            StartCoroutine(GooeyRoutine(body, squashAmount, duration, gooeyCurve));
        }
        private IEnumerator GooeyRoutine(Transform body, Vector2 squashAmount, float duration, AnimationCurve curve)
        {

            Vector3 targetScale = new Vector3(
                originalScale.x + squashAmount.x,
                originalScale.y + squashAmount.y,
                originalScale.z
            );

            float time = 0f;

            while (time < duration)
            {
                float t = curve.Evaluate(time / duration);
                body.localScale = Vector3.LerpUnclamped(originalScale, targetScale, t);
                time += Time.deltaTime;
                yield return null;
            }

            body.localScale = originalScale;
        }

        public void DOPunchScale(Transform target, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1f)
        {
            StartCoroutine(PunchScaleRoutine(target, punch, duration, vibrato, elasticity));
        }

        private IEnumerator PunchScaleRoutine(Transform target, Vector3 punch, float duration, int vibrato, float elasticity)
        {
            Vector3 originalScale = target.localScale;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                int oscillations = vibrato;
                float dampingFactor = Mathf.Pow(1f - t, elasticity);
                float oscillation = Mathf.Sin(t * Mathf.PI * oscillations) * dampingFactor;

                Vector3 punchScale = originalScale + punch * oscillation;

                target.localScale = punchScale;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            target.localScale = originalScale;
        }

        public void DOScaleFunc(Transform trans, float endScale, float duration)
        {
            StartCoroutine(DOScale(trans, endScale, duration));
        }

        private IEnumerator DOScale(Transform trans, float endScale, float duration)
        {
            Vector3 startScale = trans.localScale;
            float timeElapsed = 0f;

            while (timeElapsed < duration)
            {
                trans.localScale = Vector3.Lerp(startScale, new Vector3(endScale, endScale, endScale), timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            trans.localScale = new Vector3(endScale, endScale, endScale);
        }

    }
}
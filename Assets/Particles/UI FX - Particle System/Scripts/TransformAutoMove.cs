using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetStoreBase
{
    public class TransformAutoMove : MonoBehaviour
    {
        public Vector3 moveDistance = new Vector3(0, -1.0f, 0);
        public float moveDuration = 1.0f;
        public float waitDuration = 1.0f;

        private void Start()
        {
            StartCoroutine(MoveDownRoutine());
        }

        private IEnumerator MoveDownRoutine()
        {
            yield return new WaitForSeconds(1f + waitDuration);
            while (true)
            {
                Vector3 targetPosition = transform.position + moveDistance;
                yield return StartCoroutine(MoveToPosition(transform, targetPosition, moveDuration));
                yield return new WaitForSeconds(waitDuration);
            }
        }

        private IEnumerator MoveToPosition(Transform target, Vector3 position, float timeToMove)
        {
            var currentPos = target.position;
            var t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / timeToMove;
                target.position = Vector3.Lerp(currentPos, position, t);
                yield return null;
            }
        }
    }
}
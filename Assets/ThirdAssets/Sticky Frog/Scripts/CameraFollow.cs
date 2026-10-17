using UnityEngine;
namespace stickyfrogdemo
{
    public class CameraFollow : MonoBehaviour
    {

        public Transform target;
        public float smoothTime = 0.3F;
        private Vector3 velocity = Vector3.zero;

        private void Start()
        {
            this.GetComponent<Camera>().orthographicSize = 6;
        }

        void FixedUpdate()
        {
            if (target != null)
            {
                Vector3 targetPosition = target.TransformPoint(new Vector3(0, 0, -10));
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            }
        }
    }
}

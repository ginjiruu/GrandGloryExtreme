using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class CameraManager : MonoBehaviour
    {
        public bool lockon;

        public float followSpeed = 9;
        public float mouseSpeed = 2;
        public float controllerSpeed = 7;

        public Transform target;
        

        [HideInInspector]
        public Transform pivot;
        [HideInInspector]
        public Transform camTrans;
        StatesManager states;

        float turnSmooting = .1f;
        public float minAngle = -35;
        public float maxAngle = 35;

        float smoothX;
        float smoothY;

        float smoothXVelocity;
        float smoothYVelocity;

        public float lookAngle;
        public float tiltAngle;

        #region othercamera
        public bool trdPerson;
        public List<Transform> targets;
        public Vector3 offset;
        public float smoothing = 0.5f;
        private Vector3 velSmooth;

        public float minZoom = 5.0f;
        public float maxZoom = 15.0f;
        public float zoomLimiter = 0.5f;
        private Camera cam;
        #endregion

        bool usedRightAxis;

        public void Init(StatesManager st)
        {
            camTrans = Camera.main.transform;
            cam = GetComponentInChildren<Camera>();
            pivot = camTrans.parent;
        }

        public void Tick(float d)
        {
            if (trdPerson)
                return;
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");

            float c_h = Input.GetAxis("RightAxis X");
            float c_v = Input.GetAxis("RightAxis Y");

            float targetSpeed = mouseSpeed;

            if(usedRightAxis)
            {
                if (Mathf.Abs(c_h) < 0.6f)
                    usedRightAxis = false;
            }

            if (c_h != 0 || c_v != 0)
            {
                h = c_h;
                v = c_v;
                targetSpeed = controllerSpeed;
            }
            FollowTarget(d);
            HandleRotations(d, v, h, targetSpeed);
        }

        void FollowTarget(float d)
        {
            float speed = d * followSpeed;
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPosition;
        }

        void HandleRotations(float d, float v, float h, float targetSpeed)
        {
            if (turnSmooting > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXVelocity, turnSmooting);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYVelocity, turnSmooting);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
        }

        public static CameraManager singleton;

        private void Awake()
        {
            singleton = this;
        }
        void LateUpdate()
        {
            if (trdPerson)
            {
                //Make sure targets isnt empty
                if (targets.Count == 0)
                    return;
                Move();
                Zoom();
            }
        }
        void Move()
        {
            Vector3 centerPoint = getCenterPoint();

            Vector3 newCenter = centerPoint + offset;
            transform.position = Vector3.SmoothDamp(transform.position, newCenter, ref velSmooth, smoothing);


        }
        void Zoom()
        {

            float newZoom = Mathf.Lerp(minZoom, maxZoom, (getMaxDistance() - 5) * zoomLimiter);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
        }



        float getMaxDistance()
        {

            var bounds = new Bounds(targets[0].position, Vector3.zero);
            for (int i = 0; i < targets.Count; i++)
                bounds.Encapsulate(targets[i].position);

            return bounds.size.magnitude;
        }

        Vector3 getCenterPoint()
        {

            var bounds = new Bounds(targets[0].position, Vector3.zero);
            for (int i = 0; i < targets.Count; i++)
                bounds.Encapsulate(targets[i].position);

            return bounds.center;
        }
    }
}

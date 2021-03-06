using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour {
        public static Camera2DFollow staticCam;

        [SerializeField] Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;
        public Transform Target { get { return target; } set { target = value; } }

        // Use this for initialization
        private void Start() {
            staticCam = this;
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            // only update lookahead pos if accelerating or changed direction
            Vector2 moveDirection = target.position - m_LastTargetPosition;
            var moveDelta = moveDirection.magnitude;

            var updateLookAheadTarget = Mathf.Abs(moveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
                m_LookAheadPos = lookAheadFactor * ((Vector2)transform.position + moveDirection);//Vector3.right*Mathf.Sign(moveDelta);
            else
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);

            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward*m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            transform.position = newPos;

            m_LastTargetPosition = target.position;
        }
    }
}

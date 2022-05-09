using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils_Scripts
{
    public class MoveTowardsPosition : MonoBehaviour
    {
        public Vector3 TargetPosition;
        public Action OnCompleteAction;

        public void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, 2f * Time.deltaTime);
            if (_vector2DDistance(TargetPosition, transform.position) < 0.2f)
            {
                OnCompleteAction.Invoke();
                Destroy(this);
            }
        }

        private float _vector2DDistance(Vector3 v1, Vector3 v2)
        {
            float xDiff = v1.x - v2.x;
            float zDiff = v1.z - v2.z;
            return Mathf.Sqrt((xDiff * xDiff) + (zDiff * zDiff));
        }
    }
}

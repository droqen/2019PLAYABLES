using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace navdi3.pixel
{

    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]

    public class RatioKeeper : MonoBehaviour
    {
        public Vector2Int gameSize;
        Camera cam { get { return GetComponent<Camera>(); } }
        private void Update()
        {
            // read ratio, apply to cam orthographic size
            if (!cam.orthographic)
            {
                cam.orthographic = true;
                Dj.Warnf("Camera {0} has a RatioKeeper and must be Orthographic.", gameObject.name);
            }

            int largestValidRatio = Mathf.Min(Screen.width / gameSize.x, Screen.height / gameSize.y);
            if (largestValidRatio < 1) largestValidRatio = 1;

            transform.localScale = new Vector3(1f * gameSize.x / gameSize.y, 1, 1);

            cam.orthographicSize = Screen.height * 1f / gameSize.y / largestValidRatio;
            //cam.orthographicSize = Screen.height * 1f / largestValidRatio;
        }
    }

}
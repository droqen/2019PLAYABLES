using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace navdi3.pixel {

    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class SmartPixelPerfectCamera : MonoBehaviour
    {
        public Vector2Int gameSize = new Vector2Int(160, 144);

        public RatioKeeper renderHelperCameraRatioKeeper;

        public RenderTexture rentex;
        public Material renmat;
        public float pixelSize = 1f;
        public float zPosition = -10;
        //Vector3 realPosition;

        private void Start()
        {
            //realPosition = transform.position;
            RecreateRentex();
        }

        void RecreateRentex()
        {
            rentex = new RenderTexture(gameSize.x, gameSize.y, 16);
            rentex.filterMode = FilterMode.Point;
            GetComponent<Camera>().targetTexture = rentex;
            renmat.mainTexture = rentex;
        }

        private void Update()
        {
            if (gameSize.x < 1) gameSize.x = 1;
            if (gameSize.y < 1) gameSize.y = 1;
            if (rentex == null || rentex.width != gameSize.x || rentex.height != gameSize.y)
            {
                RecreateRentex();
            }
            if (renmat.mainTexture != rentex) renmat.mainTexture = rentex;
            renderHelperCameraRatioKeeper.gameSize = this.gameSize;
            zPosition = transform.position.z;
        }

        //private void LateUpdate()
        //{
        //    this.transform.position = new Vector3(
        //        Mathf.RoundToInt(realPosition.x),
        //        Mathf.RoundToInt(realPosition.y),
        //        zPosition
        //    );
        //}
    }

}
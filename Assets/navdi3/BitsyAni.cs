using UnityEngine;
using System.Collections;

namespace navdi3
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class BitsyAni : MonoBehaviour
    {
        SpriteRenderer spriter { get { return GetComponent<SpriteRenderer>(); } }
        public float speed = .05f;
        public int[] spriteIds = { 0, 1 };
        public SpriteLot spriteLot;
        public float anim;
        private void FixedUpdate()
        {
            if (spriter && spriteLot) {
                anim = (anim + speed) % (spriteIds.Length);
                spriter.sprite = spriteLot[spriteIds[(int)anim]];
            }
        }
        private void Update()
        {
            if (!Application.isPlaying && spriteLot != null && spriteIds != null && spriteIds.Length > 0)
            {
                spriter.sprite = spriteLot[spriteIds[0]];
            }
        }
    }

}
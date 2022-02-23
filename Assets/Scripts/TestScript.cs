using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts {
    public class TestScript : MonoBehaviour
    {
        public Material GlowMat;

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Move up then back down
                transform.DOMove(new Vector3(2f, 5f), 2f)
                    .Then(Utils.DOWait(0.5f))
                    .Then(transform.DOMove(new Vector3(2f, 0f), 2f));
            }
        }
        private void OnMouseEnter()
        {
            GlowMat.SetFloat("_glowing", 1f);
        }

        private void OnMouseExit()
        {
            GlowMat.SetFloat("_glowing", 0f);
        }
    }
}

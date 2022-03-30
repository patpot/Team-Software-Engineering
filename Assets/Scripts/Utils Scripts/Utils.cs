using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public static class Utils
    {
        // DOTween helpers.

        /// <summary>
        /// Chain Tweens as continuation events.
        /// This replaces the tween's OnComplete event.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="next">the next tween to run.</param>
        /// <returns></returns>
        public static Tween Then(this Tween t, Tween next)
        {
            // Don't play the tween immediately
            next.Pause();
            // Play it when the current one is done!
            t.OnComplete(() => { next.Play(); });
            return next;
        }

        /// <summary>
        /// Chain an Action as a continuation event.
        /// This replaces the tween's OnComplete event,
        /// and is essentially an alias of OnComplete.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="next">the next tween to run.</param>
        /// <returns></returns>
        public static Tween Then(this Tween t, Action then)
        {
            // Create an event that we will complete instantly.
            var next = DOWait(1f);
            // Pause it for now.
            next.Pause();
            // Play it when the current one is done!
            t.OnComplete(() =>
            {
            // Run the callback.
            then?.Invoke();
            // Trigger the next event.
            next.Kill(complete: true);
            });

            return next;
        }

        /// <summary>
        /// Uses DOTween to delay a certain amount of time.
        /// This is most useful when combined with .Then(...) or .OnComplete(...)
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static Tween DOWait(float seconds)
        {
            // Empty tween that just delays some time.
            float start = 0f;
            return DOTween.To(() => start, v => start = v, 1f, duration: seconds);
        }

        // Color opacity/transparency helpers
        public static Color AsTransparent(this Color c)
        {
            return new Color(c.r, c.g, c.b, 0f);
        }
        public static Color AsOpaque(this Color c)
        {
            return new Color(c.r, c.g, c.b, 1f);
        }
        public static Color WithOpacity(this Color c, float a)
        {
            return new Color(c.r, c.g, c.b, a);
        }

        // DOTween auto-fade in for all images in a game object
        public static void FadeInUI(GameObject go)
        {
            foreach (var image in go.GetComponentsInChildren<Image>())
            {
                Color tempColor = image.color;
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
                image.DOColor(tempColor, 0.4f);
            }
        }
    }
}
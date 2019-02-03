using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CompassNavigatorPro {

	public class LetterAnimator : MonoBehaviour {

		public float startTime, revealDuration, startFadeTime, fadeDuration;
		public Text text, textShadow;


		Vector3 localScale;

		void Start () {
			localScale = text.transform.localScale;
			Update ();
		}

		// Update is called once per frame
		void Update () {

			float now = Time.time;
			float elapsed = now - startTime;
			if (elapsed < revealDuration) { // revealing
				float t = Mathf.Clamp01 (elapsed / revealDuration);
				UpdateTextScale (t);
				UpdateTextAlpha (t);
			} else if (now < startFadeTime) {
				UpdateTextScale (1.0f);
				UpdateTextAlpha (1.0f);
			} else if (now < startFadeTime + fadeDuration) {
				float t = Mathf.Clamp01 (1.0f - (now - startFadeTime) / fadeDuration);
				UpdateTextAlpha (t);
			} else {
				Destroy (text.gameObject);
				Destroy (textShadow.gameObject);
			}
		}

		void UpdateTextScale (float t) {
			text.transform.localScale = localScale * t;
			textShadow.transform.localScale = localScale * t;
		}

		void UpdateTextAlpha (float t) {
			text.color = new Color (text.color.r, text.color.g, text.color.b, t);
			textShadow.color = new Color (0, 0, 0, t);
		}



	}

}
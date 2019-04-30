using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CompassNavigatorPro {
	public class DiscoverArea : MonoBehaviour {

		void OnTriggerEnter(Collider col) {
			if (col.tag == "MainCamera") {
				gameObject.SetActive (false);
				CompassPro.instance.ShowAnimatedText ("Area Discovered!");
			}
		}
	}

}
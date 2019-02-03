using UnityEngine;
using System.Collections;

namespace CompassNavigatorPro {
	public class LevelManager : MonoBehaviour {

		public int initialPoiCount = 1;
		public Material sphereMaterial;
		public Sprite[] icons;
		public AudioClip[] soundClips;


		int poiNumber;
		CompassPro compass;

		// Create random POIs
		void Start () {
			// Get a reference to the Compass Pro Navigator component
			compass = CompassPro.instance;

			// Add a callback when POIs are reached
			compass.OnPOIVisited += POIVisited;

			// Populate the scene with initial POIs
			for (int k = 1; k <= initialPoiCount; k++) {
				AddRandomPOI ();
			}

		}

		void Update () {
			if (Input.GetKeyDown (KeyCode.B)) {
				compass.POIShowBeacon (5f, 1.1f, 1f, new Color(1,1,0.25f));
			}
		}


		void AddRandomPOI () {
			// Create placeholder
			GameObject obj = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			obj.transform.position = new Vector3 (Random.Range (-50, 50), 1, Random.Range (-50, 50));
			obj.GetComponent<Renderer> ().material = sphereMaterial;
			
			// Add POI info
			CompassProPOI poi = obj.AddComponent<CompassProPOI> ();

			// Title name and reveal text
			poi.title = "Target " + (++poiNumber).ToString ();
			poi.titleVisibility = TITLE_VISIBILITY.Always;
			poi.visitedText = "Target " + poiNumber + " acquired!";

			// Assign icons
			int j = Random.Range (0, icons.Length / 2);
			poi.iconNonVisited = icons [j * 2];
			poi.iconVisited = icons [j * 2 + 1];

			// Enable GameView gizmo
			poi.showPlayModeGizmo = true;

			// Assign random sound
			j = Random.Range (0, soundClips.Length);
			poi.visitedAudioClip = soundClips [j];
		}


		void POIVisited (CompassProPOI poi) {
			Debug.Log (poi.title + " has been reached.");
			StartCoroutine (RemovePOI (poi));
			AddRandomPOI ();
		}

		IEnumerator RemovePOI (CompassProPOI poi) {
			while (poi.transform.position.y < 5) {
				poi.transform.position += Vector3.up * Time.deltaTime;
				poi.transform.localScale *= 0.9f;
				yield return new WaitForEndOfFrame ();
			}
			Destroy (poi.gameObject);
		}


	}
}
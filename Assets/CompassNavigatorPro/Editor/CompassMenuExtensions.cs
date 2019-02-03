using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CompassNavigatorPro {
	public class CompassMenuExtensions : MonoBehaviour {
		
		[MenuItem("GameObject/Create Other/Compass POI")]
		static void CreateCompassPOI (MenuCommand menuCommand) {
			GameObject poi = Resources.Load<GameObject> ("CNPro/Prefabs/CompassPOI");
			if (poi == null) {
				Debug.LogError ("Could not load CompassPOI from Resources/Prefabs folder!");
				return;
			}
			GameObject newPOI = Instantiate (poi);
			newPOI.name = "Compass POI";
			
			// Ensure it gets reparented if this was a context click (otherwise does nothing)
			GameObjectUtility.SetParentAndAlign (newPOI, menuCommand.context as GameObject);
			
			// Register root object for undo.
			Undo.RegisterCreatedObjectUndo (newPOI, "Create Compass POI");
			Selection.activeObject = newPOI;
		}
	
	}
	
}

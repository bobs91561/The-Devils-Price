using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CompassNavigatorPro
{

	public enum MINIMAP_STYLE
	{
		TornPaper = 0,
		SolidBox = 1,
		SolidCircle = 2,
		Custom = 100
	}


	public enum MINIMAP_CAMERA_MODE
	{
		Perspective = 0,
		Orthographic = 1
	}


	public partial class CompassPro : MonoBehaviour
	{


		#region Events

		/// <summary>
		/// Event fired when this POI appears in the Mini-Map.
		/// </summary>
		public event POIEvent OnPOIVisibleInMiniMap;

		/// <summary>
		/// Event fired when the POI disappears from the Mini-Map
		/// </summary>
		public event POIEvent OnPOIHidesInMiniMap;

		#endregion

		#region Public MiniMap properties


		[SerializeField]
		bool _showMiniMap = false;

		/// <summary>
		/// Show/Hide minimap 
		/// </summary>
		public bool showMiniMap {
			get { return _showMiniMap; }
			set {
				if (value != _showMiniMap) {
					_showMiniMap = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}


		[SerializeField]
		float _miniMapSize = 0.2f;

		/// <summary>
		/// The screen size of the mini-map
		/// </summary>
		public float miniMapSize {
			get { return _miniMapSize; }
			set {
				if (value != _miniMapSize) {
					_miniMapSize = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}


		[SerializeField]
		Camera _miniMapCamera;

		public Camera miniMapCamera {
			get { return _miniMapCamera; }
			set {
				if (_miniMapCamera != value) {
					_miniMapCamera = value; 
					isDirty = true;
				}
			}
		}

		/// <summary>
		/// Where to center the mini map
		/// </summary>
		public Transform miniMapFollow;


		/// <summary>
		/// Optional mini-map mask texture
		/// </summary>
		[SerializeField]
		Sprite _miniMapMaskSprite;

		/// <summary>
		/// The sprite for the mini-map mask
		/// </summary>
		public Sprite miniMapMaskSprite {
			get { return _miniMapMaskSprite; }
			set {
				if (value != _miniMapMaskSprite) {
					_miniMapMaskSprite = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}

		/// <summary>
		/// Optional mini-map border texture
		/// </summary>
		[SerializeField]
		Texture2D _miniMapBorderTexture;

		/// <summary>
		/// Show/Hide minimap 
		/// </summary>
		public Texture2D miniMapBorderTexture {
			get { return _miniMapBorderTexture; }
			set {
				if (value != _miniMapBorderTexture) {
					_miniMapBorderTexture = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}


		/// <summary>
		/// Style for the mini-map
		/// </summary>
		[SerializeField]
		MINIMAP_STYLE _miniMapStyle = MINIMAP_STYLE.TornPaper;

		/// <summary>
		/// Style of mini-map
		/// </summary>
		public MINIMAP_STYLE miniMapStyle {
			get { return _miniMapStyle; }
			set {
				if (value != _miniMapStyle) {
					_miniMapStyle = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}

		[SerializeField]
		int _miniMapResolution = 8;

		/// <summary>
		/// The capture resolution exponent. Texture resolution is 2^miniMapResolution (2^8 = 256)
		/// </summary>
		public int miniMapResolution {
			get { return _miniMapResolution; }
			set {
				if (value != _miniMapResolution) {
					_miniMapResolution = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}



		[SerializeField] MINIMAP_CAMERA_MODE _miniMapCameraMode = MINIMAP_CAMERA_MODE.Orthographic;
		/// <summary>
		/// The sprite for the mini-map mask
		/// </summary>
		public MINIMAP_CAMERA_MODE miniMapCameraMode {
			get { return _miniMapCameraMode; }
			set {
				if (value != _miniMapCameraMode) {
					_miniMapCameraMode = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}

		[SerializeField]
		float _miniMapZoomSize = 5;

		/// <summary>
		/// The orthographic size for the camera
		/// </summary>
		public float miniMapZoomSize {
			get { return _miniMapZoomSize; }
			set {
				if (value != _miniMapZoomSize) {
					_miniMapZoomSize = value;
					UpdateMiniMap ();
					isDirty = true;
				}
			}
		}


		[SerializeField]
		float _miniMapCameraAltitude = 50;

		/// <summary>
		/// The distance from the camera to the following target
		/// </summary>
		public float miniMapCameraAltitude {
			get { return _miniMapCameraAltitude; }
			set {
				if (value != _miniMapCameraAltitude) {
					_miniMapCameraAltitude = value;
					UpdateMiniMap ();
					isDirty = true;
				}
			}
		}



		[SerializeField] int _miniMapLayerMask = 1;
		/// <summary>
		/// The layer mask for the mini-map camera
		/// </summary>
		public int miniMapLayerMask {
			get { return _miniMapLayerMask; }
			set {
				if (value != _miniMapLayerMask) {
					_miniMapLayerMask = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}

		[SerializeField]
		float _miniMapIconSize = 0.5f;

		/// <summary>
		/// The size for the icons on the mini-map
		/// </summary>
		public float miniMapIconSize {
			get { return _miniMapIconSize; }
			set {
				if (value != _miniMapIconSize) {
					_miniMapIconSize = value;
					UpdateMiniMap ();
					isDirty = true;
				}
			}
		}



		[SerializeField]
		float _miniMapClampBorder = 0.02f;

		/// <summary>
		/// The distance to the edge for the clamped icons on the minimap
		/// </summary>
		public float miniMapClampBorder {
			get { return _miniMapClampBorder; }
			set {
				if (value != _miniMapClampBorder) {
					_miniMapClampBorder = value;
					UpdateMiniMap ();
					isDirty = true;
				}
			}
		}


		[SerializeField]
		float _miniMapAlpha = 1.0f;

		/// <summary>
		/// The alpha (transparency) of the mini-map.
		/// </summary>
		public float miniMapAlpha {
			get { return _miniMapAlpha; }
			set {
				if (value != _miniMapAlpha) {
					_miniMapAlpha = value;
					UpdateMiniMap ();
					isDirty = true;
				}
			}
		}


		#endregion


		
	}

}




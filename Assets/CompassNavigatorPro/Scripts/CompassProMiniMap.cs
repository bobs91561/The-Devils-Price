using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CompassNavigatorPro {

	public enum MINIMAP_STYLE {
		TornPaper = 0,
		SolidBox = 1,
		SolidCircle = 2,
		Custom = 100
	}

	public enum MINIMAP_LOCATION {
		TopLeft,
		TopCenter,
		TopRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		BottomLeft,
		BottomCenter,
		BottomRight,
		Custom
	}


	public enum MINIMAP_CAMERA_MODE {
		Perspective = 0,
		Orthographic = 1
	}


	public enum MINIMAP_CAMERA_SNAPSHOT_FREQUENCY {
		Continuous = 0,
		TimeInterval = 1,
		DistanceTravelled = 2
	}


	public partial class CompassPro : MonoBehaviour {


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
		MINIMAP_LOCATION _miniMapLocation = MINIMAP_LOCATION.BottomRight;

		/// <summary>
		/// Minimap screen location
		/// </summary>
		public MINIMAP_LOCATION miniMapLocation {
			get { return _miniMapLocation; }
			set {
				if (value != _miniMapLocation) {
					_miniMapLocation = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}



		[SerializeField]
		Vector2 _miniMapLocationOffset;

		/// <summary>
		/// Minimap screen location offset
		/// </summary>
		public Vector2 miniMapLocationOffset {
			get { return _miniMapLocationOffset; }
			set {
				if (value != _miniMapLocationOffset) {
					_miniMapLocationOffset = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}


		[SerializeField]
		bool _miniMapKeepStraight = false;

		/// <summary>
		/// Keep the mini-map oriented to North
		/// </summary>
		public bool miniMapKeepStraight {
			get { return _miniMapKeepStraight; }
			set {
				if (value != _miniMapKeepStraight) {
					_miniMapKeepStraight = value;
					needMiniMapShot = true;
					needUpdateBarContents = true;
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
		int _miniMapResolutionNormalSize = 256;

		/// <summary>
		/// The capture resolution when minimap is in full-screen mode.
		/// </summary>
		public int miniMapResolutionNormalSize {
			get { return _miniMapResolutionNormalSize; }
			set {
				if (value != _miniMapResolutionNormalSize) {
					_miniMapResolutionNormalSize = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}


		[SerializeField]
		float _miniMapFullScreenSize = 0.9f;

		/// <summary>
		/// The percentage of screen size when minimap is in full screen mode.
		/// </summary>
		public float miniMapFullScreenSize {
			get { return _miniMapFullScreenSize; }
			set {
				if (value != _miniMapFullScreenSize) {
					_miniMapFullScreenSize = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}



		[SerializeField]
		bool _miniMapKeepAspectRatio = true;

		/// <summary>
		/// Keep aspect ration in full screen mode
		/// </summary>
		public bool miniMapKeepAspectRatio {
			get { return _miniMapKeepAspectRatio; }
			set {
				if (value != _miniMapKeepAspectRatio) {
					_miniMapKeepAspectRatio = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}


		[SerializeField]
		bool _miniMapDisableMainCameraInFullScreen = true;

		/// <summary>
		/// Keep aspect ration in full screen mode
		/// </summary>
		public bool miniMapDisableMainCameraInFullScreen {
			get { return _miniMapDisableMainCameraInFullScreen; }
			set {
				if (value != _miniMapDisableMainCameraInFullScreen) {
					_miniMapDisableMainCameraInFullScreen = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}


		[SerializeField] MINIMAP_CAMERA_MODE _miniMapCameraMode = MINIMAP_CAMERA_MODE.Orthographic;

		/// <summary>
		/// Mini-map projection mode
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


		[SerializeField] MINIMAP_CAMERA_SNAPSHOT_FREQUENCY _miniMapCameraSnapshotFrequency = MINIMAP_CAMERA_SNAPSHOT_FREQUENCY.Continuous;

		/// <summary>
		/// How often the mini-map camera will capture the scene
		/// </summary>
		public MINIMAP_CAMERA_SNAPSHOT_FREQUENCY miniMapCameraSnapshotFrequency {
			get { return _miniMapCameraSnapshotFrequency; }
			set {
				if (value != _miniMapCameraSnapshotFrequency) {
					_miniMapCameraSnapshotFrequency = value;
					SetupMiniMap ();
					isDirty = true;
				}
			}
		}

		[SerializeField]
		float _miniMapCaptureSize = 256f;

		/// <summary>
		/// The orthographic camera size
		/// </summary>
		public float miniMapCaptureSize {
			get { return _miniMapCaptureSize; }
			set {
				if (value != _miniMapCaptureSize) {
					_miniMapCaptureSize = value;
					needUpdateBarContents = true;
					needMiniMapShot = true;
					isDirty = true;
				}
			}
		}


		[SerializeField]
		float _miniMapSnapshotInterval = 10f;

		/// <summary>
		/// The time interval between minimap camera shots
		/// </summary>
		public float miniMapSnapshotInterval {
			get { return _miniMapSnapshotInterval; }
			set {
				if (value != _miniMapSnapshotInterval) {
					_miniMapSnapshotInterval = value;
					isDirty = true;
				}
			}
		}


		[SerializeField]
		float _miniMapSnapshotDistance = 10f;

		/// <summary>
		/// The distance interval between minimap camera shots
		/// </summary>
		public float miniMapSnapshotDistance {
			get { return _miniMapSnapshotDistance; }
			set {
				if (value != _miniMapSnapshotDistance) {
					_miniMapSnapshotDistance = value;
					isDirty = true;
				}
			}
		}


        [SerializeField]
        float _miniMapContrast = 1.02f;

        /// <summary>
        /// Contrast of the mini-map image
        /// </summary>
        public float miniMapContrast
        {
            get { return _miniMapContrast; }
            set
            {
                if (value != _miniMapContrast)
                {
                    _miniMapContrast = value;
                    miniMapMaterialRefresh = true;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        float _miniMapBrightness = 1.05f;

        /// <summary>
        /// Brightness of the mini-map image
        /// </summary>
        public float miniMapBrightness
        {
            get { return _miniMapBrightness; }
            set
            {
                if (value != _miniMapBrightness)
                {
                    _miniMapBrightness = value;
                    miniMapMaterialRefresh = true;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _miniMapEnableShadows = false;

        /// <summary>
        /// Enables/disables shadow casting when rendering mini-map
        /// </summary>
        public bool miniMapEnableShadows
        {
            get { return _miniMapEnableShadows; }
            set
            {
                if (value != _miniMapEnableShadows)
                {
                    _miniMapEnableShadows = value;
                    isDirty = true;
                }
            }
        }





        [SerializeField, Range(0,1)]
		float _miniMapZoomMin = 0.01f;

		/// <summary>
		/// The orthographic minimum size for the camera
		/// </summary>
		public float miniMapZoomMin {
			get { return _miniMapZoomMin; }
			set {
				if (value != _miniMapZoomMin) {
					_miniMapZoomMin = value;
					needUpdateBarContents = true;
					needMiniMapShot = true;
					isDirty = true;
				}
			}
		}

		[SerializeField, Range(0,1)]
		float _miniMapZoomMax = 1f;

		/// <summary>
		/// The orthographic maximum size for the camera
		/// </summary>
		public float miniMapZoomMax {
			get { return _miniMapZoomMax; }
			set {
				if (value != _miniMapZoomMax) {
					_miniMapZoomMax = value;
					needUpdateBarContents = true;
					needMiniMapShot = true;
					isDirty = true;
				}
			}
		}


		[SerializeField, Range (0, 1f)]
		float _miniMapZoomLevel = 0.5f;

		/// <summary>
		/// The current mini-map zoom based on the min/max size (orthographic mode) or altitude (perspective mode)
		/// </summary>
		public float miniMapZoomLevel {
			get { return _miniMapZoomLevel; }
			set {
				if (value != _miniMapZoomLevel) {
					_miniMapZoomLevel = Mathf.Clamp01 (value);
					needMiniMapShot = true;
					needUpdateBarContents = true;
					isDirty = true;
				}
			}
		}


		[SerializeField]
		float _miniMapCameraMinAltitude = 10;

		/// <summary>
		/// The min distance from the camera to the following target
		/// </summary>
		public float miniMapCameraMinAltitude {
			get { return _miniMapCameraMinAltitude; }
			set {
				if (value != _miniMapCameraMinAltitude) {
					_miniMapCameraMinAltitude = value;
					needUpdateBarContents = true;
					needMiniMapShot = true;
					isDirty = true;
				}
			}
		}


		[SerializeField]
		float _miniMapCameraMaxAltitude = 100f;

		/// <summary>
		/// The max distance from the camera to the following target
		/// </summary>
		public float miniMapCameraMaxAltitude {
			get { return _miniMapCameraMaxAltitude; }
			set {
				if (value != _miniMapCameraMaxAltitude) {
					_miniMapCameraMaxAltitude = value;
					needUpdateBarContents = true;
					needMiniMapShot = true;
					isDirty = true;
				}
			}
		}



        [SerializeField]
        float _miniMapCameraHeightVSFollow = 200f;

        /// <summary>
        /// When mini-map is in orthographic projection, an optional height for the camera with respect to the main camera or followed item
        /// </summary>
        public float miniMapCameraHeightVSFollow
        {
            get { return _miniMapCameraHeightVSFollow; }
            set
            {
                if (value != _miniMapCameraHeightVSFollow)
                {
                    _miniMapCameraHeightVSFollow = value;
                    needUpdateBarContents = true;
                    needMiniMapShot = true;
                    isDirty = true;
                }
            }
        }


        [SerializeField] int _miniMapLayerMask = -1;

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
					needUpdateBarContents = true;
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
					isDirty = true;
				}
			}
		}

		[SerializeField]
		bool _miniMapShowButtons;

		public bool miniMapShowButtons {
			get { return _miniMapShowButtons; }
			set {
				if (value != _miniMapShowButtons) {
					_miniMapShowButtons = value;
					isDirty = true;
					SetupMiniMap ();
				}
			}
		}


		public void MiniMapZoomIn (float speed = 1f) {
			miniMapZoomLevel += Time.deltaTime * speed;
		}

		public void MiniMapZoomOut (float speed = 1f) {
			miniMapZoomLevel -= Time.deltaTime * speed;
		}

		bool _miniMapZoomState;

		public bool miniMapZoomState {
			get {
				return _miniMapZoomState; 
			}
			set {
				if (_miniMapZoomState != value) {
					MiniMapZoomToggle (value);
				}
			}
		}


		public void UpdateMiniMapContents() {
			needMiniMapShot = true;
			needUpdateBarContents = true;
		}


		#endregion


		
	}

}




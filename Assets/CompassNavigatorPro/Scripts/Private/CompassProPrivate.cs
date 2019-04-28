using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CompassNavigatorPro {
	public delegate void POIEvent (CompassProPOI poi);

	[ExecuteInEditMode]
	public partial class CompassPro : MonoBehaviour {
		class CompassActiveIcon {
			public CompassProPOI poi;
			public Image image, miniMapImage;
			public float lastPosX;
			public string levelName;
			RectTransform _rectTransform, _miniMapRectTransform;

			public RectTransform rectTransform {
				get { return _rectTransform; }
				set {
					_rectTransform = value;
					image = _rectTransform.GetComponent<Image> ();
				}
			}

			public RectTransform miniMapRectTransform {
				get { return _miniMapRectTransform; }
				set {
					_miniMapRectTransform = value;
					miniMapImage = _miniMapRectTransform.GetComponent<Image> ();
				}
			}

			public CompassActiveIcon (CompassProPOI poi) {
				this.poi = poi;
				this.levelName = SceneManager.GetActiveScene ().name;
			}
		}

		enum CompassPoint {
			CardinalEast = 0,
			HalfWindEastNorthEast = 1,
			OrdinalNorthEast = 2,
			HalfWindNorthNorthEast = 3,
			CardinalNorth = 4,
			HalfWindNorthNorthWest = 5,
			OrdinalNorthWest = 6,
			HalfWindWestNorthWest = 7,
			CardinalWest = 8,
			HalfWindWestSouthWest = 9,
			OrdinalSouthWest = 10,
			HalfWindSouthSouthWest = 11,
			CardinalSouth = 12,
			HalfWindSouthSouthEast = 13,
			OrdinalSouthEast = 14,
			HalfWindEastSouthEast = 15,
		}

		int[] cardinals = new int[] { 0, 4, 8, 12 };
		int[] ordinals = new int[] { 2, 6, 10, 14 };
		int[] halfwinds = new int[] { 1, 3, 5, 7, 9, 11, 13, 15 };

		struct CompassPointPOI {
			public Vector3 position;
			public float cos, sin;
			public Text text;
			public RawImage image;
		}


		#region Internal fields

		const float COMPASS_POI_POSITION_THRESHOLD = 0.001f;
		public bool isDirty;
		static CompassPro _instance;
		List<CompassActiveIcon> icons;
		float fadeStartTime, prevAlpha;
		CanvasGroup canvasGroup;
		RectTransform compassBackRect;
		Image compassBackImage;
		GameObject compassIconPrefab, compassMiniMapClampedPrefab;
		Text text, textShadow;
		Text title, titleShadow;
		float endTimeOfCurrentTextReveal;
		Material spriteOverlayMat;
		Vector3 lastCamPos, lastCamRot;
		int lastFrameCount;
		float lastTime;
		StringBuilder titleText;
		AudioSource audioSource;
		int poiVisibleCount;
		bool autoHiding;
		// performing autohide fade
		float thisAlpha;
		bool needUpdateBarContents;
		string lastDistanceText;
		float lastDistanceSqr;
		CompassPointPOI[] compassPoints;
		float usedNorthDegrees;
        const int TEXT_POOL_SIZE = 256;
        LetterAnimator[] textPool;
        int poolIndex;
        Transform canvasTextPool;
        #endregion

		#region internal Minimap stuff

		const string SKW_COMPASS_FOG_OF_WAR = "COMPASS_FOG_OF_WAR";
		const string SKW_COMPASS_ROTATED = "COMPASS_ROTATED";
		Transform miniMapUIRoot;
		Transform miniMapUI, miniMapButtonsPanel;
		Camera miniMapCamera;
		Transform cameraCompass;
		RenderTexture miniMapTex;
		CanvasGroup miniMapCanvasGroup;
		Material miniMapOverlayMat;
		Vector2 miniMapAnchorMin, miniMapAnchorMax, miniMapPivot, miniMapSizeDelta;
		float miniMapCameraAspect;
		MINIMAP_STYLE miniMapCurrentStyle;
		float miniMapLastSnapshotTime;
		Vector3 miniMapLastSnapshotLocation;
		bool needMiniMapShot;
		Image miniMapImage;
        bool miniMapMaterialRefresh;
        float miniMapLastCameraRotation;
        Vector3 lastMiniMapCameraPos;

		#endregion

		#region Curved compass

		Material curvedMat, defaultUICurvedMat;

		#endregion

		#region Gameloop lifecycle

		#if UNITY_EDITOR
		
		[MenuItem ("GameObject/UI/Compass Navigator Pro", false)]
		static void CreateCompassNavigatorPro (MenuCommand menuCommand) {
			// Create a custom game object
			GameObject go = Instantiate (Resources.Load<GameObject> ("CNPro/Prefabs/CompassNavigatorPro")) as GameObject;
			go.name = "CompassNavigatorPro";
			GameObjectUtility.SetParentAndAlign (go, menuCommand.context as GameObject);
			Undo.RegisterCreatedObjectUndo (go, "Create " + go.name);
			Selection.activeObject = go;
		}

		[MenuItem ("GameObject/UI/Compass Navigator Pro", true)]
		static bool ValidateCreateCompassNavigatorPro (MenuCommand menuCommand) {
			return CompassPro.instance == null;
		}


		#endif
		public void OnEnable () {

			if (icons == null) {
				Init ();
			}

                // ensure there's an EventSystem gameobject is buttons are visible so they can be used
                // an EventSystem gameobject is automatically created when instantiating a Canvas prefab so here we go
                if (Application.isPlaying && _miniMapShowButtons && FindObjectOfType<EventSystem>() == null)
                {
                    GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem));
                    eventSystem.AddComponent<StandaloneInputModule>();
                }

            SetupTextPool();
            SetupMiniMap ();

			if (dontDestroyOnLoad && Application.isPlaying) {
				if (FindObjectsOfType (GetType ()).Length > 1) {
					Destroy (gameObject);
					return;
				}
				DontDestroyOnLoad (this);
				SceneManager.sceneLoaded += UpdateFogOfWarOnLoadScene;
			}
		}


        void OnValidate()
        {
            SetupMiniMap();
        }

		void OnDisable () {
			DisableMiniMap ();
			SceneManager.sceneLoaded -= UpdateFogOfWarOnLoadScene;
		}

		void Init () {
			#if UNITY_EDITOR
			#if UNITY_2018_3_OR_NEWER
			UnityEditor.PrefabInstanceStatus prefabInstanceStatus = UnityEditor.PrefabUtility.GetPrefabInstanceStatus(gameObject);
			if (prefabInstanceStatus != UnityEditor.PrefabInstanceStatus.NotAPrefab) {
			UnityEditor.PrefabUtility.UnpackPrefabInstance(gameObject, UnityEditor.PrefabUnpackMode.Completely, UnityEditor.InteractionMode.AutomatedAction);
			}
			#else
			UnityEditor.PrefabType prefabType = UnityEditor.PrefabUtility.GetPrefabType (gameObject);
			if (prefabType != UnityEditor.PrefabType.None && prefabType != UnityEditor.PrefabType.DisconnectedPrefabInstance && prefabType != UnityEditor.PrefabType.DisconnectedModelPrefabInstance) {
				UnityEditor.PrefabUtility.DisconnectPrefabInstance (gameObject);
			}
			#endif
			#endif


			icons = new List<CompassActiveIcon> (1000);
			audioSource = GetComponent<AudioSource> ();
			spriteOverlayMat = Resources.Load<Material> ("CNPro/Materials/SpriteOverlayUnlit");
			compassIconPrefab = Resources.Load<GameObject> ("CNPro/Prefabs/CompassIcon");
			compassMiniMapClampedPrefab = Resources.Load<GameObject> ("CNPro/Prefabs/CompassMiniMapClampedIcon");
			GameObject compassBack = transform.Find ("CompassBack").gameObject;
			compassBackRect = compassBack.GetComponent<RectTransform> ();
			compassBackImage = compassBack.GetComponent<Image> ();
			canvasGroup = GetCanvasGroup (compassBackRect);
			text = compassBackRect.transform.Find ("Text").GetComponent<Text> ();
			textShadow = compassBackRect.transform.Find ("TextShadow").GetComponent<Text> ();
			text.text = textShadow.text = "";
			title = compassBackRect.transform.Find ("Title").GetComponent<Text> ();
			titleShadow = compassBackRect.transform.Find ("TitleShadow").GetComponent<Text> ();
			title.text = titleShadow.text = "";
			canvasGroup.alpha = 0;
			prevAlpha = 0f;
			fadeStartTime = Time.time;
			lastDistanceText = "";
			lastDistanceSqr = float.MinValue;
			compassPoints = new CompassPointPOI[16];
			if (compassBackRect.transform.Find ("CardinalN") == null) {
				Debug.LogError ("CompassNavigatorPro gameobject has to be updated. Please delete and add prefab again to this scene.");
				_showCardinalPoints = false;
			} else {
				compassPoints [(int)CompassPoint.CardinalNorth].text = compassBackRect.transform.Find ("CardinalN").GetComponent<Text> ();
				compassPoints [(int)CompassPoint.CardinalWest].text = compassBackRect.transform.Find ("CardinalW").GetComponent<Text> ();
				compassPoints [(int)CompassPoint.CardinalSouth].text = compassBackRect.transform.Find ("CardinalS").GetComponent<Text> ();
				compassPoints [(int)CompassPoint.CardinalEast].text = compassBackRect.transform.Find ("CardinalE").GetComponent<Text> ();
			}
			if (compassBackRect.transform.Find ("InterCardinalNW") == null) {
				Debug.LogError ("CompassNavigatorPro gameobject has to be updated. Please delete and add prefab again to this scene.");
				_showOrdinalPoints = false;
			} else {
				compassPoints [(int)CompassPoint.OrdinalNorthWest].text = compassBackRect.transform.Find ("InterCardinalNW").GetComponent<Text> ();
				compassPoints [(int)CompassPoint.OrdinalNorthEast].text = compassBackRect.transform.Find ("InterCardinalNE").GetComponent<Text> ();
				compassPoints [(int)CompassPoint.OrdinalSouthWest].text = compassBackRect.transform.Find ("InterCardinalSW").GetComponent<Text> ();
				compassPoints [(int)CompassPoint.OrdinalSouthEast].text = compassBackRect.transform.Find ("InterCardinalSE").GetComponent<Text> ();
			}
			if (compassBackRect.transform.Find ("TickNNE") == null) {
				Debug.LogError ("CompassNavigatorPro gameobject has to be updated. Please delete and add prefab again to this scene.");
				_showHalfWinds = false;
			} else {
				compassPoints [(int)CompassPoint.HalfWindEastNorthEast].image = compassBackRect.transform.Find ("TickENE").GetComponent<RawImage> ();
				compassPoints [(int)CompassPoint.HalfWindEastSouthEast].image = compassBackRect.transform.Find ("TickESE").GetComponent<RawImage> ();
				compassPoints [(int)CompassPoint.HalfWindNorthNorthEast].image = compassBackRect.transform.Find ("TickNNE").GetComponent<RawImage> ();
				compassPoints [(int)CompassPoint.HalfWindNorthNorthWest].image = compassBackRect.transform.Find ("TickNNW").GetComponent<RawImage> ();
				compassPoints [(int)CompassPoint.HalfWindSouthSouthEast].image = compassBackRect.transform.Find ("TickSSE").GetComponent<RawImage> ();
				compassPoints [(int)CompassPoint.HalfWindSouthSouthWest].image = compassBackRect.transform.Find ("TickSSW").GetComponent<RawImage> ();
				compassPoints [(int)CompassPoint.HalfWindWestNorthWest].image = compassBackRect.transform.Find ("TickWNW").GetComponent<RawImage> ();
				compassPoints [(int)CompassPoint.HalfWindWestSouthWest].image = compassBackRect.transform.Find ("TickWSW").GetComponent<RawImage> ();
			}
			usedNorthDegrees = -1;
			ComputeCompassPointsPositions ();
			UpdateCompassBarAppearance ();
			UpdateHalfWindsAppearance ();
			UpdateCompassBarAlpha ();
			UpdateFogOfWarTexture ();
		}

		void LateUpdate () {
			UpdateCompassBarAlpha ();
			UpdateCompassBarContents ();
			UpdateFogOfWarPosition ();
			UpdateMiniMap ();
		}

		#endregion

		#region Internal stuff

		/// <summary>
		/// Update bar icons
		/// </summary>
		void UpdateCompassBarContents () {

			if (cameraMain == null) {
				if (_cameraMain == null)
					return;
			}

			// If camera has not moved, then don't refresh compass bar so often - just once each second in case one POI is moving
			switch (_updateInterval) {
			case UPDATE_INTERVAL.NumberOfFrames:
				if (Time.frameCount - lastFrameCount >= _updateIntervalFrameCount) {
					lastFrameCount = Time.frameCount;
					needUpdateBarContents = true;
				}
				break;
			case UPDATE_INTERVAL.Time:
				if (Time.time - lastTime >= _updateIntervalTime) {
					lastTime = Time.time;
					needUpdateBarContents = true;
				}
				break;
			case UPDATE_INTERVAL.Continuous:
				needUpdateBarContents = true;
				break;
			}
			if (!needUpdateBarContents) {
				if (lastCamPos != _cameraMain.transform.position || lastCamRot != _cameraMain.transform.eulerAngles) {
					needUpdateBarContents = true;
				}
			}
			if (!needUpdateBarContents)
				return;

			needUpdateBarContents = false;
			lastCamPos = _cameraMain.transform.position;
			lastCamRot = _cameraMain.transform.eulerAngles;

			float visibleDistanceSQR = _visibleDistance * _visibleDistance;
			float visitedDistanceSQR = _visitedDistance * _visitedDistance;
			float nearDistanceSQR = _nearDistance * _nearDistance;
			float barMax = _width * 0.5f - _endCapsWidth / _cameraMain.pixelWidth;
			const float visibleDistanceFallOffSQR = 25f * 25f;

			// Cardinal & Ordinal (ordinal) Points
			ComputeCompassPointsPositions ();
			UpdateCardinalPoints (barMax);
			UpdateOrdinalPoints (barMax);
			UpdateHalfWinds (barMax);

			// Update Icons
			poiVisibleCount = 0;

			float nearestPOIDistanceThisFrame = float.MaxValue;
			CompassProPOI nearestPOI = null;
			bool miniMapIsActive = _showMiniMap && miniMapUI != null;

			for (int p = 0; p < icons.Count; p++) {
				bool iconVisible = false;
				CompassActiveIcon activeIcon = icons [p];
				CompassProPOI poi = activeIcon.poi;
				if (poi == null) {
					if (activeIcon.rectTransform != null && activeIcon.rectTransform.gameObject != null) {
						DestroyImmediate (activeIcon.rectTransform.gameObject);
					}
					if (activeIcon.miniMapRectTransform != null && activeIcon.miniMapRectTransform.gameObject != null) {
						DestroyImmediate (activeIcon.miniMapRectTransform.gameObject);
					}
					icons.RemoveAt (p);	// POI no longer registered; remove and exit to prevent indexing errors
					continue;
				}

				// Change in visibility?
				Vector3 poiPosition = poi.transform.position;
				float distanceSqr, distancePlanarSQR;
				distanceSqr = (poiPosition - lastCamPos).sqrMagnitude;
				distanceSqr -= poi.radius;
				if (distanceSqr <= 0)
					distanceSqr = 0.01f;
				poi.distanceToCameraSQR = distanceSqr;

				// Distance has changed, proceed with update...
				if (_use3Ddistance) {
					distancePlanarSQR = distanceSqr;
				} else {
					Vector2 v = new Vector2 (poiPosition.x - lastCamPos.x, poiPosition.z - lastCamPos.z);
					distancePlanarSQR = v.sqrMagnitude;
				}
				float distanceFactor = distancePlanarSQR / nearDistanceSQR;
				float alphaFactor = poi.visibility == POI_VISIBILITY.AlwaysVisible ? 1f : distanceFactor;
				if (poi.showPlayModeGizmo) {
					poi.iconAlpha = Mathf.Lerp (0.65f, 0, 5f * alphaFactor);
				}
				poi.iconScale = Misc.Vector3one * Mathf.Lerp (_maxIconSize, _minIconSize, distanceFactor);
				poi.miniMapIconScale = Misc.Vector3one * _miniMapIconSize;

				// Should we make this POI visible in the compass bar?
				float thisPOIVisibleDistanceSQR = poi.visibleDistanceOverride > 0 ? poi.visibleDistanceOverride * poi.visibleDistanceOverride : visibleDistanceSQR;
				bool isInRange = poi.distanceToCameraSQR < thisPOIVisibleDistanceSQR;
				bool prevVisible = poi.isVisible;
				poi.isVisible = ((isInRange && poi.visibility == POI_VISIBILITY.WhenInRange) || poi.visibility == POI_VISIBILITY.AlwaysVisible) && poi.isActiveAndEnabled;

				// Is it same scene?
				if (poi.isVisible && poi.dontDestroyOnLoad && !activeIcon.levelName.Equals (SceneManager.GetActiveScene ().name)) {
					poi.isVisible = false;
				}

				// Do we visit this POI for the first time?
				if (poi.isVisible && !poi.isVisited && poi.canBeVisited && poi.distanceToCameraSQR < visitedDistanceSQR) {
					poi.isVisited = true;
					if (OnPOIVisited != null)
						OnPOIVisited (poi);
					if (audioSource != null) {
						if (poi.visitedAudioClip != null) {
							audioSource.PlayOneShot (poi.visitedAudioClip);
						} else if (_visitedDefaultAudioClip != null) {
							audioSource.PlayOneShot (_visitedDefaultAudioClip);
						}
					}
					ShowPOIDiscoveredText (poi);
					if (poi.hideWhenVisited)
						poi.enabled = false;
				}

				// Check heartbeat
				if (poi.heartbeatEnabled) {
					bool inHeartbeatRange = poi.distanceToCameraSQR < poi.heartbeatDistance * poi.heartbeatDistance;
					if (!poi.heartbeatIsActive && inHeartbeatRange) {
						poi.StartHeartbeat ();
					} else if (poi.heartbeatIsActive && !inHeartbeatRange) {
						poi.StopHeartbeat ();
					}
				}

				// Notify POI visibility change
				if (prevVisible != poi.isVisible) {
					if (poi.isVisible && OnPOIVisible != null) {
						OnPOIVisible (poi);
					} else if (!poi.isVisible && OnPOIHide != null) {
						OnPOIHide (poi);
					}
				}

				// If POI is not visible, then hide and skip
				if (!poi.isVisible) {
					if (activeIcon.image != null && activeIcon.image.enabled) {
						activeIcon.image.enabled = false;
					}
					if (poi.spriteRenderer != null && poi.spriteRenderer.enabled) {
						poi.spriteRenderer.enabled = false;
					}
				} else {
					
					// POI is visible, should we create the icon in the compass bar?
					if (activeIcon.rectTransform == null) {
						GameObject iconGO = Instantiate (compassIconPrefab);
						iconGO.hideFlags = HideFlags.DontSave;
						iconGO.transform.SetParent (compassBackRect.transform, false);
						activeIcon.rectTransform = iconGO.GetComponent<RectTransform> ();
					}

					// Check bending
					if (_bendAmount == 0 && activeIcon.image.material != null) {
						activeIcon.image.material = null;
					} else if (_bendAmount != 0 && activeIcon.image.material != curvedMat) {
						activeIcon.image.material = curvedMat;
					}

					// Position the icon on the compass bar
					Vector3 screenPos = GetScreenPos (poiPosition);
					float posX;
					if (Mathf.Abs (screenPos.x - activeIcon.lastPosX) > COMPASS_POI_POSITION_THRESHOLD) {
						needUpdateBarContents = true;
						posX = (screenPos.x + activeIcon.lastPosX) * 0.5f;
					} else {
						posX = screenPos.x;
					}
					activeIcon.lastPosX = posX;

					// Always show the focused icon in the compass bar; if out of bar, maintain it on the edge with normal scale
					if (poi.clampPosition) {
						if (screenPos.z < 0) {
							posX = barMax * -Mathf.Sign (screenPos.x - 0.5f);
							if (poi.iconScale.x > 1f)
								poi.iconScale = Misc.Vector3one;
						} else {
							posX = Mathf.Clamp (posX, -barMax, barMax);
							if (poi.iconScale.x > 1f)
								poi.iconScale = Misc.Vector3one;
						}
						screenPos.z = 0;
					}
					float absPosX = Mathf.Abs (posX);

					// Set icon position
					if (absPosX > barMax || screenPos.z < 0) {
						// Icon outside of bar
						if (activeIcon.image != null && activeIcon.image.enabled) {
							activeIcon.image.enabled = false;
						}
					} else {
						// Unhide icon
						if (activeIcon.image != null && !activeIcon.image.enabled) {
							activeIcon.image.enabled = true;
							activeIcon.poi.visibleTime = Time.time;
						}
						activeIcon.rectTransform.anchorMin = activeIcon.rectTransform.anchorMax = new Vector2 (0.5f + posX / _width, 0.5f);
						iconVisible = true;
					}

					// Icon is visible, manage it
					if (iconVisible) {
						poiVisibleCount++;

						// Check gizmo
						if (!poi.showPlayModeGizmo && poi.spriteRenderer != null && poi.spriteRenderer.enabled) {
							poi.spriteRenderer.enabled = false;
						} else if (poi.showPlayModeGizmo) {
							if (poi.spriteRenderer == null) {
								// Add a dummy child gameObject
								GameObject go = new GameObject ("POI Gizmo Renderer");
								go.transform.SetParent (poi.transform, false);
								poi.spriteRenderer = go.gameObject.AddComponent<SpriteRenderer> ();
								poi.spriteRenderer.material = spriteOverlayMat;
								poi.spriteRenderer.enabled = false;
							}
							if (poi.spriteRenderer != null) {
								if (!poi.spriteRenderer.enabled) {
									poi.spriteRenderer.enabled = true;
									poi.spriteRenderer.sprite = poi.iconVisited;
								}
								poi.spriteRenderer.transform.LookAt (lastCamPos);
								poi.spriteRenderer.transform.localScale = Misc.Vector3one * _gizmoScale;
								poi.spriteRenderer.color = new Color (1, 1, 1, poi.iconAlpha);
							}
						}

						// Assign proper icon
						if (activeIcon.poi.isVisited) {
							if (activeIcon.image != poi.iconVisited) {
								activeIcon.image.sprite = poi.iconVisited;
							}
						} else if (activeIcon.image != poi.iconNonVisited) {
							activeIcon.image.sprite = poi.iconNonVisited;
						}

						// Scale in animation
						if (_scaleInDuration > 0) {
							float t = (Time.time - activeIcon.poi.visibleTime) / _scaleInDuration;
							if (t < 1) {
								needUpdateBarContents = true;
								activeIcon.poi.iconScale *= t;
							}
						}

						// Scale icon
						if (activeIcon.poi.iconScale != activeIcon.rectTransform.localScale) {
							activeIcon.rectTransform.localScale = activeIcon.poi.iconScale;
						}

                        // Set icon's color and alpha
                        Color spriteColor = poi.tintColor;
						if (visibleDistanceFallOffSQR > 0) {
							if (poi.visibility == POI_VISIBILITY.AlwaysVisible) {
								spriteColor.a = 1f;
							} else {
								float t = (visibleDistanceSQR - poi.distanceToCameraSQR) / visibleDistanceFallOffSQR;
								spriteColor.a = Mathf.Lerp (0, 1, t);
							}
						}
                        activeIcon.image.color = spriteColor;

						// Get title if POI is centered
						if (absPosX < _labelHotZone && distancePlanarSQR < nearestPOIDistanceThisFrame) {
							nearestPOI = poi;
							nearestPOIDistanceThisFrame = distancePlanarSQR;
						}
					}
				}

				// mini-map icon
				if (miniMapIsActive && (poi.miniMapVisibility == POI_VISIBILITY.AlwaysVisible || (poi.miniMapVisibility == POI_VISIBILITY.WhenInRange && isInRange))) {
					iconVisible = false;

					// POI is visible, should we create the icon in the minimap?
					if (activeIcon.miniMapRectTransform == null) {
						GameObject iconGO = Instantiate (poi.miniMapClampPosition ? compassMiniMapClampedPrefab : compassIconPrefab);
						iconGO.hideFlags = HideFlags.DontSave;
						iconGO.transform.SetParent (miniMapUI.transform, false);
						activeIcon.miniMapRectTransform = iconGO.GetComponent<RectTransform> ();
					}

					// Position the icon on the mini-map area
					Vector2 miniMapScreenPos = GetMiniMapScreenPos (poiPosition);

					// Always show the focused icon in the compass bar; if out of bar, maintain it on the edge with normal scale
					if (poi.miniMapClampPosition) {
						miniMapScreenPos.x = Mathf.Clamp (miniMapScreenPos.x, _miniMapClampBorder, 1f - _miniMapClampBorder);
						miniMapScreenPos.y = Mathf.Clamp (miniMapScreenPos.y, _miniMapClampBorder, 1f - _miniMapClampBorder);
					}

					// Set icon position
					if (miniMapScreenPos.x > 1 || miniMapScreenPos.x < 0 || miniMapScreenPos.y > 1 || miniMapScreenPos.y < 0) {
						// Icon outside of bar
						if (activeIcon.miniMapImage != null && activeIcon.miniMapImage.enabled) {
							activeIcon.miniMapImage.enabled = false;
						}
					} else {
						// Unhide icon
						if (activeIcon.miniMapImage != null && !activeIcon.miniMapImage.enabled) {
							activeIcon.miniMapImage.enabled = true;
						}
						activeIcon.miniMapRectTransform.anchorMin = activeIcon.miniMapRectTransform.anchorMax = miniMapScreenPos;
						iconVisible = true;
					}

					// Icon is visible, manage it
					if (iconVisible) {

						// Assign proper icon
						if (activeIcon.poi.isVisited) {
							if (activeIcon.miniMapImage != poi.iconVisited) {
								activeIcon.miniMapImage.sprite = poi.iconVisited;
							}
						} else if (activeIcon.miniMapImage != poi.iconNonVisited) {
							activeIcon.miniMapImage.sprite = poi.iconNonVisited;
						}


						// tint color
						activeIcon.miniMapImage.color = poi.tintColor;

						// Scale icon
						if (activeIcon.poi.miniMapIconScale != activeIcon.miniMapRectTransform.localScale) {
							activeIcon.miniMapRectTransform.localScale = activeIcon.poi.miniMapIconScale;
						}
					}

					// Send events
					if (activeIcon.poi.miniMapIsVisible && !iconVisible) {
						if (OnPOIVisibleInMiniMap != null) {
							OnPOIVisibleInMiniMap (activeIcon.poi);
						}
					} else if (iconVisible && !activeIcon.poi.miniMapIsVisible) {
						if (OnPOIHidesInMiniMap != null) {
							OnPOIHidesInMiniMap (activeIcon.poi);
						}
					}
					activeIcon.poi.miniMapIsVisible = iconVisible;
				} else {
					if (activeIcon.miniMapImage != null && activeIcon.miniMapImage.enabled) {
						activeIcon.miniMapImage.enabled = false;
					}
				}

			}

			// Update title
			if (nearestPOI != null) {
				if (titleText == null) {
					titleText = new StringBuilder ();
				} else {
					if (titleText.Length > 0)
						titleText.Length = 0;
				}
				if (nearestPOI.isVisited || nearestPOI.titleVisibility == TITLE_VISIBILITY.Always) {
					titleText.Append (nearestPOI.title);
				}
				// indicate "above" or "below"
				bool addedAlt = false; 
				if (nearestPOI.transform.position.y > lastCamPos.y + _sameAltitudeThreshold) {
					if (titleText.Length > 0)
						titleText.Append (" ");
					titleText.Append ("(Above");
					addedAlt = true;
				} else if (nearestPOI.transform.position.y < lastCamPos.y - _sameAltitudeThreshold) {
					if (titleText.Length > 0)
						titleText.Append (" ");
					titleText.Append ("(Below");
					addedAlt = true;
				}
				if (_showDistance) {
					if (addedAlt) {
						titleText.Append (", ");
					} else {
						if (titleText.Length > 0)
							titleText.Append (" ");
						titleText.Append ("(");
					}
					if (lastDistanceSqr != nearestPOIDistanceThisFrame) {
						lastDistanceSqr = nearestPOIDistanceThisFrame;
						lastDistanceText = Mathf.Sqrt (nearestPOIDistanceThisFrame).ToString (showDistanceFormat);
					}
					titleText.Append (lastDistanceText);
					titleText.Append (" m)");
				} else if (addedAlt) {
					titleText.Append (")");
				}

				string tt = titleText.ToString ();
				if (!title.text.Equals (tt)) {
					title.text = titleShadow.text = tt;
					UpdateTitleAlpha (1.0f);
					UpdateTitleAppearance ();
				}
			} else {
				title.text = titleShadow.text = "";
			}


		}

		Vector3 GetScreenPos (Vector3 poiPosition) {
			Vector3 camPos = _cameraMain.transform.position;
			poiPosition.y = camPos.y;
			Vector3 screenPos = Misc.Vector3zero;
			Quaternion oldRot = _cameraMain.transform.rotation;
			Vector3 angles = _cameraMain.transform.eulerAngles;
			_cameraMain.transform.eulerAngles = new Vector3 (0, angles.y, 0);

			switch (_worldMappingMode) {
			case WORLD_MAPPING_MODE.LimitedToBarWidth: 
				screenPos = _cameraMain.WorldToViewportPoint (poiPosition);
				break;
			case WORLD_MAPPING_MODE.Full180Degrees:
				{
					Vector3 v2poi = poiPosition - camPos;
					Vector3 dir = new Vector3 (_cameraMain.transform.forward.x, 0, _cameraMain.transform.forward.z);
					float angle = (Quaternion.FromToRotation (dir, v2poi).eulerAngles.y + 180f) / 180f;
					screenPos.x = 0.5f + (angle % 2.0f - 1.0f) * (_width - _endCapsWidth / _cameraMain.pixelWidth) * 0.9f;
				}
				break;
			case WORLD_MAPPING_MODE.Full360Degrees: 
				{
					Vector3 v2poi = poiPosition - camPos;
					Vector3 dir = new Vector3 (_cameraMain.transform.forward.x, 0, _cameraMain.transform.forward.z);
					float angle = (Quaternion.FromToRotation (dir, v2poi).eulerAngles.y + 180f) / 180f;
					screenPos.x = 0.5f + (angle % 2.0f - 1f) * 0.5f * (_width - _endCapsWidth / _cameraMain.pixelWidth) * 0.9f;
				}
				break;
			default: // WORLD_MAPPING_MODE.CameraFustrum: 
				screenPos = _cameraMain.WorldToViewportPoint (poiPosition);
				screenPos.x = 0.5f + (screenPos.x - 0.5f) * (_width - _endCapsWidth / _cameraMain.pixelWidth) * 0.9f;
				break;
			}
			screenPos.x -= 0.5f;

			_cameraMain.transform.rotation = oldRot;
			return screenPos;
		}

		Vector2 GetMiniMapScreenPos (Vector3 poiPosition) {
			Vector2 viewportPos = miniMapCamera.WorldToViewportPoint (poiPosition);
			viewportPos.x = (viewportPos.x - 0.5f) / _miniMapZoomLevel + 0.5f;
			viewportPos.y = (viewportPos.y - 0.5f) / _miniMapZoomLevel + 0.5f;
			return viewportPos;
		}

		void ComputeCompassPointsPositions () {

			if (_cameraMain == null)
				return;

			if (_northDegrees != usedNorthDegrees) {
				usedNorthDegrees = _northDegrees;
				for (int k = 0; k < 16; k++) {
					float angle = (Mathf.PI * 2f * k / 16f) + _northDegrees * Mathf.Deg2Rad;
					compassPoints [k].cos = Mathf.Cos (angle);
					compassPoints [k].sin = Mathf.Sin (angle);
				}
			}
			Vector3 camPos = _cameraMain.transform.position;
			Vector3 pos;
			for (int k = 0; k < 16; k++) {
				pos = camPos;
				pos.x += compassPoints [k].cos;
				pos.z += compassPoints [k].sin;
				compassPoints [k].position = pos;
			}
		}

		/// <summary>
		/// If showCardinalPoints is enabled, show N, W, S, E across the compass bar
		/// </summary>
		void UpdateCardinalPoints (float barMax) {

			for (int i = 0; i < cardinals.Length; i++) {
				int k = cardinals [i];
				if (!_showCardinalPoints) {
					if (compassPoints [k].text.enabled) {
						compassPoints [k].text.enabled = false;
					}
					continue;
				}
				Vector3 screenPos = GetScreenPos (compassPoints [k].position);
				float posX = screenPos.x;
				float absPosX = Mathf.Abs (posX);

				// Set icon position
				if (absPosX > barMax || screenPos.z <= 0) {
					// Icon outside of bar
					if (compassPoints [k].text.enabled) {
						compassPoints [k].text.enabled = false;
					}
				} else {
					// Unhide icon
					if (!compassPoints [k].text.enabled) {
						compassPoints [k].text.enabled = true;
					}
					RectTransform rt = compassPoints [k].text.rectTransform;
					rt.anchorMin = rt.anchorMax = new Vector2 (0.5f + posX / _width, 0.5f + _cardinalPointsVerticalOffset / rt.sizeDelta.y);
				}
			}

		}


		/// <summary>
		/// If showOrdinalPoints is enabled, show NE, NW, SW, SE across the compass bar
		/// </summary>
		void UpdateOrdinalPoints (float barMax) {
			
			for (int i = 0; i < ordinals.Length; i++) {
				int k = ordinals [i];
				if (!_showOrdinalPoints) {
					if (compassPoints [k].text != null && compassPoints [k].text.enabled) {
						compassPoints [k].text.enabled = false;
					}
					continue;
				}
				Vector3 screenPos = GetScreenPos (compassPoints [k].position);
				float posX = screenPos.x; 
				float absPosX = Mathf.Abs (posX);
				
				// Set icon position
				if (absPosX > barMax || screenPos.z <= 0) {
					// Icon outside of bar
					if (compassPoints [k].text.enabled) {
						compassPoints [k].text.enabled = false;
					}
				} else {
					// Unhide icon
					if (!compassPoints [k].text.enabled) {
						compassPoints [k].text.enabled = true;
					}
					RectTransform rt = compassPoints [k].text.rectTransform;
					rt.anchorMin = rt.anchorMax = new Vector2 (0.5f + posX / _width, 0.5f + _cardinalPointsVerticalOffset / rt.sizeDelta.y);
				}
			}
			
		}

		/// <summary>
		/// Manages compass bar alpha transitions
		/// </summary>
		void UpdateCompassBarAlpha () {

			// Alpha
			if (_alwaysVisibleInEditMode && !Application.isPlaying) {
				thisAlpha = 1.0f;
			} else if (_autoHide) {
				if (!autoHiding) {
					if (poiVisibleCount == 0) {
						if (thisAlpha > 0) {
							autoHiding = true;
							fadeStartTime = Time.time;
							prevAlpha = canvasGroup.alpha;
							thisAlpha = 0;
						}
					} else if (poiVisibleCount > 0 && thisAlpha == 0) {
						thisAlpha = _alpha;
						autoHiding = true;
						fadeStartTime = Time.time;
						prevAlpha = canvasGroup.alpha;
					}
				}
			} else {
				thisAlpha = _alpha;
			}

			if (thisAlpha != canvasGroup.alpha) {
				float t = Application.isPlaying ? (Time.time - fadeStartTime) / _fadeDuration : 1.0f;
				canvasGroup.alpha = Mathf.Lerp (prevAlpha, thisAlpha, t);
				if (t >= 1) {
					prevAlpha = canvasGroup.alpha;
				}
			} else if (autoHiding)
				autoHiding = false;
		}

		void UpdateCompassBarAppearance () {
			// Width & Vertical Position
			float anchorMinX = (1 - _width) * 0.5f;
			compassBackRect.anchorMin = new Vector2 (anchorMinX, _verticalPosition);
			float anchorMaxX = 1f - anchorMinX;
			compassBackRect.anchorMax = new Vector2 (anchorMaxX, _verticalPosition);

			// Style
			Sprite barSprite;
			switch (_style) {
			case COMPASS_STYLE.Rounded:
				barSprite = Resources.Load<Sprite> ("CNPro/Sprites/Bar2");
				break;
			case COMPASS_STYLE.Celtic_White:
				barSprite = Resources.Load<Sprite> ("CNPro/Sprites/Bar3-White");
				break;
			case COMPASS_STYLE.Celtic_Black:
				barSprite = Resources.Load<Sprite> ("CNPro/Sprites/Bar3-Black");
				break;
			default:
				barSprite = Resources.Load<Sprite> ("CNPro/Sprites/Bar1");
				break;
			}
			if (compassBackImage.sprite != barSprite) {
				compassBackImage.sprite = barSprite;
			}

			ToggleCurvedCompass ();

		}


		/// <summary>
		/// If showHalfWinds is enabled, show NNE, ENE, ESE, SSE, SSW, WSW, WNW, NNW marks
		/// </summary>
		void UpdateHalfWinds (float barMax) {
			for (int i = 0; i < halfwinds.Length; i++) {
				int k = halfwinds [i];
				if (!_showHalfWinds) {
					if (compassPoints [k].image.enabled) {
						compassPoints [k].image.enabled = false;
					}
					continue;
				}
				Vector3 screenPos = GetScreenPos (compassPoints [k].position);
				float posX = screenPos.x;
				float absPosX = Mathf.Abs (posX);

				// Set icon position
				if (absPosX > barMax || screenPos.z <= 0) {
					// Icon outside of bar
					if (compassPoints [k].image.enabled) {
						compassPoints [k].image.enabled = false;
					}
				} else {
					// Unhide icon
					if (!compassPoints [k].image.enabled) {
						compassPoints [k].image.enabled = true;
					}
					compassPoints [k].image.rectTransform.anchorMin = new Vector2 (0.5f + posX / _width, 0f);
					compassPoints [k].image.rectTransform.anchorMax = new Vector2 (0.5f + posX / _width, 1f);
				}
			}

		}

		void UpdateHalfWindsAppearance () {
			for (int i = 0; i < halfwinds.Length; i++) {
				int k = halfwinds [i];
				RawImage image = compassPoints [k].image;
				if (image != null) {
					if (!_showHalfWinds) {
						image.enabled = _showHalfWinds;
					}
					image.color = _halfWindsTintColor;
					image.transform.localScale = new Vector3 (_halfWindsWidth, _halfWindsHeight, 1f);
				}
			}
		}

		void UpdateTextAppearanceEditMode () {
			if (!gameObject.activeInHierarchy)
				return;
			text.text = textShadow.text = "SAMPLE TEXT";
			UpdateTextAlpha (1);
			UpdateTextAppearance ();
		}

		void UpdateTextAppearance () {
            // Vertical and horizontal position
            text.alignment = TextAnchor.MiddleCenter;
			Vector3 localScale = new Vector3 (_textScale, _textScale, 1f);
			RectTransform rt = text.GetComponent<RectTransform> ();
			rt.pivot = new Vector2 (0.5f, 0.5f);
			rt.anchoredPosition3D = new Vector3 (0, _textVerticalPosition, 0);
			text.transform.localScale = localScale;
			text.font = _textFont;

			textShadow.enabled = _textShadowEnabled;
			textShadow.alignment = TextAnchor.MiddleCenter;
			rt = textShadow.GetComponent<RectTransform> ();
			rt.pivot = new Vector2 (0.5f, 0.5f);
			rt.anchoredPosition3D = new Vector3 (1f, _textVerticalPosition - 1f, 0);
			textShadow.transform.localScale = localScale;
			textShadow.font = _textFont;
		}

		void UpdateTextAlpha (float t) {
			text.color = new Color (text.color.r, text.color.g, text.color.b, t);
			textShadow.color = new Color (0, 0, 0, t);
		}

		void UpdateTitleAppearanceEditMode () {
			if (!gameObject.activeInHierarchy)
				return;
			title.text = titleShadow.text = "SAMPLE TITLE";
			UpdateTitleAlpha (1);
			UpdateTitleAppearance ();
		}

		void UpdateTitleAppearance () {
			// Vertical and horizontal position
			title.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (0, _titleVerticalPosition, 0);
			Vector3 localScale = new Vector3 (_titleScale, _titleScale, 1f);
			title.transform.localScale = localScale;
			title.font = _titleFont;
			titleShadow.enabled = _titleShadowEnabled;
			titleShadow.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (1f, _titleVerticalPosition - 1f, 0);
			titleShadow.transform.localScale = localScale;
			titleShadow.font = _titleFont;
		}

		void UpdateTitleAlpha (float t) {
			title.color = new Color (title.color.r, title.color.g, title.color.b, t);
			titleShadow.color = new Color (0, 0, 0, t);
		}

        void SetupTextPool()
        {
            if (!Application.isPlaying) return;

            text.text = textShadow.text = "";
            UpdateTextAppearance();
            if (textPool == null || textPool.Length != TEXT_POOL_SIZE)
            {
                textPool = new LetterAnimator[TEXT_POOL_SIZE];
            }

            GameObject o = GameObject.Find("CompassProTextPool");
            if (o==null)
            {
                o = new GameObject("CompassProTextPool");
            }
            canvasTextPool = o.transform;

            for (int k=0;k<textPool.Length;k++)
            {
                GameObject letterShadow = Instantiate(textShadow.gameObject);
                letterShadow.transform.SetParent(canvasTextPool);
                letterShadow.name = "TextShadowPool";
                Text lts = letterShadow.GetComponent<Text>();

                GameObject letter = Instantiate (text.gameObject);
                letter.transform.SetParent(canvasTextPool);
                letter.name = "TextPool";
                Text lt = letter.GetComponent<Text>();

                LetterAnimator animator = lts.gameObject.AddComponent<LetterAnimator>();
                animator.poolIndex = k;
                animator.text = lt;
                animator.textShadow = lts;
                animator.OnAnimationEnds += PushTextToPool;
                animator.used = false;
                textPool[k] = animator;
            }
        }

        void FetchTextFromPool(out Text lt, out Text lts)
        {
            for (int k=0;k< TEXT_POOL_SIZE; k++)
            {
                ++poolIndex;
                if (poolIndex >= TEXT_POOL_SIZE)
                {
                    poolIndex = 0;
                }
                if (!textPool[poolIndex].used)
                {
                    break;
                }
            }
            // Setup shadow (first, so it goes behind white text)
            lts = textPool[poolIndex].textShadow;
            lts.transform.SetParent(text.transform.parent, false);
            lt = textPool[poolIndex].text;
            lt.transform.SetParent(text.transform.parent, false);
            textPool[poolIndex].used = true;
        }

        void PushTextToPool(int index)
        {
            textPool[index].text.transform.SetParent(canvasTextPool);
            textPool[index].textShadow.transform.SetParent(canvasTextPool);
            textPool[index].used = false;
        }


        void ShowPOIDiscoveredText (CompassProPOI poi) {
			if (poi.visitedText == null || !_textRevealEnabled)
				return;
			StartCoroutine (AnimateDiscoverText (poi.visitedText));
		}

		IEnumerator AnimateDiscoverText (string discoverText) {

			int len = discoverText.Length;
			if (len == 0 || _cameraMain == null || textPool == null || textPool.Length != TEXT_POOL_SIZE)
				yield break;

			while (Time.time < endTimeOfCurrentTextReveal) {
				yield return new WaitForSeconds (1);
			}

			float now = Time.time;
			endTimeOfCurrentTextReveal = now + _textRevealDuration + _textDuration + _textFadeOutDuration * 0.5f;

            text.text = textShadow.text = "";
            UpdateTextAppearance();

			// initial pos of text
			string discoverTextSpread = discoverText.Replace (" ", "A");
			float posX = -text.cachedTextGenerator.GetPreferredWidth (discoverTextSpread, text.GetGenerationSettings (Misc.Vector2zero)) * 0.5f * _textScale;

			float acum = 0;
            TextGenerationSettings settings = new TextGenerationSettings();
            for (int k = 0; k < len; k++) {
                Text lts, lt;
                string ch = discoverText.Substring (k, 1);
                FetchTextFromPool(out lt, out lts);
                lts.text = ch;
                lt.text = ch;

                float letw = 0;
                if (k==0)
                {
                    settings = lt.GetGenerationSettings(Misc.Vector2max);
                }
				if (ch.Equals (" ")) {
					letw = lt.cachedTextGenerator.GetPreferredWidth ("A", settings) * _textScale;
				} else {
					letw = lt.cachedTextGenerator.GetPreferredWidth (ch, settings) * _textScale;
				}

				RectTransform letterRT = lt.GetComponent<RectTransform> ();
				letterRT.anchoredPosition3D = new Vector3 (posX + acum + letw * 0.5f, letterRT.anchoredPosition3D.y, 0);
				RectTransform shadowRT = lts.GetComponent<RectTransform> ();
				shadowRT.anchoredPosition3D = new Vector3 (posX + acum + letw * 0.5f + 1f, shadowRT.anchoredPosition3D.y, 0);

				acum += letw;

                // Trigger animator
                LetterAnimator anim = textPool[poolIndex];
				anim.startTime = now + k * _textRevealLetterDelay;
				anim.revealDuration = _textRevealDuration;
				anim.startFadeTime = now + _textRevealDuration + _textDuration;
				anim.fadeDuration = _textFadeOutDuration;
                anim.enabled = true;
                anim.Play();
			}
		}

		#endregion


		#region MiniMap

		void SetupMiniMap () {
			if (miniMapFollow == null && _cameraMain != null) {
				miniMapFollow = _cameraMain.transform;
			}

			miniMapUIRoot = transform.Find ("MiniMap Root");
			if (_showMiniMap) {
				miniMapUI = transform.Find ("MiniMap");
				// if there exists an old minimap at root or the minimap root does not exist at root, remove anything and recreate
				bool rebuildMiniMap = false;
				if (miniMapUI != null || miniMapUIRoot == null) {
					rebuildMiniMap = true;
				} 
				// check buttons
				if (miniMapUIRoot != null) {
					miniMapButtonsPanel = miniMapUIRoot.Find ("Buttons");
					if (miniMapButtonsPanel == null) {
						rebuildMiniMap = true;
					}
				}

				if (rebuildMiniMap) {
					if (miniMapUI != null) {
						DestroyImmediate (miniMapUI.gameObject);
					}
					if (miniMapUIRoot != null) {
						DestroyImmediate (miniMapUIRoot.gameObject);
					}
					miniMapUIRoot = null;
				}
				if (miniMapUIRoot == null) {
					// rebuild from prefab
					GameObject pb = Resources.Load<GameObject> ("CNPro/Prefabs/CompassNavigatorPro");
					Transform pt = pb.transform.Find ("MiniMap Root");
					GameObject minimapRootChild = null;
					if (pt != null) {
						minimapRootChild = pt.gameObject;
					}
					if (minimapRootChild != null) {
						GameObject minimapRootGO = Instantiate<GameObject> (minimapRootChild, transform);
						minimapRootGO.name = "MiniMap Root";
						if (minimapRootGO != null) {
							miniMapUIRoot = minimapRootGO.transform;
						}
					}
					miniMapButtonsPanel = miniMapUIRoot.Find ("Buttons");
				}

				// check buttons

				ToggleButtonEventHandler ("ZoomIn", () => {
					MiniMapZoomIn ();
				}, true);
				ToggleButtonEventHandler ("ZoomOut", () => {
					MiniMapZoomOut ();
				}, true);
				ToggleButtonEventHandler ("ToggleFull", () => {
					miniMapZoomState = !miniMapZoomState;
				}, false);

				miniMapUIRoot = transform.Find ("MiniMap Root");
				miniMapUI = miniMapUIRoot.Find ("MiniMap");
				if (miniMapCamera == null) {
					miniMapCamera = transform.GetComponentInChildren<Camera> (true);
				}
				if (miniMapCamera != null) {
					miniMapCamera.enabled = false;
				}
				if (miniMapCamera != null && miniMapUIRoot != null) {
					RectTransform miniMapUIRootRT = miniMapUIRoot.GetComponent<RectTransform> ();

					// set mini-map position
					if (_miniMapLocation != MINIMAP_LOCATION.Custom) {
						switch (_miniMapLocation) {
						case MINIMAP_LOCATION.TopLeft:
							miniMapUIRootRT.anchorMin = new Vector2 (0, 1);
							miniMapUIRootRT.anchorMax = new Vector2 (0, 1);
							miniMapUIRootRT.pivot = new Vector2 (0, 1);
							break;
						case MINIMAP_LOCATION.TopCenter:
							miniMapUIRootRT.anchorMin = new Vector2 (0.5f, 1f);
							miniMapUIRootRT.anchorMax = new Vector2 (0.5f, 1);
							miniMapUIRootRT.pivot = new Vector2 (0.5f, 1);
							break;
						case MINIMAP_LOCATION.TopRight:
							miniMapUIRootRT.anchorMin = new Vector2 (1, 1);
							miniMapUIRootRT.anchorMax = new Vector2 (1, 1);
							miniMapUIRootRT.pivot = new Vector2 (1, 1);
							break;
						case MINIMAP_LOCATION.MiddleLeft:
							miniMapUIRootRT.anchorMin = new Vector2 (0, 0.5f);
							miniMapUIRootRT.anchorMax = new Vector2 (0, 0.5f);
							miniMapUIRootRT.pivot = new Vector2 (0, 0.5f);
							break;
						case MINIMAP_LOCATION.MiddleCenter:
							miniMapUIRootRT.anchorMin = new Vector2 (0.5f, 0.5f);
							miniMapUIRootRT.anchorMax = new Vector2 (0.5f, 0.5f);
							miniMapUIRootRT.pivot = new Vector2 (0.5f, 0.5f);
							break;
						case MINIMAP_LOCATION.MiddleRight:
							miniMapUIRootRT.anchorMin = new Vector2 (1, 0.5f);
							miniMapUIRootRT.anchorMax = new Vector2 (1, 0.5f);
							miniMapUIRootRT.pivot = new Vector2 (1, 0.5f);
							break;
						case MINIMAP_LOCATION.BottomLeft:
							miniMapUIRootRT.anchorMin = new Vector2 (0, 0);
							miniMapUIRootRT.anchorMax = new Vector2 (0, 0);
							miniMapUIRootRT.pivot = new Vector2 (0, 0);
							break;
						case MINIMAP_LOCATION.BottomCenter:
							miniMapUIRootRT.anchorMin = new Vector2 (0.5f, 0);
							miniMapUIRootRT.anchorMax = new Vector2 (0.5f, 0);
							miniMapUIRootRT.pivot = new Vector2 (0.5f, 0);
							break;
						case MINIMAP_LOCATION.BottomRight:
							miniMapUIRootRT.anchorMin = new Vector2 (1, 0);
							miniMapUIRootRT.anchorMax = new Vector2 (1, 0);
							miniMapUIRootRT.pivot = new Vector2 (1, 0);
							break;
						}

						if (_miniMapLocation != MINIMAP_LOCATION.Custom) {
							miniMapUIRootRT.anchoredPosition = _miniMapLocationOffset;
						}
					}

					// set mini-map size
					float screenSize = _cameraMain != null ? _cameraMain.pixelHeight * _miniMapSize : Screen.height * _miniMapSize;
					miniMapUIRootRT.sizeDelta = new Vector2 (screenSize, screenSize);

					miniMapUIRoot.gameObject.SetActive (true);

					// set minimap viewer properties
					switch (_miniMapStyle) {
					case MINIMAP_STYLE.TornPaper:
						_miniMapBorderTexture = Resources.Load<Texture2D> ("CNPro/Textures/MiniMapBorder");
						_miniMapMaskSprite = Resources.Load<Sprite> ("CNPro/Sprites/MiniMapMask");
						break;
					case MINIMAP_STYLE.SolidBox:
						_miniMapBorderTexture = Resources.Load<Texture2D> ("CNPro/Textures/MiniMapBorderSolidBox");
						_miniMapMaskSprite = null;
						break;
					case MINIMAP_STYLE.SolidCircle:
						_miniMapBorderTexture = Resources.Load<Texture2D> ("CNPro/Textures/MiniMapBorderSolidCircle");
						_miniMapMaskSprite = Resources.Load<Sprite> ("CNPro/Sprites/MiniMapMaskSolidCircle");
						break;
					default:
						break;
					}

					if (miniMapOverlayMat == null) {
						miniMapOverlayMat = Instantiate<Material> (Resources.Load<Material> ("CNPro/Materials/MiniMapOverlayUnlit"));
					}

					if (_miniMapCameraMode == MINIMAP_CAMERA_MODE.Perspective) {
						miniMapCameraSnapshotFrequency = MINIMAP_CAMERA_SNAPSHOT_FREQUENCY.Continuous;
					}

					if (miniMapUI != null) {
						miniMapImage = miniMapUI.GetComponent<Image> ();
						if (miniMapImage != null) {
							miniMapImage.sprite = _miniMapMaskSprite;
							miniMapImage.material = miniMapOverlayMat;
							Material mat = miniMapImage.materialForRendering;
							mat.SetTexture ("_BorderTex", _miniMapBorderTexture);
							if (_fogOfWarEnabled && _miniMapCameraMode == MINIMAP_CAMERA_MODE.Orthographic) {
								mat.EnableKeyword (SKW_COMPASS_FOG_OF_WAR);
							} else {
								mat.DisableKeyword (SKW_COMPASS_FOG_OF_WAR);
							}
							if (_miniMapKeepStraight) {
								mat.DisableKeyword (SKW_COMPASS_ROTATED);
							} else {
								mat.EnableKeyword (SKW_COMPASS_ROTATED);
							}
						}
					}

					// setup render texture
					MiniMapResizeRenderTexture (_miniMapResolutionNormalSize, _miniMapResolutionNormalSize);
					#if UNITY_5_6_OR_NEWER
					miniMapCamera.allowHDR = false;
					miniMapCamera.allowMSAA = false;
					#endif
					miniMapCamera.clearFlags = CameraClearFlags.SolidColor;
					miniMapCamera.backgroundColor = Color.black;
					miniMapCamera.orthographic = (_miniMapCameraMode == MINIMAP_CAMERA_MODE.Orthographic);
					miniMapCamera.cullingMask = _miniMapLayerMask;

					cameraCompass = miniMapUIRoot.Find ("CameraCompass");
				} else {
					Debug.LogError ("Mini Map prefab element could not be intialized.");
					_showMiniMap = false;
				}
				if (miniMapCanvasGroup == null) {
					miniMapCanvasGroup = GetCanvasGroup (miniMapUIRoot);
				}
				miniMapCanvasGroup.interactable = miniMapCanvasGroup.blocksRaycasts = _miniMapShowButtons;
			} else {
				if (miniMapUIRoot != null) {
					miniMapUIRoot.gameObject.SetActive (false);
				}
			}
			needUpdateBarContents = true;
			needMiniMapShot = true;
		}

		void MiniMapResizeRenderTexture (int width, int height) {
			if (miniMapCamera == null || miniMapCamera == null)
				return;

			if (miniMapTex == null || miniMapTex.width != width || miniMapTex.height != height) {
				if (miniMapTex != null) {
					miniMapTex.Release ();
				}
				miniMapTex = new RenderTexture (width, height, 24, RenderTextureFormat.ARGB32);
			}
			miniMapCamera.targetTexture = miniMapTex;
			if (miniMapImage != null) {
				miniMapImage.materialForRendering.SetTexture ("_MiniMapTex", miniMapTex);
			}
		}

		void DisableMiniMap () {
			if (miniMapUIRoot != null && miniMapUIRoot.gameObject.activeSelf) {
				miniMapUIRoot.gameObject.SetActive (false);
			}
			if (miniMapCamera != null) {
				miniMapCamera.enabled = false;
			}
			if (miniMapTex != null) {
				miniMapTex.Release ();
			}
		}

		void UpdateMiniMap () {
			if (!_showMiniMap || miniMapCamera == null || miniMapFollow == null)
				return;

			Vector3 followPos = miniMapFollow.position;
			if (_miniMapCameraMode == MINIMAP_CAMERA_MODE.Orthographic) {
				miniMapCamera.transform.position = new Vector3 (followPos.x, followPos.y + _miniMapCameraHeightVSFollow + 0.1f, followPos.z);
				if (_miniMapZoomMin < 0.001f) {
					_miniMapZoomMin = 0.001f;
				}
				if (_miniMapZoomMax < _miniMapZoomMin) {
					_miniMapZoomMax = _miniMapZoomMin;
				}
				_miniMapZoomLevel = Mathf.Clamp (_miniMapZoomLevel, _miniMapZoomMin, _miniMapZoomMax);
				miniMapCamera.orthographicSize = _miniMapCaptureSize * 0.5f;
			} else {
				float altitude = _miniMapCameraMinAltitude + (_miniMapCameraMaxAltitude - _miniMapCameraMinAltitude) * _miniMapZoomLevel;
				if (_miniMapCameraMaxAltitude < _miniMapCameraMinAltitude) {
					_miniMapCameraMaxAltitude = _miniMapCameraMinAltitude;
				}
				miniMapCamera.transform.position = new Vector3 (followPos.x, followPos.y + altitude, followPos.z);
			}
            if (miniMapCanvasGroup.alpha != _miniMapAlpha)
            {
                miniMapCanvasGroup.alpha = _miniMapAlpha;
            }

			// snapshot control
			switch (_miniMapCameraSnapshotFrequency) {
			case MINIMAP_CAMERA_SNAPSHOT_FREQUENCY.TimeInterval:
				if (Time.time - miniMapLastSnapshotTime > _miniMapSnapshotInterval) {
					needMiniMapShot = true;
				}
				break;
			case MINIMAP_CAMERA_SNAPSHOT_FREQUENCY.DistanceTravelled:
				if ((miniMapLastSnapshotLocation - miniMapCamera.transform.position).sqrMagnitude > _miniMapSnapshotDistance * _miniMapSnapshotDistance) {
					needMiniMapShot = true;
				}
				break;
			case MINIMAP_CAMERA_SNAPSHOT_FREQUENCY.Continuous:
				needMiniMapShot = true;
				break;
			}

            // minimapp camera rotation
			float rotation = 0;
			if (_miniMapKeepStraight) {
                miniMapCamera.transform.eulerAngles = new Vector3(90f, 0, 0);
				if (cameraCompass != null) {
					Vector3 angles = miniMapFollow.rotation.eulerAngles;
					angles.z = 180f - angles.y;
					angles.x = angles.y = 0;
					cameraCompass.eulerAngles = angles;
				}
			} else {
				Vector3 forward = miniMapFollow.forward;
				forward.y = 0;
				miniMapCamera.transform.LookAt (followPos, forward);
				rotation = miniMapFollow.rotation.eulerAngles.y * Mathf.Deg2Rad;
			}
            if (miniMapLastCameraRotation != rotation)
            {
                miniMapLastCameraRotation = rotation;
                miniMapMaterialRefresh = true;
            }

            // capture map
            if (needMiniMapShot)
            {
                needMiniMapShot = false;
				Quaternion oldRot = miniMapCamera.transform.rotation;
				miniMapCamera.transform.eulerAngles = new Vector3(90f, 0, 0);
                if (!_miniMapEnableShadows)
                {
                    ShadowQuality sq = QualitySettings.shadows;
                    QualitySettings.shadows = ShadowQuality.Disable;
                    miniMapCamera.Render();
                    QualitySettings.shadows = sq;
                } else
                {
                    miniMapCamera.Render();
                }
                miniMapLastSnapshotTime = Time.time;
                miniMapLastSnapshotLocation = miniMapCamera.transform.position;
				miniMapCamera.transform.rotation = oldRot;
                miniMapMaterialRefresh = true;
            }

            // Set mini-map shader properties

            // mini-map uv
            Vector3 miniMapCameraPos = miniMapCamera.transform.position;
            if (miniMapCameraPos != lastMiniMapCameraPos)
            {
                miniMapMaterialRefresh = true;
                lastMiniMapCameraPos = miniMapCameraPos;
            }
            Vector3 uvOffset = miniMapLastSnapshotLocation - miniMapCameraPos;
            float camSize = miniMapCamera.orthographicSize * 2f;
            uvOffset.x = uvOffset.x / camSize;
            uvOffset.y = uvOffset.z / camSize;
            uvOffset.z = _miniMapZoomLevel;

            // fog of war shader properties
            Vector4 uvFogOffset = _fogOfWarCenter - miniMapCameraPos;
            uvFogOffset.x = (uvFogOffset.x / camSize) / _miniMapZoomLevel;
            uvFogOffset.y = (uvFogOffset.z / camSize) / _miniMapZoomLevel;
            uvFogOffset.z = _miniMapZoomLevel * camSize / _fogOfWarSize.x;
            uvFogOffset.w = _miniMapZoomLevel * camSize / _fogOfWarSize.z;

            // update material
            if (miniMapMaterialRefresh && miniMapImage != null)
            {
                miniMapMaterialRefresh = false;
                Material mat = miniMapImage.materialForRendering;
                mat.SetFloat("_Rotation", rotation);
                mat.SetVector("_UVOffset", uvOffset);
                mat.SetVector("_UVFogOffset", uvFogOffset);
                mat.SetTexture("_FogOfWarTex", fogOfWarTexture);
                mat.SetColor("_FogOfWarTintColor", _fogOfWarColor);
                mat.SetVector("_Effects", new Vector3(_miniMapBrightness, _miniMapContrast, 0));
            }
        }

		CanvasGroup GetCanvasGroup (Transform transform) {
			if (transform == null) {
				return null;
			}
			CanvasGroup canvasGroup = transform.GetComponent<CanvasGroup> ();
			if (canvasGroup == null) {
				canvasGroup = transform.gameObject.AddComponent<CanvasGroup> ();
				canvasGroup.blocksRaycasts = false;
			}
			return canvasGroup;

		}

		void ToggleButtonEventHandler (string buttonName, UnityAction handler, bool continuous) {
			if (miniMapButtonsPanel == null)
				return;
			Transform t = miniMapButtonsPanel.Find (buttonName);
			if (t == null)
				return;
			if (!_miniMapShowButtons) {
				t.gameObject.SetActive (false);
				return;
			}
			t.gameObject.SetActive (true);
			Button button = t.GetComponent<Button> ();
			if (button == null)
				return;

			if (continuous) {
				CompassButtonHandler buttonHandler = button.GetComponent<CompassButtonHandler> ();
				if (buttonHandler == null) {
					buttonHandler = button.gameObject.AddComponent<CompassButtonHandler> ();
				}
				buttonHandler.actionHandler = handler;
			} else {
				button.onClick.RemoveListener (handler);
				button.onClick.AddListener (handler);
			}
		}

		void MiniMapZoomToggle (bool state) {

			if (miniMapUIRoot == null || cameraMain == null)
				return;

			RectTransform rt = miniMapUIRoot.GetComponent<RectTransform> ();

			_miniMapZoomState = state;

			if (state) {
				miniMapAnchorMin = rt.anchorMin;
				miniMapAnchorMax = rt.anchorMax;
				miniMapPivot = rt.pivot;
				miniMapSizeDelta = rt.sizeDelta;
				miniMapCurrentStyle = _miniMapStyle;
				miniMapCameraAspect = miniMapCamera.aspect;
				if (_miniMapStyle != MINIMAP_STYLE.SolidBox) {
					miniMapStyle = MINIMAP_STYLE.SolidBox;
				}
				float padding = (1f - _miniMapFullScreenSize) * 0.5f;
				float minX, minY, maxX, maxY;
				minY = padding;
				maxY = 1f - padding;
				int height = (int)(cameraMain.pixelHeight * _miniMapFullScreenSize);
				int width = _miniMapKeepAspectRatio ? height : (int)(cameraMain.pixelWidth * _miniMapFullScreenSize);
				if (_miniMapKeepAspectRatio) {
					float paddingW = (1f - _miniMapFullScreenSize / cameraMain.aspect) * 0.5f;
					minX = paddingW;
					maxX = 1f - paddingW;
				} else {
					minX = minY;
					maxX = maxY;
					miniMapCamera.aspect = (float)width / height;
				}
				rt.anchorMin = new Vector3 (minX, minY);
				rt.anchorMax = new Vector3 (maxX, maxY);
				rt.pivot = new Vector2 (0.5f, 0.5f);
				rt.anchoredPosition = Vector2.zero;
				rt.sizeDelta = new Vector2 (0, 0);
				MiniMapResizeRenderTexture (width, height);	
				if (_miniMapFullScreenSize >= 1f && _miniMapDisableMainCameraInFullScreen && !_miniMapKeepAspectRatio) {
					_cameraMain.enabled = false;
				}
			} else {
				rt.anchorMin = miniMapAnchorMin;
				rt.anchorMax = miniMapAnchorMax;
				rt.pivot = miniMapPivot;
				rt.sizeDelta = miniMapSizeDelta;
				if (_miniMapStyle != miniMapCurrentStyle) {
					miniMapStyle = miniMapCurrentStyle;
				}
				miniMapCamera.aspect = miniMapCameraAspect;
				MiniMapResizeRenderTexture (_miniMapResolutionNormalSize, _miniMapResolutionNormalSize);
				if (_miniMapFullScreenSize >= 1f && _miniMapDisableMainCameraInFullScreen && !_miniMapKeepAspectRatio) {
					_cameraMain.enabled = true;
				}

			}
		}

		#endregion

		#region Curved compass

		void ToggleCurvedCompass () {
			if (compassBackRect == null) {
				return;
			}

			CompassBarMeshModifier m = compassBackImage.GetComponent<CompassBarMeshModifier> ();

			if (_bendAmount == 0) {
				if (m != null) {
					DestroyImmediate (m);
				}
				Image[] ii = compassBackRect.GetComponentsInChildren<Image> (true);
				for (int k = 0; k < ii.Length; k++) {
					ii [k].material = null;
				}
				// Adjust texts
				Text[] tt = compassBackRect.GetComponentsInChildren<Text> (true);
				for (int k = 0; k < tt.Length; k++) {
					tt [k].material = null;
				}

				// Adjust ticks
				RawImage[] im = compassBackRect.GetComponentsInChildren<RawImage> (true);
				for (int k = 0; k < im.Length; k++) {
					im [k].material = null;
				}
			} else {
				if (curvedMat == null) {
					curvedMat = Resources.Load<Material> ("CNPro/Materials/SpriteCurved");
				}
				if (defaultUICurvedMat == null) {
					defaultUICurvedMat = Resources.Load<Material> ("CNPro/Materials/UIDefaultCurved");
				}

				curvedMat.SetFloat ("_BendFactor", _bendAmount);
				defaultUICurvedMat.SetFloat ("_BendFactor", _bendAmount);

				if (m == null) {
					compassBackImage.gameObject.AddComponent<CompassBarMeshModifier> ();
				}

				// Images
				Image[] ii = compassBackRect.GetComponentsInChildren<Image> (true);
				for (int k = 0; k < ii.Length; k++) {
					ii [k].material = curvedMat;
				}

				// Adjust texts
				Text[] tt = compassBackRect.GetComponentsInChildren<Text> (true);
				for (int k = 0; k < tt.Length; k++) {
					tt [k].material = defaultUICurvedMat;
				}

				// Adjust ticks
				RawImage[] im = compassBackRect.GetComponentsInChildren<RawImage> (true);
				for (int k = 0; k < im.Length; k++) {
					im [k].material = defaultUICurvedMat;
				}
			}

		}

		#endregion

	
	}



}




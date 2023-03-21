using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.EditorTools;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Slider = UnityEngine.UIElements.Slider;

namespace UnityEditor.TerrainTools.UI
{
    [Overlay(typeof(SceneView), "Brush Attributes", defaultDockPosition = DockPosition.Top, defaultDockZone = DockZone.TopToolbar)]
    [Icon("Packages/com.unity.terrain-tools/Editor/Icons/TerrainOverlays/BrushSettingIcons/BrushAttributes.png")]
    internal class BrushAttributesOverlay : ToolbarOverlay, ITransientOverlay
    {
        public bool visible
        {
            get
            {
                var currTool = BrushesOverlay.ActiveTerrainTool as TerrainPaintToolWithOverlaysBase;
                if (currTool == null)
                    return false;
                bool isTerrainToolPaintTool = currTool is ITerrainToolPaintTool;
                return currTool.HasBrushAttributes && BrushesOverlay.IsSelectedObjectTerrain() && isTerrainToolPaintTool;
            }
        }

        internal static BrushAttributesOverlay instance;
        internal Dictionary<int, CondensedSlider> m_Attributes = new Dictionary<int, CondensedSlider>();
        public static Layout activeToolbarLayout => instance.activeLayout;

        public static string s_UssPath => "Styles/BrushPopup";
        public static int popUpWidth = 200;  
        public static int popUpHeight = 50; 

        BrushAttributesOverlay() : base(
            BrushOpacity.id, 
            BrushSize.id,
            BrushRotation.id,
            BrushSpacing.id,
            BrushScattering.id)
        {
            if (instance != null)
            {
                instance.m_Attributes.Clear();
                instance = null;
            }
            instance = this;
        }
        
        internal static void RegisterAttribute(CondensedSlider attr)
        {
            if (instance != null)
                instance.m_Attributes.TryAdd(attr.contentLabel.GetHashCode(), attr);
        }
        
        internal static void RebuildContent()
        {
            if (instance != null)
            {
                foreach (var attr in instance.m_Attributes)
                {
                    attr.Value.RebuildContent();
                }
            }
        }

        public static void AddBorder(EditorWindow window)
        {
            Color color = EditorGUIUtility.isProSkin
                ? new Color(0.44f, 0.44f, 0.44f, 1f)
                : new Color(0.51f, 0.51f, 0.51f);
            window.rootVisualElement.style.borderLeftWidth = (StyleFloat) 1f;
            window.rootVisualElement.style.borderTopWidth = (StyleFloat) 1f;
            window.rootVisualElement.style.borderRightWidth = (StyleFloat) 1f;
            window.rootVisualElement.style.borderBottomWidth = (StyleFloat) 1f;
            window.rootVisualElement.style.borderLeftColor = (StyleColor) color;
            window.rootVisualElement.style.borderTopColor = (StyleColor) color;
            window.rootVisualElement.style.borderRightColor = (StyleColor) color;
            window.rootVisualElement.style.borderBottomColor = (StyleColor) color;
        }

        public static TerrainPaintToolWithOverlaysBase GetActiveOverlaysTool()
        {
            var tool = BrushesOverlay.ActiveTerrainTool as TerrainPaintToolWithOverlaysBase;
            if (!tool) return null;
            if (!Selection.activeGameObject) return null; 
            if (!tool.Terrain)
            {
                if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<Terrain>() == null) return null; // check when loading and unloading packages
                tool.Terrain = Selection.activeGameObject.GetComponent<Terrain>();
            }
            if (!tool.Terrain)
            {
                Debug.LogError("Tool does NOT have associated terrain");
                return null; 
            }

            return tool; 
        }

        public static IBrushUIGroup GetCommonUI()
        {
            // get commonUI 
            var tool = GetActiveOverlaysTool();
            if (!tool) return null;
            
            Type type = tool.GetType();
            MethodInfo func = type.GetMethod("get_m_commonUI");
            if (func == null) return null; // avoid null errors 
            IBrushUIGroup commonUI = (IBrushUIGroup) func.Invoke(tool, null);
            return commonUI; 
        }
    }
    
    // brush attributes (opacity, size, rotation, spacing, scattering) -------
    // BRUSH OPACITY 
    [EditorToolbarElement(id, typeof(SceneView))]
    internal class BrushOpacity : CondensedSliderDropdown
    {
        internal const string id = "Brushes/OpacityPackage";
        private const string label = "Opacity";
        
        private static float minValue
        {
            get
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return 0f;
                return commonUI.brushStrengthMin;
            }
        }
        
        private static float maxValue
        {
            get
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return 1f;
                return commonUI.brushStrengthMax;
            }
        }

        static Texture2D s_OpacityIcon;

        static Texture2D Texture
        {
            get
            {
                if(s_OpacityIcon == null)
                    s_OpacityIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.unity.terrain-tools/Editor/Icons/TerrainOverlays/BrushSettingIcons/Opacity.png", typeof(Texture2D));
                return s_OpacityIcon;
            }
        }
        
        public void UpdateOverlayDirection(Layout l)
        {
            DropdownUpdateDirection(BrushAttributesOverlay.activeToolbarLayout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal, null, minValue, maxValue); 
            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };
            clicked += () => OpenPopup(CreatePopUp(), BrushAttributesOverlay.popUpWidth, BrushAttributesOverlay.popUpHeight);
            UpdateValues(); 
        }
        
        public void UpdateOverlayDirection(bool collapsedChanged)
        {
            DropdownUpdateDirection(BrushAttributesOverlay.activeToolbarLayout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal, null, minValue, maxValue);
            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };
            clicked += () => OpenPopup(CreatePopUp(), BrushAttributesOverlay.popUpWidth, BrushAttributesOverlay.popUpHeight);
            UpdateValues(); 
        }

        // On first install of the package, need to rebuild the attribute UI after
        // assets have been loaded. So this function implements a reconstruction path, essentially all the same
        // steps as if you step through all the constructors
        internal override void RebuildContent()
        {
            RebuildContent(Texture, minValue, maxValue);
            ConstructDropdown(null, BrushAttributesOverlay.instance.layout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal);
            UpdateOverlayDirection(true);
            SetContentWidth();
            UpdateValues();
        }

        public BrushOpacity()
        : base(label, Texture, minValue,  maxValue, null, BrushAttributesOverlay.instance.layout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal)
        {
            BrushAttributesOverlay.RegisterAttribute(this);

            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };
            
            void UpdateMin()
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                SetLowValueWithoutNotify(commonUI.brushStrengthMin);
            }

            void UpdateMax()
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                SetHighValueWithoutNotify(commonUI.brushStrengthMax);
            }

            UpdateOverlayDirection(true);

            RegisterCallback<AttachToPanelEvent>(e =>
            {
                ToolManager.activeToolChanged += UpdateValues;
                ToolManager.activeContextChanged += UpdateValues;
                BrushAttributesOverlay.instance.layoutChanged += UpdateOverlayDirection; 
                BrushAttributesOverlay.instance.collapsedChanged += UpdateOverlayDirection;
                BrushStrengthVariator.BrushStrengthChanged += UpdateValues;
                BrushStrengthVariator.BrushStrengthMinChanged += UpdateMin;
                BrushStrengthVariator.BrushStrengthMaxChanged += UpdateMax;
            });

            this.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushStrength = e.newValue;
            });
            
            RegisterCallback<DetachFromPanelEvent>(e =>
            {
                ToolManager.activeToolChanged -= UpdateValues;
                ToolManager.activeContextChanged -= UpdateValues;
                BrushAttributesOverlay.instance.layoutChanged -= UpdateOverlayDirection; 
                BrushAttributesOverlay.instance.collapsedChanged -= UpdateOverlayDirection;
                BrushStrengthVariator.BrushStrengthChanged -= UpdateValues;
                BrushStrengthVariator.BrushStrengthMinChanged -= UpdateMin;
                BrushStrengthVariator.BrushStrengthMaxChanged -= UpdateMax;
            });

            SetContentWidth();

            clicked += () => OpenPopup(CreatePopUp(), BrushAttributesOverlay.popUpWidth, BrushAttributesOverlay.popUpHeight);

            UpdateValues();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetContentWidth()
        {
            if (direction == SliderDirection.Horizontal)
                contentWidth = 100;
        }

        private void UpdateValues()
        {
            var commonUI = BrushAttributesOverlay.GetCommonUI();
            if (commonUI == null) return; 
            style.display = commonUI.hasBrushStrength ? DisplayStyle.Flex : DisplayStyle.None;
            if (!commonUI.hasBrushStrength) return; 
            value = commonUI.brushStrengthVal;
        }

        private VisualElement CreatePopUp()
        {
            var opacityContainer = new VisualElement();
            StyleSheet styleSheet = (StyleSheet)AssetDatabase.LoadAssetAtPath("Packages/com.unity.terrain-tools/Editor/Style/BrushPopup.uss", typeof(StyleSheet));
            if (styleSheet) opacityContainer.styleSheets.Add(styleSheet);

            // add min/max slider
            var opacityMinMaxContainer = new VisualElement();
            opacityMinMaxContainer.style.flexDirection = FlexDirection.Row;
            opacityContainer.Add(opacityMinMaxContainer);
            var opacityMinText = new TextElement();
            opacityMinText.text = "Min"; 
            opacityMinMaxContainer.Add(opacityMinText);
            var opacityMinField = new FloatField();
            opacityMinMaxContainer.Add(opacityMinField);
            
            var opacityMaxText = new TextElement();
            opacityMaxText.text = "Max"; 
            opacityMinMaxContainer.Add(opacityMaxText);
            var opacityMaxField = new FloatField();
            opacityMinMaxContainer.Add(opacityMaxField);

            // add jitter slider 
            var opacityJitterContainer = new VisualElement();
            opacityJitterContainer.style.flexDirection = FlexDirection.Row;
            opacityContainer.Add(opacityJitterContainer);
            var jitter = new Slider("Jitter", 0f, 1f);
            jitter.style.flexGrow = 1; 
            opacityJitterContainer.Add(jitter);
            var opacityJitterTextField = new FloatField();
            opacityJitterContainer.Add(opacityJitterTextField);

            opacityMinField.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushStrengthMin = e.newValue;
                if (commonUI.brushStrengthMin > commonUI.brushStrengthMax)
                {
                    commonUI.brushStrengthMax = e.newValue;
                    opacityMaxField.value = e.newValue;
                    SetHighValueWithoutNotify(e.newValue);
                }
                SetLowValueWithoutNotify(e.newValue);
            });
            
            opacityMaxField.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushStrengthMax = e.newValue; 
                if (commonUI.brushStrengthMax < commonUI.brushStrengthMin)
                {
                    commonUI.brushStrengthMin = e.newValue;
                    opacityMinField.value = e.newValue; 
                    SetLowValueWithoutNotify(e.newValue);
                }
                SetHighValueWithoutNotify(e.newValue);
            });
            
            jitter.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushStrengthJitter = e.newValue; 
                opacityJitterTextField.SetValueWithoutNotify(e.newValue);
            });
            
            opacityJitterTextField.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushStrengthJitter = e.newValue;
                jitter.SetValueWithoutNotify(e.newValue);
            });
            
            var commonUI = BrushAttributesOverlay.GetCommonUI();
            if (commonUI == null) Debug.LogWarning("cannot display STRENGTH in BrushAttributesOverlay.CS in PACKAGE");
            
            // NOTE: when I set values, I am setting them to commonUI.brushStrengthVal rather than commonUI.brushStrength
            // this is because commonUI.brushStrengthVal gives the raw brush strength, where as commonUI.brushStrength uses
            // a getter which calculates the jitter, which I don't want to display in the UI 
            // this is also true for size (commonUI.brushSizeVal) and rotation (commonUI.brushRotationVal) 
            
            opacityMinField.value = commonUI.brushStrengthMin;
            opacityMaxField.value = commonUI.brushStrengthMax; 
            jitter.value = commonUI.brushStrengthJitter;
            opacityJitterTextField.value = commonUI.brushStrengthJitter; 
            return opacityContainer;
        }

        private void OpenPopup(VisualElement popup, float width, float height)
        {
            var window = ScriptableObject.CreateInstance<EditorWindow>();
            BrushAttributesOverlay.AddBorder(window);
            window.rootVisualElement.Add(popup);
            window.ShowAsDropDown(GUIUtility.GUIToScreenRect(worldBound), new Vector2(width, height));
        }
    }
    
    // BRUSH SIZE 
    [EditorToolbarElement(id, typeof(SceneView))]
    internal class BrushSize : CondensedSliderDropdown
    {
        internal const string id = "Brushes/SizePackage";
        private const string label = "Size";

        private static float minValue
        {
            get
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return 0f;
                return commonUI.brushSizeMin;
            }
        }
        
        private static float maxValue
        {
            get
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return 500f;
                return commonUI.brushSizeMax;
            }
        }

        static Texture2D s_SizeIcon;            

        static Texture2D Texture
        {
            get
            {
                if(s_SizeIcon == null)
                    s_SizeIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.unity.terrain-tools/Editor/Icons/TerrainOverlays/BrushSettingIcons/Size.png", typeof(Texture2D));
                return s_SizeIcon;
            }
        }
        
        public void UpdateOverlayDirection(Layout l)
        {
            DropdownUpdateDirection(BrushAttributesOverlay.activeToolbarLayout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal, null, minValue, maxValue); 
            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };
            clicked += () => OpenPopup(CreatePopUp(), BrushAttributesOverlay.popUpWidth, BrushAttributesOverlay.popUpHeight);
            UpdateValues(); 
        }
        
        public void UpdateOverlayDirection(bool collapsedChanged)
        {
            DropdownUpdateDirection(BrushAttributesOverlay.activeToolbarLayout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal, null, minValue, maxValue);
            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };
            clicked += () => OpenPopup(CreatePopUp(), BrushAttributesOverlay.popUpWidth, BrushAttributesOverlay.popUpHeight);
            UpdateValues(); 
        }

        // On first install of the package, need to rebuild the attribute UI after
        // assets have been loaded. So this function implements a reconstruction path, essentially all the same
        // steps as if you step through all the constructors
        internal override void RebuildContent()
        {
            RebuildContent(Texture, minValue, maxValue);
            ConstructDropdown(null, BrushAttributesOverlay.instance.layout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal);
            UpdateOverlayDirection(true);
            SetContentWidth();
            UpdateValues();
        }

        public BrushSize()
        : base(label, Texture, minValue, maxValue, null, BrushAttributesOverlay.instance.layout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal)
        {
            BrushAttributesOverlay.RegisterAttribute(this);

            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };
            
            void UpdateMin()
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                SetLowValueWithoutNotify(commonUI.brushSizeMin);
            }

            void UpdateMax()
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                SetHighValueWithoutNotify(commonUI.brushSizeMax);
            }
            
            UpdateOverlayDirection(true);
            
            RegisterCallback<AttachToPanelEvent>(e =>
            {
                ToolManager.activeToolChanged += UpdateValues;
                ToolManager.activeContextChanged += UpdateValues;
                BrushAttributesOverlay.instance.layoutChanged += UpdateOverlayDirection; 
                BrushAttributesOverlay.instance.collapsedChanged += UpdateOverlayDirection;
                BrushSizeVariator.BrushSizeChanged += UpdateValues;
                BrushSizeVariator.BrushSizeMinChanged += UpdateMin;
                BrushSizeVariator.BrushSizeMaxChanged += UpdateMax; 
            });

            this.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushSize = e.newValue;
            });
            
            RegisterCallback<DetachFromPanelEvent>(e =>
            {
                ToolManager.activeToolChanged -= UpdateValues;
                ToolManager.activeContextChanged -= UpdateValues;
                BrushAttributesOverlay.instance.layoutChanged -= UpdateOverlayDirection; 
                BrushAttributesOverlay.instance.collapsedChanged -= UpdateOverlayDirection;
                BrushSizeVariator.BrushSizeChanged -= UpdateValues;
                BrushSizeVariator.BrushSizeMinChanged -= UpdateMin;
                BrushSizeVariator.BrushSizeMaxChanged -= UpdateMax; 
            });

            SetContentWidth();

            clicked += () => OpenPopup(CreatePopUp(), BrushAttributesOverlay.popUpWidth, BrushAttributesOverlay.popUpHeight);

            UpdateValues();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetContentWidth()
        {
            if (direction == SliderDirection.Horizontal)
                contentWidth = 100;
        }

        private void UpdateValues()
        {
            var commonUI = BrushAttributesOverlay.GetCommonUI();
            if (commonUI == null) return; 
            style.display = commonUI.hasBrushSize ? DisplayStyle.Flex : DisplayStyle.None;
            if (!commonUI.hasBrushSize) return; 
            value = commonUI.brushSizeVal;
        }
        
        private VisualElement CreatePopUp()
        {
            var sizeContainer = new VisualElement();
            StyleSheet styleSheet = (StyleSheet)AssetDatabase.LoadAssetAtPath("Packages/com.unity.terrain-tools/Editor/Style/BrushPopup.uss", typeof(StyleSheet));
            if (styleSheet) sizeContainer.styleSheets.Add(styleSheet);

            // add min/max slider
            var sizeMinMaxContainer = new VisualElement();
            sizeMinMaxContainer.style.flexDirection = FlexDirection.Row;
            sizeContainer.Add(sizeMinMaxContainer);
            var sizeMinText = new TextElement();
            sizeMinText.text = "Min"; 
            sizeMinMaxContainer.Add(sizeMinText);
            var sizeMinField = new FloatField();
            sizeMinMaxContainer.Add(sizeMinField);
            
            var sizeMaxText = new TextElement();
            sizeMaxText.text = "Max"; 
            sizeMinMaxContainer.Add(sizeMaxText);
            var sizeMaxField = new FloatField();
            sizeMinMaxContainer.Add(sizeMaxField);
            
            // add jitter slider 
            var sizeJitterContainer = new VisualElement();
            sizeJitterContainer.style.flexDirection = FlexDirection.Row;
            sizeContainer.Add(sizeJitterContainer);
            var jitter = new Slider("Jitter", 0f, 1f);
            jitter.style.flexGrow = 1; 
            sizeJitterContainer.Add(jitter);
            var sizeJitterTextField = new FloatField();
            sizeJitterContainer.Add(sizeJitterTextField);

            sizeMinField.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushSizeMin = e.newValue;
                if (commonUI.brushSizeMin > commonUI.brushSizeMax)
                {
                    commonUI.brushSizeMax = e.newValue;
                    sizeMaxField.value = e.newValue;
                    SetHighValueWithoutNotify(e.newValue);
                }
                SetLowValueWithoutNotify(e.newValue);
            });
            
            sizeMaxField.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushSizeMax = e.newValue; 
                if (commonUI.brushSizeMax < commonUI.brushSizeMin)
                {
                    commonUI.brushSizeMax = e.newValue;
                    sizeMinField.value = e.newValue; 
                    SetLowValueWithoutNotify(e.newValue);
                }
                SetHighValueWithoutNotify(e.newValue);
            });
            
            jitter.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushSizeJitter = e.newValue; 
                sizeJitterTextField.SetValueWithoutNotify(e.newValue);
            });
            
            sizeJitterTextField.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushSizeJitter = e.newValue;
                jitter.SetValueWithoutNotify(e.newValue);
            });
            
            var commonUI = BrushAttributesOverlay.GetCommonUI();
            if (commonUI == null) Debug.LogWarning("cannot display SIZE in BrushAttributesOverlay.CS in PACKAGE");
            
            sizeMinField.value = commonUI.brushSizeMin;
            sizeMaxField.value = commonUI.brushSizeMax; 
            jitter.value = commonUI.brushSizeJitter;
            sizeJitterTextField.value = commonUI.brushSizeJitter; 
            return sizeContainer;
        }

        private void OpenPopup(VisualElement popup, float width, float height)
        {
            var window = ScriptableObject.CreateInstance<EditorWindow>();
            BrushAttributesOverlay.AddBorder(window);
            window.rootVisualElement.Add(popup);
            window.ShowAsDropDown(GUIUtility.GUIToScreenRect(worldBound), new Vector2(width, height));
        }
    }
    
    // BRUSH ROTATION 
    [EditorToolbarElement(id, typeof(SceneView))]
    internal class BrushRotation : CondensedSliderDropdown
    {
        internal const string id = "Brushes/RotationPackage";
        private const string label = "Rotation";
        private const float minValue = -180;
        private const float maxValue = 180;

        static Texture2D s_RotationIcon;            

        static Texture2D Texture
        {
            get
            {
                if(s_RotationIcon == null)
                    s_RotationIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.unity.terrain-tools/Editor/Icons/TerrainOverlays/BrushSettingIcons/Rotation.png", typeof(Texture2D));
                return s_RotationIcon;
            }
        }
        
        public void UpdateOverlayDirection(Layout l)
        {
            DropdownUpdateDirection(BrushAttributesOverlay.activeToolbarLayout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal, null, minValue, maxValue); 
            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };
            clicked += () => OpenPopup(CreatePopUp(), BrushAttributesOverlay.popUpWidth, BrushAttributesOverlay.popUpHeight);
            UpdateValues(); 
        }
        
        public void UpdateOverlayDirection(bool collapsedChanged)
        {
            DropdownUpdateDirection(BrushAttributesOverlay.activeToolbarLayout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal, null, minValue, maxValue);
            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };
            clicked += () => OpenPopup(CreatePopUp(), BrushAttributesOverlay.popUpWidth, BrushAttributesOverlay.popUpHeight);
            UpdateValues(); 
        }

        // On first install of the package, need to rebuild the attribute UI after
        // assets have been loaded. So this function implements a reconstruction path, essentially all the same
        // steps as if you step through all the constructors
        internal override void RebuildContent()
        {
            RebuildContent(Texture, minValue, maxValue);
            ConstructDropdown(null, BrushAttributesOverlay.instance.layout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal);
            UpdateOverlayDirection(true);
            SetContentWidth();
            UpdateValues();
        }

        public BrushRotation()
        : base(label, Texture, minValue, maxValue, null, BrushAttributesOverlay.instance.layout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal)
        {
            BrushAttributesOverlay.RegisterAttribute(this);

            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };
            
            UpdateOverlayDirection(true);
            
            RegisterCallback<AttachToPanelEvent>(e =>
            {
                ToolManager.activeToolChanged += UpdateValues;
                ToolManager.activeContextChanged += UpdateValues;
                BrushAttributesOverlay.instance.layoutChanged += UpdateOverlayDirection; 
                BrushAttributesOverlay.instance.collapsedChanged += UpdateOverlayDirection;
                BrushRotationVariator.BrushRotationChanged += UpdateValues;
            });
            

            this.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushRotation = e.newValue;
            });
            
            RegisterCallback<DetachFromPanelEvent>(e =>
            {
                ToolManager.activeToolChanged -= UpdateValues;
                ToolManager.activeContextChanged -= UpdateValues;
                BrushAttributesOverlay.instance.layoutChanged -= UpdateOverlayDirection; 
                BrushAttributesOverlay.instance.collapsedChanged -= UpdateOverlayDirection;
                BrushRotationVariator.BrushRotationChanged -= UpdateValues;
            });

            SetContentWidth();
            
            clicked += () => OpenPopup(CreatePopUp(), BrushAttributesOverlay.popUpWidth, BrushAttributesOverlay.popUpHeight);

            UpdateValues();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetContentWidth()
        {
            if (direction == SliderDirection.Horizontal)
                contentWidth = 120;
        }

        private void UpdateValues()
        {
            var commonUI = BrushAttributesOverlay.GetCommonUI();
            if (commonUI == null) return; 
            style.display = commonUI.hasBrushRotation ? DisplayStyle.Flex : DisplayStyle.None;
            if (!commonUI.hasBrushRotation) return; 
            value = commonUI.brushRotationVal;
        }

        private VisualElement CreatePopUp()
        {
            var rotationContainer = new VisualElement();
            StyleSheet styleSheet = (StyleSheet)AssetDatabase.LoadAssetAtPath("Packages/com.unity.terrain-tools/Editor/Style/BrushPopup.uss", typeof(StyleSheet));
            if (styleSheet) rotationContainer.styleSheets.Add(styleSheet);

            // add jitter slider 
            var rotationJitterContainer = new VisualElement();
            rotationJitterContainer.style.flexDirection = FlexDirection.Row;
            rotationContainer.Add(rotationJitterContainer);
            var jitter = new Slider("Jitter", 0f, 1f);
            jitter.style.flexGrow = 1; 
            rotationJitterContainer.Add(jitter);
            var rotationJitterTextField = new FloatField();
            rotationJitterContainer.Add(rotationJitterTextField);

            jitter.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushRotationJitter = e.newValue; 
                rotationJitterTextField.SetValueWithoutNotify(e.newValue);
            });
            
            rotationJitterTextField.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushRotationJitter = e.newValue;
                jitter.SetValueWithoutNotify(e.newValue);
            });
            var commonUI = BrushAttributesOverlay.GetCommonUI();
            if (commonUI == null) Debug.LogWarning("cannot display ROTATION in BrushAttributesOverlay.CS in PACKAGE");

            jitter.value = commonUI.brushRotationJitter;
            rotationJitterTextField.value = commonUI.brushRotationJitter; 
            return rotationContainer;
        }

        private void OpenPopup(VisualElement popup, float width, float height)
        {
            var window = ScriptableObject.CreateInstance<EditorWindow>();
            BrushAttributesOverlay.AddBorder(window);
            window.rootVisualElement.Add(popup);
            window.ShowAsDropDown(GUIUtility.GUIToScreenRect(worldBound), new Vector2(width, height));
        }
    }
    
    // BRUSH SPACING 
    [EditorToolbarElement(id, typeof(SceneView))]
    internal class BrushSpacing : CondensedSlider
    {
        internal const string id = "Brushes/SpacingPackage";
        private const string label = "Spacing";
        private const float minValue = 0;
        private const float maxValue = 100;
        private const float offset = 100;

        static Texture2D s_SpacingIcon;            

        static Texture2D Texture
        {
            get
            {
                if(s_SpacingIcon == null)
                    s_SpacingIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.unity.terrain-tools/Editor/Icons/TerrainOverlays/BrushSettingIcons/Spacing.png", typeof(Texture2D));
                return s_SpacingIcon;
            }
        }
        
        public void UpdateOverlayDirection(Layout l)
        { 
            UpdateDirection(BrushAttributesOverlay.activeToolbarLayout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal, minValue, maxValue);
            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };
            UpdateValues(); 
        }
        
        public void UpdateOverlayDirection(bool collapsedChanged)
        {
            UpdateDirection(BrushAttributesOverlay.activeToolbarLayout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal, minValue, maxValue);
            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };
            UpdateValues(); 
        }

        // On first install of the package, need to rebuild the attribute UI after
        // assets have been loaded. So this function implements a reconstruction path, essentially all the same
        // steps as if you step through all the constructors
        internal override void RebuildContent()
        {
            RebuildContent(Texture, minValue, maxValue);
            UpdateOverlayDirection(true);
            SetContentWidth();
            UpdateValues();
        }

        public BrushSpacing()
        : base(label, Texture, minValue, maxValue, BrushAttributesOverlay.instance.layout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal)
        {
            BrushAttributesOverlay.RegisterAttribute(this);

            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };

            UpdateOverlayDirection(true);
            
            RegisterCallback<AttachToPanelEvent>(e =>
            {
                ToolManager.activeToolChanged += UpdateValues;
                ToolManager.activeContextChanged += UpdateValues;
                BrushAttributesOverlay.instance.layoutChanged += UpdateOverlayDirection; 
                BrushAttributesOverlay.instance.collapsedChanged += UpdateOverlayDirection;
                BrushSpacingVariator.BrushSpacingChanged += UpdateValues; 
            });

            this.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushSpacing = e.newValue / offset;
            });
            
            RegisterCallback<DetachFromPanelEvent>(e =>
            {
                ToolManager.activeToolChanged -= UpdateValues;
                ToolManager.activeContextChanged -= UpdateValues;
                BrushAttributesOverlay.instance.layoutChanged -= UpdateOverlayDirection; 
                BrushAttributesOverlay.instance.collapsedChanged -= UpdateOverlayDirection;
                BrushSpacingVariator.BrushSpacingChanged -= UpdateValues; 
            });

            SetContentWidth();            
            
            UpdateValues();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetContentWidth()
        {
            if (direction == SliderDirection.Horizontal)
                contentWidth = 118;
        }

        private void UpdateValues()
        {
            var commonUI = BrushAttributesOverlay.GetCommonUI();
            if (commonUI == null) return; 
            style.display = commonUI.hasBrushSpacing ? DisplayStyle.Flex : DisplayStyle.None;
            if (!commonUI.hasBrushSpacing) return; 
            value = commonUI.brushSpacing * offset;
        }
        
    }
    
    // BRUSH SCATTERING 
    [EditorToolbarElement(id, typeof(SceneView))]
    internal class BrushScattering : CondensedSlider
    {
        internal const string id = "Brushes/ScatteringPackage";
        private const string label = "Scattering";
        private const float minValue = 0;
        private const float maxValue = 100;
        private const float offset = 100;

        static Texture2D s_ScatteringIcon;           

        static Texture2D Texture
        {
            get
            {
                if (s_ScatteringIcon == null)
                    s_ScatteringIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.unity.terrain-tools/Editor/Icons/TerrainOverlays/BrushSettingIcons/Scattering.png", typeof(Texture2D));
                return s_ScatteringIcon;
            }
        }
        
        public void UpdateOverlayDirection(Layout l)
        {
            UpdateDirection(BrushAttributesOverlay.activeToolbarLayout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal, minValue, maxValue);
            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };
            UpdateValues(); 
        }
        
        public void UpdateOverlayDirection(bool collapsedChanged)
        {
            UpdateDirection(BrushAttributesOverlay.activeToolbarLayout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal, minValue, maxValue);
            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };
            UpdateValues(); 
        }

        // On first install of the package, need to rebuild the attribute UI after
        // assets have been loaded. So this function implements a reconstruction path, essentially all the same
        // steps as if you step through all the constructors
        internal override void RebuildContent()
        {
            RebuildContent(Texture, minValue, maxValue);
            UpdateOverlayDirection(true);
            SetContentWidth();
            UpdateValues();
        }

        public BrushScattering()
        : base(label, Texture, minValue, maxValue, BrushAttributesOverlay.instance.layout == Layout.VerticalToolbar ? SliderDirection.Vertical : SliderDirection.Horizontal)
        {
            BrushAttributesOverlay.RegisterAttribute(this);

            labelFormatting = (f, s, d) =>
            {
                if (direction == SliderDirection.Vertical)
                    return $"{f:F2}";
                return $"{s} {f:F2}";
            };

            UpdateOverlayDirection(true);
            
            RegisterCallback<AttachToPanelEvent>(e =>
            {
                ToolManager.activeToolChanged += UpdateValues;
                ToolManager.activeContextChanged += UpdateValues;
                BrushAttributesOverlay.instance.layoutChanged += UpdateOverlayDirection; 
                BrushAttributesOverlay.instance.collapsedChanged += UpdateOverlayDirection;
                BrushScatterVariator.BrushScatterChanged += UpdateValues;
            });
            
            this.RegisterValueChangedCallback(e =>
            {
                var commonUI = BrushAttributesOverlay.GetCommonUI();
                if (commonUI == null) return;
                commonUI.brushScatter = e.newValue / offset;
            });
            
            RegisterCallback<DetachFromPanelEvent>(e =>
            {
                ToolManager.activeToolChanged -= UpdateValues;
                ToolManager.activeContextChanged -= UpdateValues;
                BrushAttributesOverlay.instance.layoutChanged -= UpdateOverlayDirection; 
                BrushAttributesOverlay.instance.collapsedChanged -= UpdateOverlayDirection;
                BrushScatterVariator.BrushScatterChanged -= UpdateValues;
            });

            SetContentWidth();
            
            UpdateValues();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetContentWidth()
        {
            if (direction == SliderDirection.Horizontal)
                contentWidth = 130;
        }

        private void UpdateValues()
        {
            var commonUI = BrushAttributesOverlay.GetCommonUI();
            if (commonUI == null) return; 
            style.display = commonUI.hasBrushScatter ? DisplayStyle.Flex : DisplayStyle.None;
            if (!commonUI.hasBrushScatter) return; 
            value = commonUI.brushScatter * offset;
        }
    }
}
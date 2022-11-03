using System;
using UnityEngine;
using UEditor = UnityEditor.Editor;
using UnityEngine.UIElements;

namespace UnityEditor.TerrainTools.UI
{
    internal class CondensedSlider : VisualElement, INotifyValueChanged<float>
    {
        private Slider m_Slider;
        private readonly string m_Label;
        private Label m_LabelField;
        private Texture2D m_Image; 
        
        // this is a variable of type func which takes in a float, string, Slider direction and returns a string
        // the internal stuff is what it starts out as
        // so maybe declare this as a property and give it a get; set; and set the value in the constructor like line 55 or something
        public Func<float, string, SliderDirection, string> labelFormatting
        {
            get;
            set; 
        }
        
        public StyleLength contentWidth
        {
            get => m_Slider.style.width;
            set => m_Slider.style.width = value;
        }

        public StyleLength contentHeight
        {
            get => m_Slider.style.height;
            set => m_Slider.style.height = value;
        }

        public SliderDirection direction
        {
            get =>  m_Slider.direction;
            set => m_Slider.direction = value;

        }
        public float value
        {
            get => m_Slider.value;
            set => SetValue(value);
        }
        
        public CondensedSlider(float min, float max, SliderDirection direction = SliderDirection.Horizontal)
            : this(null, null, min, max, direction)
        {
        }

        public CondensedSlider(string label, float min, float max, SliderDirection direction = SliderDirection.Horizontal)
            : this(label, null, min, max, direction)
        {
        }

        public CondensedSlider(Texture2D image, float min, float max, SliderDirection direction = SliderDirection.Horizontal)
            : this(null, image, min, max, direction)
        {
        }

        public CondensedSlider(string label, Texture2D image, float min, float max, SliderDirection direction = SliderDirection.Horizontal)
        {
            m_Label = label;
            tooltip = label;
            m_Image = image; 
            CreateSlider(min, max, direction);
            
        }

        private void UpdateValue(float value, bool withoutNotify)
        {
            if (withoutNotify)
                m_Slider.SetValueWithoutNotify(value);
            else
                m_Slider.value = value;

            var size = (value - m_Slider.lowValue) / (m_Slider.highValue - m_Slider.lowValue);
            if (m_Slider.direction == SliderDirection.Horizontal)
                m_Slider.Q("unity-tracker").style.width = new StyleLength(new Length(size * 100, LengthUnit.Percent));
            else
            {
                m_Slider.Q("unity-tracker").style.height = new StyleLength(new Length(size * 100, LengthUnit.Percent));
                m_Slider.Q("unity-tracker").style.top = new StyleLength(new Length((1 - size) * 100, LengthUnit.Percent));
            }

            m_LabelField.text = labelFormatting(m_Slider.value, m_Label, direction);
        }

        public void SetHighValueWithoutNotify(float highValue)
        {
            m_Slider.highValue = highValue;
            UpdateValue(m_Slider.value, true);
        }

        public void SetLowValueWithoutNotify(float lowValue)
        {
            m_Slider.lowValue = lowValue;
            UpdateValue(m_Slider.value, true);
        }

        private void SetValue(float value)
        {
            UpdateValue(value, false);
        }
        public void SetValueWithoutNotify(float value)
        {
            UpdateValue(value, true);
        }
        
        

        public void CreateSlider(float min, float max, SliderDirection direction)
        {
            labelFormatting = (value, label, direction) =>
            {
                if (direction == SliderDirection.Horizontal)
                    return $"{label} {value:F0}";
                return $"{value:F0}";
            };
            
            StyleSheet styleSheet = (StyleSheet)AssetDatabase.LoadAssetAtPath("Packages/com.unity.terrain-tools/Editor/Style/CondensedSlider.uss", typeof(StyleSheet));
            if (styleSheet) styleSheets.Add(styleSheet);

            var directionClassSuffix = direction == SliderDirection.Horizontal ? "horizontal" : "vertical";

            AddToClassList("condensed-slider");
            AddToClassList("condensed-slider--"+directionClassSuffix);
            m_Slider = new Slider("", min, max, direction);
            m_Slider.AddToClassList("condensed-slider__slider");
            m_Slider.AddToClassList("condensed-slider__slider--"+directionClassSuffix);
            Add(m_Slider);
            var slider = m_Slider.Q("unity-tracker");
            slider.ClearClassList();
            slider.AddToClassList("condensed-slider__slider-tracker");
            m_Slider.RegisterCallback<GeometryChangedEvent>(e =>
            {
                if (direction == SliderDirection.Horizontal)
                    slider.style.height = e.newRect.height;
                else
                    slider.style.width = e.newRect.width;
            });
            m_Slider.Q("unity-dragger").style.display = DisplayStyle.None;
            m_Slider.Q("unity-dragger-border").style.display = DisplayStyle.None;

            var content = new VisualElement();
            content.AddToClassList("condensed-slider__content--"+directionClassSuffix);
            content.pickingMode = PickingMode.Ignore;
            m_Slider.Add(content);
            m_Slider.RegisterCallback<GeometryChangedEvent>(e => content.style.width = e.newRect.width);
            var imageField = new VisualElement();
            imageField.AddToClassList("condensed-slider__image");
            imageField.AddToClassList("condensed-slider__image--"+directionClassSuffix);
            imageField.pickingMode = PickingMode.Ignore;
            if (m_Image == null)
                imageField.style.display = DisplayStyle.None;
            else
                imageField.style.backgroundImage = m_Image;
            content.Add(imageField);
            m_LabelField = new Label(labelFormatting(m_Slider.value, m_Label, direction));
            m_LabelField.AddToClassList("condensed-slider__label");
            m_LabelField.AddToClassList("condensed-slider__label--"+directionClassSuffix);
            m_LabelField.pickingMode = PickingMode.Ignore;
            content.Add(m_LabelField);


            var contentTextField = new VisualElement();
            contentTextField.AddToClassList("condensed-slider__content-textfield--"+directionClassSuffix);
            contentTextField.pickingMode = PickingMode.Ignore;
            m_Slider.Add(contentTextField);
            var textField = new FloatField();
            textField.AddToClassList("condensed-slider__label");
            textField.AddToClassList("condensed-slider__textfield--"+directionClassSuffix);
            textField.style.display = DisplayStyle.None;
            if (m_Image != null)
            {
                if (direction == SliderDirection.Horizontal)
                    textField.style.marginLeft = 23;
                else
                    textField.style.marginTop = 34;
            }

            textField.RegisterValueChangedCallback(e => UpdateValue(e.newValue, false));
            contentTextField.Add(textField);

            m_Slider.RegisterValueChangedCallback(e =>
            {
                SetValueWithoutNotify(e.newValue);
            });
            // open the textfield when right clicking on the slider
            m_Slider.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.button == (int)MouseButton.RightMouse)
                {
                    e.StopPropagation();
                    e.PreventDefault();

                    textField.value = m_Slider.value;
                    textField.style.display = DisplayStyle.Flex;
                    m_LabelField.style.display = DisplayStyle.None;
                    contentTextField.pickingMode = PickingMode.Position;
                }
            });
            // Stop propagating the mouse up to avoid context menus
            m_Slider.RegisterCallback<MouseUpEvent>(e =>
            {
                if (e.button == (int)MouseButton.RightMouse)
                {
                    e.StopPropagation();
                    e.PreventDefault();
                }
            });
            // Any click inside the hidden element closes the textfield
            contentTextField.RegisterCallback<MouseDownEvent>(e =>
            {
                contentTextField.pickingMode = PickingMode.Ignore;
                if (textField.style.display == DisplayStyle.Flex)
                {
                    textField.style.display = DisplayStyle.None;
                    m_LabelField.style.display = DisplayStyle.Flex;
                }
            });
            // closes the textfield on escape or return
            textField.RegisterCallback<KeyDownEvent>(e =>
            {
                if (e.keyCode == KeyCode.Escape || e.keyCode == KeyCode.Return)
                {
                    contentTextField.pickingMode = PickingMode.Ignore;
                    if (textField.style.display == DisplayStyle.Flex)
                    {
                        textField.style.display = DisplayStyle.None;
                        m_LabelField.style.display = DisplayStyle.Flex;
                    }
                }
            });
            
        }
        
        public void UpdateDirection(SliderDirection newDirection, float min, float max)
        {
            if (direction == newDirection) return; // no new direction
            direction = newDirection;
            Clear();
            ClearClassList();
            CreateSlider(min, max, newDirection);
        }

    }

    internal class CondensedSliderDropdown : CondensedSlider
    {
        private Button m_Dropdown;

        public event Action clicked
        {
            add => m_Dropdown.clicked += value;
            remove => m_Dropdown.clicked -= value;
        }

        public CondensedSliderDropdown(float min, float max, Action clicked, SliderDirection direction = SliderDirection.Horizontal)
            : this(null, null, min, max, clicked, direction)
        { }

        public CondensedSliderDropdown(string label, float min, float max, Action clicked, SliderDirection direction = SliderDirection.Horizontal)
            : this(label, null, min, max, clicked, direction)
        { }
        public CondensedSliderDropdown(Texture2D image, float min, float max, Action clicked, SliderDirection direction = SliderDirection.Horizontal)
            : this(null, image, min, max, clicked, direction)
        { }
        public CondensedSliderDropdown(string label, Texture2D image, float min, float max, Action clicked, SliderDirection direction = SliderDirection.Horizontal)
            : base(label, image, min, max, direction)
        {
            var directionClassSuffix = direction == SliderDirection.Horizontal ? "horizontal" : "vertical";

            m_Dropdown = new Button(clicked);
            Add(m_Dropdown);
            m_Dropdown.ClearClassList();
            m_Dropdown.AddToClassList("unity-base-popup-field__arrow");
            m_Dropdown.AddToClassList("condensed-slider__dropdown--"+directionClassSuffix);
        }
        
        public void DropdownUpdateDirection(SliderDirection newDirection, Action clicked, float min, float max)
        {
            Remove(m_Dropdown);
            UpdateDirection(newDirection, min, max);
            
            var directionClassSuffix = direction == SliderDirection.Horizontal ? "horizontal" : "vertical";
            
            m_Dropdown = new Button(clicked);
            Add(m_Dropdown);
            m_Dropdown.ClearClassList();
            m_Dropdown.AddToClassList("unity-base-popup-field__arrow");
            m_Dropdown.AddToClassList("condensed-slider__dropdown--"+directionClassSuffix);

        }
        

    }
}
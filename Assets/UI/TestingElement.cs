using Unity.Properties;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UIElements;

namespace TideAndFinsUILibrary
{
    [UxmlElement]
    public partial class RadialProgress : VisualElement
    {
        /// <summary>
        /// Class names used for styling this component in USS.
        /// </summary>
        static class ClassNames
        {
            public static readonly string RadialProgress = "radial-progress";
            public static readonly string Label = "radial-progress__label";
        }

        // These objects allow C# code to access custom USS properties.
        static CustomStyleProperty<Color> s_TrackColor = new CustomStyleProperty<Color>("--track-color");
        static CustomStyleProperty<Color> s_ProgressColor = new CustomStyleProperty<Color>("--progress-color");

        // Color of the background circle
        Color m_TrackColor = Color.black;

        // Color of the Progress bar
        Color m_ProgressColor = Color.red;

        // This is the label that displays the percentage.
        Label m_Label;

        // This is the number that the Label displays as a percentage.
        float m_Progress;
        float m_Thickness;

        [UxmlAttribute]
        public Color TrackColor
        {
            get => m_TrackColor;
            set
            {
                m_TrackColor = value;
                MarkDirtyRepaint();
            }
        }

        [UxmlAttribute]
        public Color ProgressColor
        {
            get => m_ProgressColor;
            set
            {
                m_ProgressColor = value;
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// This property stores a value between 0 and 100.
        ///
        /// Note this attributes:
        /// [UxmlAttribute] allows this property to be set directly in the UXML definition.
        /// [CreateProperty] allows data binding and change notifications to work.
        /// 
        /// </summary>
        [UxmlAttribute]
        [CreateProperty]
        public float Progress
        {
            // Exposes the Progress property.
            get => m_Progress;
            set
            {
                // Set the Progress value and update the label text
                m_Progress = value;
                float num = Mathf.Clamp(Mathf.Round(value), 0, 100);
                if (num < 10)
                {
                    m_Label.text = "0"+num.ToString();
                }
                else
                {
                    m_Label.text = num.ToString();
                }


                // Triggers the generateVisualContent callback and repaints the element.
                // This is used to refresh the UI when the Progress changes.
                // Useful for custom controls, especially when visual content needs updating.
                MarkDirtyRepaint();
            }
        }

        [UxmlAttribute]
        [CreateProperty]

        public float LineThickness
        {
            get => m_Thickness;
            set
            {
                m_Thickness = value;
                MarkDirtyRepaint(); 
            }
        }
        /// <summary>
        /// This default constructor is RadialProgress's only constructor.
        /// </summary>
        public RadialProgress()
        {
            // Create a Label, add a USS class name, and add it to this visual tree.
            m_Label = new Label();
            m_Label.AddToClassList(ClassNames.Label);
            Add(m_Label);

            // Assign a distinct name in the Hierarchy
            m_Label.name = ClassNames.Label;

            // Add the USS class name for the overall control.
            AddToClassList(ClassNames.RadialProgress);

            // Register a callback after custom style resolution.
            RegisterCallback<CustomStyleResolvedEvent>(evt => CustomStylesResolved(evt));

            // Register a callback to generate the visual content of the control.
            generateVisualContent += GenerateVisualContent;

            Progress = 0.0f;
        }

        /// <summary>
        /// Custom styles (such as colors) are applied after the stylesheets are resolved.
        /// This method ensures the custom styles are updated accordingly.
        /// </summary>
        /// <param name="evt">The event that triggers after custom styles are resolved.</param>
        static void CustomStylesResolved(CustomStyleResolvedEvent evt)
        {
            RadialProgress element = (RadialProgress)evt.currentTarget;
            element.UpdateCustomStyles();
        }

        /// <summary>
        /// After the custom colors are resolved, this method colors the meshes and repaints
        /// the control.
        /// </summary>
        void UpdateCustomStyles()
        {
            bool repaint = customStyle.TryGetValue(s_ProgressColor, out m_ProgressColor);

            if (customStyle.TryGetValue(s_TrackColor, out m_TrackColor))
                repaint = true;

            if (repaint)
                MarkDirtyRepaint();
        }

        /// <summary>
        /// Generates the visual content for the RadialProgress control, including the track and the Progress arc.
        /// </summary>
        /// <param name="context">The context used to generate the mesh content.</param>
        void GenerateVisualContent(MeshGenerationContext context)
        {
            float width = contentRect.width;
            float height = contentRect.height;

            var painter = context.painter2D;

            painter.lineWidth = m_Thickness;
            painter.lineCap = LineCap.Round;

            painter.strokeColor = m_TrackColor;
            painter.BeginPath();
            painter.Arc(new Vector2(width * 0.5f, height * 0.5f), width * 0.5f, 0.0f, 360.0f);
            painter.Stroke();

            painter.strokeColor = m_ProgressColor;
            painter.BeginPath();
            painter.Arc(new Vector2(width * 0.5f, height * 0.5f), width * 0.5f, -90.0f,
                360.0f * (Progress / 100.0f) - 90.0f);
            painter.Stroke();
        }
    }
}
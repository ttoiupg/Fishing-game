using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(IntVariable))]
public class IntVariableDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement inspector = new();
        inspector.styleSheets.Add(Resources.Load<StyleSheet>("soap_style"));
        inspector.AddToClassList("card");
        return BuildUI(inspector, property);
    }
    private VisualElement BuildUI(VisualElement rootElement, SerializedProperty property)
    {
        var objectField = new ObjectField(property.displayName)
        {
            objectType = typeof(IntVariable),
        };
        objectField.BindProperty(property);
        rootElement.Add(objectField);
        var valueLabel = new Label();
        valueLabel.AddToClassList("cardText");
        rootElement.Add(valueLabel);
        objectField.RegisterValueChangedCallback(evt =>
        {
            var intVar = evt.newValue as IntVariable;
            if (intVar != null)
            {
                valueLabel.text = $"Current Value: {intVar.Value}";
                intVar.OnValueChanged += newValue => valueLabel.text = $"Current Value: {newValue}";
            }
            else
            {
                valueLabel.text = "Value: N/A";
            }
        });
        var currentVariable = property.objectReferenceValue as IntVariable;
        if (currentVariable != null)
        {
            valueLabel.text = $"Current Value: {currentVariable.Value}";
            currentVariable.OnValueChanged += newValue => valueLabel.text = $"Current Value: {newValue}";
        }
        return rootElement;
    }
}
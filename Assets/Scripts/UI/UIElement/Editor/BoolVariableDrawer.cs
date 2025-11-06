using Halfmoon.InspectorUI;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;



[CustomPropertyDrawer(typeof(BoolVariable),true)]
public class BoolVariableDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement inspector = new();
        inspector.styleSheets.Add(Resources.Load<StyleSheet>("soap_style"));
        inspector.AddToClassList("card--true");
        return BuildUI(inspector, property);
    }

    private VisualElement BuildUI(VisualElement rootElement, SerializedProperty property)
    {
        var objectField = new ObjectField(property.displayName)
        {
            objectType = typeof(BoolVariable),
        };
        objectField.BindProperty(property);
        objectField.AddToClassList("cardText");
        rootElement.Add(objectField);
        var valueLabel = new Label();
        valueLabel.AddToClassList("cardText");
        rootElement.Add(valueLabel);
        objectField.RegisterValueChangedCallback(evt =>
        {
            var intVar = evt.newValue as BoolVariable;
            if (intVar != null)
            {
                valueLabel.text = intVar.Value ? "Current Value: true" : "Current Value: false";
                if (intVar.Value)
                {
                    rootElement.RemoveFromClassList("card--false");
                    rootElement.AddToClassList("card--true");
                }
                else
                {
                    rootElement.RemoveFromClassList("card--true");
                    rootElement.AddToClassList("card--false");
                }
                intVar.OnValueChanged += newValue =>
                {
                    valueLabel.text = intVar.Value ? "Current Value: true" : "Current Value: false";
                    if (intVar.Value)
                    {
                        rootElement.RemoveFromClassList("card--false");
                        rootElement.AddToClassList("card--true");
                    }
                    else
                    {
                        rootElement.RemoveFromClassList("card--true");
                        rootElement.AddToClassList("card--false");
                    }
                };
            }
            else
            {
                valueLabel.text = "Value: N/A";
            }
        });
        var currentVariable = property.objectReferenceValue as BoolVariable;
        if (currentVariable != null)
        {
            valueLabel.text = $"Current Value: {currentVariable.Value}";
            currentVariable.OnValueChanged += newValue => valueLabel.text = newValue ? "Current Value: true" : "Current Value: false";
            if (currentVariable.Value)
            {
                rootElement.RemoveFromClassList("card--false");
                rootElement.AddToClassList("card--true");
            }
            else
            {
                rootElement.RemoveFromClassList("card--true");
                rootElement.AddToClassList("card--false");
            }
        }
        return rootElement;
    }
}

using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ObjectCountGoals))]
public class ObjectCountGoalsEditor : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();

        var typeField = new PropertyField(property.FindPropertyRelative("type"), "Type");

        var countProperty = property.FindPropertyRelative("count");
        var countField = new IntegerField("Count");
        countField.BindProperty(countProperty);

        SerializedProperty goalsProperty = property.FindPropertyRelative("goal");
        var goalsField = new IntegerField("Goals");
        goalsField.BindProperty(goalsProperty);
        goalsField.SetEnabled(false);

        var goalsSlider = new SliderInt(0, countProperty.intValue / 3);
        goalsField.value = goalsProperty.intValue / 3;

        countField.RegisterValueChangedCallback(field =>
        {
            int newMax = field.newValue / 3;
            if (goalsSlider.value > newMax)
                goalsSlider.value = newMax;
            goalsSlider.highValue = newMax;
        });
        goalsSlider.RegisterValueChangedCallback(slider =>
        {
            goalsField.value = goalsSlider.value * 3;
        });

        container.Add(typeField);
        container.Add(countField);
        container.Add(goalsField);
        container.Add(goalsSlider);

        return container;
    }

}

using UnityEngine;
using UnityEditor;
using static DialogTableUnit;

[CustomPropertyDrawer(typeof(StepUnit))]
public class StepUnitDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects
        Rect actionTypeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect actorNickNameRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        Rect spawnTypeRect = new Rect(position.x, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        Rect actionPlaceRect = new Rect(position.x, position.y + 3 * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        Rect actionTimeRect = new Rect(position.x, position.y + 4 * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        Rect speechTextRect = new Rect(position.x, position.y + 5 * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        Rect isStopRect = new Rect(position.x, position.y + 6 * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

        // Get properties
        SerializedProperty unitActionType = property.FindPropertyRelative("UnitActionType");
        SerializedProperty actorNickName = property.FindPropertyRelative("ActorNickName");
        SerializedProperty spawnType = property.FindPropertyRelative("SpawnType");
        SerializedProperty actionPlace = property.FindPropertyRelative("ActionPlace");
        SerializedProperty actionTime = property.FindPropertyRelative("ActionTime");
        SerializedProperty speechText = property.FindPropertyRelative("SpeechText");
        SerializedProperty isStop = property.FindPropertyRelative("IsStop");

        // Draw fields
        EditorGUI.PropertyField(actionTypeRect, unitActionType);

        StepUnit.ActionType actionType = (StepUnit.ActionType)unitActionType.enumValueIndex;

        int line = 1;
        if (actionType != StepUnit.ActionType.None)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y + line * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), actorNickName);
            line++;
        }
        if (actionType == StepUnit.ActionType.Spawn)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y + line * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), spawnType);
            line++;
            EditorGUI.PropertyField(new Rect(position.x, position.y + line * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), actionPlace);
            line++;
        }
        if (actionType == StepUnit.ActionType.Move)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y + line * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), actionPlace);
            line++;
            EditorGUI.PropertyField(new Rect(position.x, position.y + line * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), actionTime);
            line++;
        }
        if (actionType == StepUnit.ActionType.Speech)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y + line * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), actionTime);
            line++;
            EditorGUI.PropertyField(new Rect(position.x, position.y + line * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), speechText);
            line++;
        }
        if (actionType != StepUnit.ActionType.None)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y + line * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), isStop);
            line++;
        }

        // Add space between steps
        if (actionType != StepUnit.ActionType.None)
        {
            line++;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty unitActionType = property.FindPropertyRelative("UnitActionType");
        StepUnit.ActionType actionType = (StepUnit.ActionType)unitActionType.enumValueIndex;

        int lines = 1;
        if (actionType != StepUnit.ActionType.None) lines++;
        if (actionType == StepUnit.ActionType.Spawn) lines += 3;
        if (actionType == StepUnit.ActionType.Move) lines += 2;
        if (actionType == StepUnit.ActionType.Speech) lines += 2;
        if (actionType != StepUnit.ActionType.None) lines++;

        // Add space between steps
        if (actionType != StepUnit.ActionType.None) lines++;

        return lines * EditorGUIUtility.singleLineHeight;
    }
}

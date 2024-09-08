using UnityEditor;
using UnityEngine;
using static DialogTableUnit;

[CustomPropertyDrawer(typeof(StepUnit))]
public class StepUnitDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 들여쓰기 정도를 변수에 저장
        int indent = EditorGUI.indentLevel;
        // 들여쓰기 0으로 초기화
        EditorGUI.indentLevel = 0;

        // 각 필드의 위치 설정
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing * 2; // 간격을 더 넓게 설정

        Rect actionTypeRect = new Rect(position.x, position.y, position.width, lineHeight);
        Rect actorNickNameRect = new Rect(position.x, position.y + lineHeight + spacing, position.width, lineHeight);
        Rect actionPlaceRect = new Rect(position.x, position.y + 2 * (lineHeight + spacing), position.width, lineHeight);
        Rect speechTextRect = new Rect(position.x, position.y + 2 * (lineHeight + spacing), position.width, lineHeight);
        Rect isStopRect = new Rect(position.x, position.y + 3 * (lineHeight + spacing), position.width, lineHeight);

        // 각 필드 가져오기
        var actionType = property.FindPropertyRelative("actionType");
        var actorNickName = property.FindPropertyRelative("ActorNickName");
        var actionPlace = property.FindPropertyRelative("ActionPlace");
        var speechText = property.FindPropertyRelative("SpeechText");
        var isStop = property.FindPropertyRelative("isStop");

        // 필드 그리기
        EditorGUI.PropertyField(actionTypeRect, actionType);
        EditorGUI.PropertyField(actorNickNameRect, actorNickName);

        // ActionType에 따라 필드 활성화/비활성화
        switch ((StepUnit.ActionType)actionType.enumValueIndex)
        {
            case StepUnit.ActionType.Spawn:
            case StepUnit.ActionType.Move:
                EditorGUI.PropertyField(actionPlaceRect, actionPlace);
                break;
            case StepUnit.ActionType.Speech:
                EditorGUI.PropertyField(speechTextRect, speechText);
                break;
        }

        // isStop 필드 항상 표시
        EditorGUI.PropertyField(isStopRect, isStop);

        // 들여쓰기 복원
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 기본 높이 계산
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing * 5; // 간격을 더 넓게 설정

        // 기본 높이 + 추가 필드 높이
        var actionType = property.FindPropertyRelative("actionType");
        switch ((StepUnit.ActionType)actionType.enumValueIndex)
        {
            case StepUnit.ActionType.Spawn:
            case StepUnit.ActionType.Move:
                return 4 * (lineHeight + spacing);
            case StepUnit.ActionType.Speech:
                return 4 * (lineHeight + spacing); // SpeechText가 바로 아래에 나오도록 설정
            default:
                return 3 * (lineHeight + spacing);
        }
    }
}
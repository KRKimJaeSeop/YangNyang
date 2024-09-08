using UnityEditor;
using UnityEngine;
using static DialogTableUnit;

[CustomPropertyDrawer(typeof(StepUnit))]
public class StepUnitDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // �鿩���� ������ ������ ����
        int indent = EditorGUI.indentLevel;
        // �鿩���� 0���� �ʱ�ȭ
        EditorGUI.indentLevel = 0;

        // �� �ʵ��� ��ġ ����
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing * 2; // ������ �� �а� ����

        Rect actionTypeRect = new Rect(position.x, position.y, position.width, lineHeight);
        Rect actorNickNameRect = new Rect(position.x, position.y + lineHeight + spacing, position.width, lineHeight);
        Rect actionPlaceRect = new Rect(position.x, position.y + 2 * (lineHeight + spacing), position.width, lineHeight);
        Rect speechTextRect = new Rect(position.x, position.y + 2 * (lineHeight + spacing), position.width, lineHeight);
        Rect isStopRect = new Rect(position.x, position.y + 3 * (lineHeight + spacing), position.width, lineHeight);

        // �� �ʵ� ��������
        var actionType = property.FindPropertyRelative("actionType");
        var actorNickName = property.FindPropertyRelative("ActorNickName");
        var actionPlace = property.FindPropertyRelative("ActionPlace");
        var speechText = property.FindPropertyRelative("SpeechText");
        var isStop = property.FindPropertyRelative("isStop");

        // �ʵ� �׸���
        EditorGUI.PropertyField(actionTypeRect, actionType);
        EditorGUI.PropertyField(actorNickNameRect, actorNickName);

        // ActionType�� ���� �ʵ� Ȱ��ȭ/��Ȱ��ȭ
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

        // isStop �ʵ� �׻� ǥ��
        EditorGUI.PropertyField(isStopRect, isStop);

        // �鿩���� ����
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // �⺻ ���� ���
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing * 5; // ������ �� �а� ����

        // �⺻ ���� + �߰� �ʵ� ����
        var actionType = property.FindPropertyRelative("actionType");
        switch ((StepUnit.ActionType)actionType.enumValueIndex)
        {
            case StepUnit.ActionType.Spawn:
            case StepUnit.ActionType.Move:
                return 4 * (lineHeight + spacing);
            case StepUnit.ActionType.Speech:
                return 4 * (lineHeight + spacing); // SpeechText�� �ٷ� �Ʒ��� �������� ����
            default:
                return 3 * (lineHeight + spacing);
        }
    }
}
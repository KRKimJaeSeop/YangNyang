using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ResearchTable.Unit))]
public class ResearchUnitPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // �鿩���� ������ ������ ����
        int indent = EditorGUI.indentLevel;
        // �鿩���� 0���� �ϴ� �ʱ�ȭ �ص�.
        EditorGUI.indentLevel = 0;

        // ��ġ ����ְ� rect ����
        float widthSize = position.width * 0.5f;
        float offsetSize = 8;
        Rect levelRect = new Rect(position.x, position.y, widthSize - offsetSize, position.height);
        Rect expRect = new Rect(position.x + (widthSize * 1), position.y, widthSize - offsetSize, position.height);

        // �� ��ġ�� ������Ƽ �ʵ� �׸���
        EditorGUI.PropertyField(levelRect, property.FindPropertyRelative("Level"), new GUIContent("Level & EXP"));
        EditorGUI.PropertyField(expRect, property.FindPropertyRelative("EXP"), GUIContent.none);

        // �Ʊ� �����Ѹ�ŭ �鿩���� ��.
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }


}

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DayStatusTable.Unit))]
public class DayStatusUnitPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 들여쓰기 정도를 변수에 저장
        int indent = EditorGUI.indentLevel;
        // 들여쓰기 0으로 일단 초기화 해둠.
        EditorGUI.indentLevel = 0;

        // 위치 잡아주고 rect 세팅
        float widthSize = position.width * 0.5f;
        float offsetSize = 8;
        Rect levelRect = new Rect(position.x, position.y, widthSize - offsetSize, position.height);
        Rect expRect = new Rect(position.x + (widthSize * 1), position.y, widthSize - offsetSize, position.height);

        // 각 위치에 프로퍼티 필드 그리기
        EditorGUI.PropertyField(levelRect, property.FindPropertyRelative("day"), new GUIContent("day & workSpeed"));
        EditorGUI.PropertyField(expRect, property.FindPropertyRelative("workSpeed"), GUIContent.none);

        // 아까 정의한만큼 들여쓰기 함.
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

}

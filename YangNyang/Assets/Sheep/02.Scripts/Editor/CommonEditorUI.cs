using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CommonEditorUI : MonoBehaviour
{
    [System.Serializable]
    public class HelpBoxMsg
    {
        public MessageType msgType = MessageType.None;
        public string msg = string.Empty;

        public void SetEmpty()
        {
            this.msgType = MessageType.None;
            this.msg = string.Empty;
        }

        public void Set(MessageType msgType, string msg)
        {
            this.msgType = msgType;
            this.msg = msg;
        }

        public void Show()
        {
            if (string.IsNullOrEmpty(msg))
                return;

            EditorGUILayout.HelpBox(msg, msgType);
        }

        public void ShowFormat(float value)
        {
            if (string.IsNullOrEmpty(msg))
                return;

            EditorGUILayout.HelpBox(string.Format(msg, value), msgType);
        }
    }


    public static void DrawSeparator(Color color)
    {
        EditorGUILayout.Space();
        Texture2D tex = new Texture2D(1, 1);

        GUI.color = color;
        float y = GUILayoutUtility.GetLastRect().yMax;
        GUI.DrawTexture(new Rect(0f, y, Screen.width, 1f), tex);
        tex.hideFlags = HideFlags.DontSave;
        GUI.color = Color.white;

        EditorGUILayout.Space();
    }

    public static void DrawSeparator()
    {
        DrawSeparator(new Color(0f, 0f, 0f, 0.25f));
    }

    public static void RegisterUndo(string name, Object obj)
    {
        if (obj != null)
        {
            Undo.RecordObject(obj, name);
            EditorUtility.SetDirty(obj);
        }
    }

    public static void ShowList(SerializedProperty list)
    {
        EditorGUILayout.PropertyField(list);
        EditorGUI.indentLevel += 1;
        for (int i = 0; i < list.arraySize; i++)
        {
            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
        }
        EditorGUI.indentLevel -= 1;
    }

}

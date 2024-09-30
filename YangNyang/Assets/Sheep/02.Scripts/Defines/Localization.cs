using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Localization
{
    /// <summary>
    /// ������Ʈ�� �°� ������ ��.
    /// </summary>
    public enum LocalizationTable
    {
        None = 0,
        App = 1, // �ý��� ����.
        Model = 2, // ��Ī, ���� ���̺�
        UI = 3, // UI ����.
        Dialogue = 4, // ��ȭâ ����
    }

    [Serializable]
    public struct LocalizationData
    {
        public LocalizationTable table;
        public string key;
        private string _tableName;
        public bool IsEmpty()
        {
            return (table == LocalizationTable.None) || (string.IsNullOrEmpty(key));
        }
        public string GetLocalizedString(bool includeNewLine = false)
        {
            if (string.IsNullOrEmpty(_tableName) || _tableName != table.ToString())
            {
                if (table != LocalizationTable.None)
                    _tableName = table.ToString();
                else
                {
                    Debug.Log($"{GetType()}::{nameof(GetLocalizedString)}: localization table is not set.");
                    return "Unknown";
                }
            }

            //return LocalizationSettings.StringDatabase.GetLocalizedString(_tableName, key);
            // "\r" : <CR> ������ ���ڿ� ��ħ ���� ���ֱ�.
            // (https://velog.io/@livelyjuseok/Unity-TextMeshPro-%EB%AC%B8%EC%9E%90%EC%97%B4-%EA%B2%B9%EC%B9%A8-%EB%AC%B8%EC%A0%9C)
            var text = LocalizationSettings.StringDatabase.GetLocalizedString(_tableName, key).Replace("\r", "");
            if (includeNewLine == true)
                text = string.Format(text, System.Environment.NewLine);
            return text;
        }
    }
   
}

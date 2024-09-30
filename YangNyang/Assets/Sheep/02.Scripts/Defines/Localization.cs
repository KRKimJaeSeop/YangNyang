using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Localization
{
    /// <summary>
    /// 프로젝트에 맞게 세팅할 것.
    /// </summary>
    public enum LocalizationTable
    {
        None = 0,
        App = 1, // 시스템 관련.
        Model = 2, // 명칭, 설명 테이블
        UI = 3, // UI 관련.
        Dialogue = 4, // 대화창 관련
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
            // "\r" : <CR> 문제로 문자열 겹침 현상 없애기.
            // (https://velog.io/@livelyjuseok/Unity-TextMeshPro-%EB%AC%B8%EC%9E%90%EC%97%B4-%EA%B2%B9%EC%B9%A8-%EB%AC%B8%EC%A0%9C)
            var text = LocalizationSettings.StringDatabase.GetLocalizedString(_tableName, key).Replace("\r", "");
            if (includeNewLine == true)
                text = string.Format(text, System.Environment.NewLine);
            return text;
        }
    }
   
}

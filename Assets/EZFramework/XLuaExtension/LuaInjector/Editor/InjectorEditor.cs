/*
 * Author:      熊哲
 * CreateTime:  9/4/2017 6:26:57 PM
 * Description:
 * 
*/
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EZFramework.LuaInjector
{
    [CustomEditor(typeof(Injector))]
    public class InjectorEditor : Editor
    {
        private ReorderableList injectionList;

        void OnEnable()
        {
            injectionList = new ReorderableList(serializedObject, serializedObject.FindProperty("injections"), true, true, true, true);
            injectionList.drawHeaderCallback = DrawInjectionListHeader;
            injectionList.drawElementCallback = DrawInjectionListElement;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(target as MonoBehaviour), typeof(MonoScript), false);
            GUI.enabled = true;
            injectionList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawTypeMenu(GenericMenu.MenuFunction2 callback)
        {
            callback += delegate { serializedObject.ApplyModifiedProperties(); };
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < Injection.typeList.Count; i++)
            {
                string typeName = Injection.typeList[i].FullName;
                menu.AddItem(new GUIContent(typeName.Replace(".", "/")), false, callback, typeName);
            }
            menu.ShowAsContext();
        }

        protected void DrawInjectionListHeader(Rect rect)
        {

        }
        protected void DrawInjectionListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 1;
            SerializedProperty pair = injectionList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty key = pair.FindPropertyRelative("key");
            SerializedProperty value = pair.FindPropertyRelative("value");
            SerializedProperty typeName = pair.FindPropertyRelative("typeName");
            if (string.IsNullOrEmpty(typeName.stringValue)) typeName.stringValue = "UnityEngine.Object";
            Type type = Injection.GetType(typeName.stringValue);
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight), index.ToString("00"));
            float width = (rect.width - 20) / 3 - 5;
            if (GUI.Button(new Rect(rect.x + 25, rect.y, width, EditorGUIUtility.singleLineHeight), type.Name))
            {
                DrawTypeMenu(delegate (object name) { typeName.stringValue = (string)name; });
            }
            EditorGUI.PropertyField(new Rect(rect.x + 30 + width, rect.y, width, EditorGUIUtility.singleLineHeight), key, GUIContent.none);
            value.objectReferenceValue = EditorGUI.ObjectField(new Rect(rect.x + 35 + width * 2, rect.y, width, EditorGUIUtility.singleLineHeight), value.objectReferenceValue, type, true);
        }
    }
}
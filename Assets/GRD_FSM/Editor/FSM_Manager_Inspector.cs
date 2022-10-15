using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GRD.FSM
{
    [CustomEditor(typeof(FSM_Manager), true)]
    public class FSM_Manager_Inspector : Editor
    {
        FSM_Manager _myFSMManager;

        private void OnEnable()
        {
            _myFSMManager = (FSM_Manager)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FSM_Name"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_startFSMMethod"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_transitionExecutionMethod"));

            if (GUILayout.Button("Open FSM Editor"))
            {
                OpenFSMEditorWindow();
            }

            if (Application.isPlaying)
            {
                int currentStateIndex = _myFSMManager.GetCurrentStateId();
                SerializedProperty currentStateProp = serializedObject.FindProperty("_states").GetArrayElementAtIndex(currentStateIndex);
                string currentStateName = currentStateProp.FindPropertyRelative("_name").stringValue;
                EditorGUILayout.LabelField("Current State: " + currentStateName);
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void OpenFSMEditorWindow()
        {
            FSM_NodeEditorWindow.OpenWindow(_myFSMManager);
        }
    }
}

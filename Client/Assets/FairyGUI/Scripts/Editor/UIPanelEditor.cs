﻿using System;
using System.Collections.Generic;
using UnityEngine;
#if (UNITY_5_3 || UNITY_5_4)
using UnityEditor.SceneManagement;
#endif
using UnityEditor;
using FairyGUI;

namespace FairyGUIEditor
{
	/// <summary>
	/// 
	/// </summary>
	[CustomEditor(typeof(UIPanel))]
	public class UIPanelEditor : Editor
	{
		SerializedProperty packageName;
		SerializedProperty componentName;
		SerializedProperty renderMode;
		SerializedProperty renderCamera;
		SerializedProperty sortingOrder;
		SerializedProperty position;
		SerializedProperty scale;
		SerializedProperty rotation;
		SerializedProperty fairyBatching;
		SerializedProperty fitScreen;
		SerializedProperty touchDisabled;
		SerializedProperty hitTestMode;

#if UNITY_5
		string[] propertyToExclude;
#endif
		void OnEnable()
		{
			packageName = serializedObject.FindProperty("packageName");
			componentName = serializedObject.FindProperty("componentName");
			renderMode = serializedObject.FindProperty("renderMode");
			renderCamera = serializedObject.FindProperty("renderCamera");
			sortingOrder = serializedObject.FindProperty("sortingOrder");
			position = serializedObject.FindProperty("position");
			scale = serializedObject.FindProperty("scale");
			rotation = serializedObject.FindProperty("rotation");
			fairyBatching = serializedObject.FindProperty("fairyBatching");
			fitScreen = serializedObject.FindProperty("fitScreen");
			touchDisabled = serializedObject.FindProperty("touchDisabled");
			hitTestMode = serializedObject.FindProperty("hitTestMode");

#if UNITY_5
			propertyToExclude = new string[] { "m_Script", "packageName", "componentName", "packagePath", "renderMode",
				"renderCamera", "sortingOrder", "position", "scale", "rotation", "fairyBatching", "fitScreen","touchDisabled",
				"hitTestMode","cachedUISize"
			};
#endif
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			UIPanel panel = target as UIPanel;
#if UNITY_5
			DrawPropertiesExcluding(serializedObject, propertyToExclude);
#endif
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Package Name");
			if (GUILayout.Button(packageName.stringValue, "ObjectField"))
				EditorWindow.GetWindow<PackagesWindow>(true, "Select a UI Component").SetSelection(packageName.stringValue, componentName.stringValue);

			if (GUILayout.Button("Clear", GUILayout.Width(50)))
			{
				bool isPrefab = PrefabUtility.GetPrefabType(panel) == PrefabType.Prefab;
				panel.SendMessage("OnUpdateSource", new object[] { null, null, null, !isPrefab });

#if (UNITY_5_3 || UNITY_5_4)
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
#elif UNITY_5
				EditorApplication.MarkSceneDirty();
#else
				EditorUtility.SetDirty(panel);
#endif
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Component Name");
			if (GUILayout.Button(componentName.stringValue, "ObjectField"))
				EditorWindow.GetWindow<PackagesWindow>(true, "Select a UI Component").SetSelection(packageName.stringValue, componentName.stringValue);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.PropertyField(renderMode);
			if ((RenderMode)renderMode.enumValueIndex != RenderMode.ScreenSpaceOverlay)
				EditorGUILayout.PropertyField(renderCamera);
			int oldSortingOrder = panel.sortingOrder;
			EditorGUILayout.PropertyField(sortingOrder);
			EditorGUILayout.PropertyField(fairyBatching);
			EditorGUILayout.PropertyField(hitTestMode);
			EditorGUILayout.PropertyField(touchDisabled);
			EditorGUILayout.LabelField("UI Transform", (GUIStyle)"OL Title");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(position);
			EditorGUILayout.PropertyField(rotation);
			EditorGUILayout.PropertyField(scale);
			EditorGUILayout.Space();

			FitScreen oldFitScreen = (FitScreen)fitScreen.enumValueIndex;
			EditorGUILayout.PropertyField(fitScreen);

			if (serializedObject.ApplyModifiedProperties())
			{
				if (PrefabUtility.GetPrefabType(panel) != PrefabType.Prefab)
				{
					panel.ApplyModifiedProperties(sortingOrder.intValue != oldSortingOrder, (FitScreen)fitScreen.enumValueIndex != oldFitScreen);
				}
			}
		}

		void OnSceneGUI()
		{
			UIPanel panel = (target as UIPanel);
			if (panel.container == null)
				return;

			Vector3 pos = panel.GetUIWorldPosition();
			float sizeFactor = HandleUtility.GetHandleSize(pos);
			Vector3 newPos = Handles.FreeMoveHandle(pos, Quaternion.identity, sizeFactor, Vector3.one, Handles.ArrowCap);
			if (newPos != pos)
			{
				Vector2 v1 = HandleUtility.WorldToGUIPoint(pos);
				Vector2 v2 = HandleUtility.WorldToGUIPoint(newPos);
				Vector3 delta = v2 - v1;
				delta.x = (int)delta.x;
				delta.y = (int)delta.y;

				panel.MoveUI(delta);
			}
		}
	}
}

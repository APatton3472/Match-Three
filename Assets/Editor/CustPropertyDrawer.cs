﻿using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(ArrayLayout))]
public class CustPropertyDrawer : PropertyDrawer {

	public override void OnGUI(Rect position,SerializedProperty property,GUIContent label){
		EditorGUI.PrefixLabel(position,label);
		Rect newposition = position;
		newposition.y += 18f;
		SerializedProperty data = property.FindPropertyRelative("rows");
        if (data.arraySize != 12)
            data.arraySize = 12;
		//data.rows[0][]
		for(int j=0;j<12;j++){
			SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("row");
			newposition.height = 18f;
			if(row.arraySize != 20)
				row.arraySize = 20;
			newposition.width = position.width/9;
			for(int i=0;i<20;i++){
				EditorGUI.PropertyField(newposition,row.GetArrayElementAtIndex(i),GUIContent.none);
				newposition.x += newposition.width;
			}

			newposition.x = position.x;
			newposition.y += 18f;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property,GUIContent label){
		return 18f * 15;
	}
}

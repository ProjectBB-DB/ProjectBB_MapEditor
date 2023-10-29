using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;


[CustomEditor(typeof(SaveStageRoundData))]
public class SaveStageRoundDataEditor : Editor
{
    public VisualTreeAsset inspectorXML;


    public override VisualElement CreateInspectorGUI()
    {
        // return할 VisualElement
        VisualElement saveInspector = new VisualElement();


/*      // UXML의 Visual Tree를 Clone하고 불러오기
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/UIToolkit/UXML/SaveRoundData_Inspector.uxml");
        visualTree.CloneTree(saveInspector);*/

        // 위 코드 대신 아래와 같이 사용할 수 있습니다.

        // 선언한 inspectorXML에서 에디터 상에서 넣어준 UXML Tree를 통해 Clone하기
        inspectorXML.CloneTree(saveInspector); 



        return saveInspector;
    }


}

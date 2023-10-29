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
        // return�� VisualElement
        VisualElement saveInspector = new VisualElement();


/*      // UXML�� Visual Tree�� Clone�ϰ� �ҷ�����
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/UIToolkit/UXML/SaveRoundData_Inspector.uxml");
        visualTree.CloneTree(saveInspector);*/

        // �� �ڵ� ��� �Ʒ��� ���� ����� �� �ֽ��ϴ�.

        // ������ inspectorXML���� ������ �󿡼� �־��� UXML Tree�� ���� Clone�ϱ�
        inspectorXML.CloneTree(saveInspector); 



        return saveInspector;
    }


}

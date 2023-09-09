using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Dynamic;

//自定义属性绘制器，为具有自定义PropertyAttribute的脚本变量创建自定义绘制器
[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))]
public class ItemCodeDescriptionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property)*2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //调用Begin和End方法，确保创建预制体时绘制的逻辑流程是正确的
        EditorGUI.BeginProperty(position, label, property);

        if(property.propertyType == SerializedPropertyType.Integer)
        {
            //调用Begin和End方法创建一个代码块，检查是否只有该代码块中包含的控件的GUI状态发生了更改
            EditorGUI.BeginChangeCheck();
            //绘制Item的Code
            var newValue = EditorGUI.IntField(new Rect(position.x, position.y,
                position.width, position.height / 2), label, property.intValue);
            //绘制Item的Description
            EditorGUI.LabelField(new Rect(position.x, position.y + position.height / 2,
                position.width, position.height / 2), "Item Description",
                GetItemDescription(property.intValue));

            if(EditorGUI.EndChangeCheck() )
            {
                property.intValue = newValue;
            }
        }

        EditorGUI.EndProperty();
    }

    private string GetItemDescription(int intCode)
    {
        SO_ItemList sO_ItemList;
        sO_ItemList = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Object Assets/Item/so_ItemList.asset",
            typeof(SO_ItemList)) as SO_ItemList;

        List<ItemDetails> itemDetailsList = sO_ItemList.ItemDetails;
        ItemDetails itemDetails = itemDetailsList.Find(x=>x.itemCode == intCode);

        if(itemDetails != null)
        {
            return itemDetails.itemDescription;
        }
        else 
        { 
            return string.Empty;
        }
    }
}

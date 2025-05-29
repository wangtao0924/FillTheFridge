using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

//[CustomEditor(typeof(Button),true)]
//[CanEditMultipleObjects]
/// <summary>
/// time:2019/6/2 13:57
/// author:Sun
/// des:UI类型
///
/// github:https://github.com/KingSun5
/// csdn:https://blog.csdn.net/Mr_Sun88
/// </summary>
public class UILet : MonoBehaviour
{
	[SerializeField]
	public List<UILetGameObject> uiGameObject;
}

[System.Serializable]
public class UILetGameObject
{
	[SerializeField]
	public string uiName;
	[SerializeField]
	public GameObject uiGameObj;


	/// <summary>
	/// UI类型
	/// </summary>
	public string ComponentName
	{
		get
		{
			if (null != uiGameObj.GetComponent<ScrollRect>())
				return "ScrollRect";
			if (null != uiGameObj.GetComponent<InputField>())
				return "InputField";
			if (null != uiGameObj.GetComponent<Button>())
				return "Button";
			if (null != uiGameObj.GetComponent<Text>())
				return "Text";
			if (null != uiGameObj.GetComponent<RawImage>())
				return "RawImage";
			if (null != uiGameObj.GetComponent<Toggle>())
				return "Toggle";
			if (null != uiGameObj.GetComponent<Slider>())
				return "Slider";
			if (null != uiGameObj.GetComponent<Scrollbar>())
				return "Scrollbar";
			if (null != uiGameObj.GetComponent<Image>())
				return "Image";
			if (null != uiGameObj.GetComponent<ToggleGroup>())
				return "ToggleGroup";
			if (null != uiGameObj.GetComponent<Animator>())
				return "Animator";
			if (null != uiGameObj.GetComponent<Canvas>())
				return "Canvas";
			if (null != uiGameObj.GetComponent<RectTransform>())
				return "RectTransform";

			return "Transform";
		}
	}
	/// <summary>
	/// 变量声明内容
	/// </summary>
	public string StatementContent
	{
		get { return "private" + " " + ComponentName + " " + uiName + ";"; }
	}

	/// <summary>
	/// 变量绑定内容
	/// </summary>
	public string BindVariableContent
	{
		get
		{
			var content = uiName + " = " + "GameObject.Find(\"" + uiName + "\")";
			var bindProperty = "GetComponent<" + ComponentName + ">();";
			return content + "." + bindProperty;
		}
	}
}

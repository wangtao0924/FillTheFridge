

using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
	public partial class UiCodeCreate
	{
		
		public static readonly UiCodeCreate Instance = new UiCodeCreate();
		

		[MenuItem("Assets/--生成UI代码--")]
		public static void CreateUiCode()
		{
			//获取选中的Prefab
			var objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets | SelectionMode.TopLevel);

			for (var i = 0; i < objs.Length; i++)
			{
				if (objs[i].name.EndsWith("UI") || objs[i].name.EndsWith("Item"))
				{
					GameObject obj = objs[i] as GameObject;
					Instance.CreateCode(obj);
					if (obj.GetComponent<Canvas>()==null)
                    {
						obj.AddComponent<Canvas>();
					}
                    if (obj.GetComponent<GraphicRaycaster>()==null)
                    {
						obj.AddComponent<GraphicRaycaster>();
					}
				}
				Debug.LogError("命名不是UI或Item结尾！！");
			}
			AssetDatabase.Refresh();
		}

		private void CreateCode(GameObject obj)
		{
			CreateViewCode(obj);
			CreateModelCode(obj);
			//CreateControlCode(obj);
			AddUIEnum(obj);
		}
		/// <summary>
		/// 创建View类
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objPath"></param>
		private void CreateViewCode(GameObject obj)
        {
			StringBuilder statementContent = new StringBuilder();
			//变量绑定
			StringBuilder bindVariableContent = new StringBuilder();
			//写入内容
			StringBuilder fileContent = new StringBuilder();
			fileContent.Append(_viewTemplateContent);
			foreach (var item in obj.GetComponent<UILet>().uiGameObject)
			{
				statementContent.Append("\n");
				statementContent.Append("		");
				statementContent.Append(item.StatementContent);
				bindVariableContent.Append("\n");
				bindVariableContent.Append("			");
				bindVariableContent.Append(item.BindVariableContent);
			}
			//替换脚本名和命名空间
			fileContent = fileContent.Replace("TemNamespace", "UI." + obj.name).Replace("ViewTemplate", obj.name);
			//添加属性和绑定
			fileContent = fileContent.Replace("/*声明*/", statementContent.ToString()).Replace("/*绑定*/", bindVariableContent.ToString());
			CreateFileAndWrite(obj.name, fileContent.ToString(), "View");
		}

		/// <summary>
		/// 创建Model类
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objPath"></param>
		private void CreateModelCode(GameObject obj)
		{
			//写入内容
			StringBuilder fileContent = new StringBuilder();
            if (obj.name.EndsWith("UI"))
            {
				fileContent.Append(_modelTemplateContent);
			}
            if (obj.name.EndsWith("Item"))
            {
				fileContent.Append(_itemTemplateContent);
			}
			//替换脚本名和命名空间
			fileContent = fileContent.Replace("TemNamespace", "UI."+obj.name).Replace("ModelTemplate", obj.name);
			CreateFileAndWrite(obj.name, fileContent.ToString());
		}

		/// <summary>
		/// 创建Control类
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objPath"></param>
		private void CreateControlCode(GameObject obj)
		{
			//写入内容
			StringBuilder fileContent = new StringBuilder();
			fileContent.Append(_controlTemplateContent);
			//替换脚本名和命名空间
			fileContent = fileContent.Replace("TemNamespace", "UI."+obj.name).Replace("ControlTemplate", obj.name);
			CreateFileAndWrite(obj.name, fileContent.ToString(),"Control");
		}

		/// <summary>
		/// 获取文件内容
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		private string GetFileContent(string filePath)
		{
			if (File.Exists(filePath))
			{
				return File.ReadAllText(filePath);

			}
			else
			{
				Debug.Log("Error:["+filePath+"]该文件不存在！");
				return null;
			}
		}

		/// <summary>
		/// 创建文件并写入
		/// </summary>
		/// <returns></returns>
		private void CreateFileAndWrite(string name,string content,string type="")
		{
			string path="";
			if (name.EndsWith("UI"))
            {
				path = _UiFilePath;
			}
            if (name.EndsWith("Item"))
            {
				path = _ItemFilePath;
			}
			if (type.Equals("View"))
            {
				path += "View/";
			}
            else
            {
				path +="UIPanel/";
			}
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			File.WriteAllText(path+"/"+name+type+".cs",content);
		}
		
		private void AddUIEnum(GameObject obj)
        {
			if (obj.name.EndsWith("Item"))
			{
				return;
			}
			StringBuilder fileContent = new StringBuilder();
			fileContent.Append(_uiEnumContent);
            //替换脚本名和命名空间
            if (fileContent.ToString().Contains(obj.name))
            {
				return;
            }
			fileContent = fileContent.Replace("//--UI", "//--UI" + "\n"+ "    " + obj.name+",");
			File.WriteAllText(_UIEnmuPath, fileContent.ToString());
		}
	}
}



namespace Editor
{
	/// <summary>
	/// time:2019/6/2 0:37
	/// author:Sun
	/// des:UICodeCreate.Data
	///
	/// github:https://github.com/KingSun5
	/// csdn:https://blog.csdn.net/Mr_Sun88
	/// </summary>
	public partial class UiCodeCreate {

		/// <summary>
		/// Item模板类脚本路径
		/// </summary>
		private string _itemTemplatePath = "Assets/UIEditorTool/Template/ItemTemplate.cs";
		/// <summary>
		/// Item模板类脚本内容
		/// </summary>
		private string _itemTemplateContent
		{
			get { return GetFileContent(_itemTemplatePath); }
		}

		/// <summary>
		/// Model模板类脚本路径
		/// </summary>
		private string _modelTemplatePath = "Assets/UIEditorTool/Template/ModelTemplate.cs";
		/// <summary>
		/// Model模板类脚本内容
		/// </summary>
		private string _modelTemplateContent
		{
			get { return GetFileContent(_modelTemplatePath); }
		}
		/// <summary>
		/// View模板类脚本路径
		/// </summary>
		private string _viewTemplatePath = "Assets/UIEditorTool/Template/ViewTemplate.cs";
		/// <summary>
		/// View模板类脚本内容
		/// </summary>
		private string _viewTemplateContent
		{
			get { return GetFileContent(_viewTemplatePath); }
		}
		/// <summary>
		/// UI枚举脚本路径
		/// </summary>
		private string _UIEnmuPath = "Assets/Scripts/Utility/UIEnum.cs";
		/// <summary>
		/// UI枚举
		/// </summary>
		private string _uiEnumContent
        {
			get { return GetFileContent(_UIEnmuPath); }
		}
		/// <summary>
		/// Control模板类脚本路径
		/// </summary>
		private string _controlTemplatePath = "Assets/UIEditorTool/Template/ControlTemplate.cs";
		/// <summary>
		/// Control模板类脚本内容
		/// </summary>
		private string _controlTemplateContent
		{
			get { return GetFileContent(_controlTemplatePath); }
		}
		/// <summary>
		/// 生成UI文件路径
		/// </summary>
		private string _UiFilePath = "Assets/Scripts/UI/";

		/// <summary>
		/// 生成Item文件路径
		/// </summary>
		private string _ItemFilePath = "Assets/Scripts/UI/Item/";

	}
}

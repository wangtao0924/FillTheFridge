//using Excel;
//using System.Data;
//using System.IO;
//using UnityEngine;

//public class LevelAutoSetUp:MonoBehaviour
//{
//    private void Awake()
//    {
        
//    }
//    void SetupLevel()
//    {
//        string path = Application.dataPath.Replace("Assets", "Config");
//        DataRowCollection _dataRowCollection = ReadExcel(path + "/cs_level.xlsx");

//        for (int i = 0; i < _dataRowCollection.Count; i++)
//        {
//            Debug.Log(_dataRowCollection[i][0] + " " + _dataRowCollection[i][1] + " " + _dataRowCollection[i][2]);
//        }

//    }

//    //通过表的索引，返回一个DataRowCollection表数据对象
//    private static DataRowCollection ReadExcel(string _path, int _sheetIndex = 0)
//    {
//        FileStream stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
//       // IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
//        DataSet result = excelReader.AsDataSet();
//        return result.Tables[_sheetIndex].Rows;
//    }

//    static DataRowCollection ReadExcel(string excelName, string sheetName)
//    {
//        string path = Application.dataPath + "/" + excelName;
//        FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
//       // IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

//        DataSet result = excelReader.AsDataSet();
//        //int columns = result.Tables[0].Columns.Count;
//        //int rows = result.Tables[0].Rows.Count;

//        //tables可以按照sheet名获取，也可以按照sheet索引获取
//        //return result.Tables[0].Rows;
//        return result.Tables[sheetName].Rows;
//    }
//    static string GetSuffix(string path)
//    {
//        string ext = Path.GetExtension(path);
//        string[] arg = ext.Split(new char[] { '.' });
//        return arg[1];
//    }
//}

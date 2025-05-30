using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using NPOI.OpenXmlFormats.Spreadsheet;

public class LevelAutoSetup
{
    static int levelcount = 30;

    static ISheet _sheet = null;
    static IWorkbook _workbook = null;
    static List<BasketExcel> basket = new List<BasketExcel>();
    static List<RefrigeratorExcel> refrigerator = new List<RefrigeratorExcel>();
    static List<GoodsExcel> goods = new List<GoodsExcel>();
    static int goodtypecount = 10;
    static int Vol = 384;
    [MenuItem("关卡/随机生成", false)]
    static void AutoSetup()
    {
        ReadGoods();
        RandomCreateRefrigerator();
        RandomCreateBasket();
        ChangeRefrigerator();
        string excelPath1 = Application.dataPath.Replace("Assets", "Config") + "/cs_refrigerator.xlsx";
        SaveData(excelPath1);
        ChangeBasket();
        string excelPath2 = Application.dataPath.Replace("Assets", "Config") + "/basket..xlsx";
        SaveData(excelPath2);

        basket.Clear();
        refrigerator.Clear();
        goods.Clear();
    }
    static void ChangeBasket()
    {
        string basketPath = Application.dataPath.Replace("Assets", "Config") + "/basket..xlsx";
        CreateSheet(basketPath);
        for (int col = 0; col <2; col++)
        {
            for (int i = 4; i < 4 + basket.Count; i++)
            {
                XSSFRow row = _sheet.GetRow(i + 1) as XSSFRow;
                if (row != null)
                {
                    ICell cell = row.GetCell(col, MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    if (col == 0)
                    {
                        if (cell != null)
                        {
                            cell.SetCellValue(basket[i - 4].id);
                        }
                        else
                        {
                            cell = row.CreateCell(col);
                            cell.SetCellValue(basket[i - 4].id);
                        }
                    }
                    else
                    {
                        string asa = string.Empty;
                        for (int p = 0; p < basket[i - 4].basketInfo.Count; p++)
                        {
                            asa += basket[i - 4].basketInfo[p].color.ToString() + "_" + basket[i - 4].basketInfo[p].goodId.ToString() + "_" + basket[i - 4].basketInfo[p].num.ToString();
                            if (p < basket[i - 4].basketInfo.Count - 1)
                            {
                                asa += ";";
                            }
                        }
                        if (cell != null)
                        {
                            cell.SetCellValue(asa);
                        }
                        else
                        {
                            cell = row.CreateCell(col);
                            cell.SetCellValue(asa);
                        }
                    }
                }
                else
                {
                    IRow rowX = _sheet.CreateRow(i + 1);
                    ICell cell = rowX.CreateCell(col);
                    if (col == 0)
                    {
                        cell.SetCellValue(basket[i - 4].id);
                    }
                    else
                    {
                        string asa = string.Empty;
                        for (int p = 0; p < basket[i - 4].basketInfo.Count; p++)
                        {
                            asa += basket[i - 4].basketInfo[p].color.ToString() + "_" + basket[i - 4].basketInfo[p].goodId.ToString() + "_" + basket[i - 4].basketInfo[p].num.ToString();
                            if (p < basket[i - 4].basketInfo.Count - 1)
                            {
                                asa += ";";
                            }
                        }
                        cell.SetCellValue(asa);
                    }
                }
            }
        }
    }
    static void ChangeRefrigerator()
    {
        string RefrigeratorPath = Application.dataPath.Replace("Assets", "Config") + "/cs_refrigerator.xlsx";
        CreateSheet(RefrigeratorPath);
        for (int col = 0; col < 3; col++)
        {
            for (int i = 4; i < 4+ refrigerator.Count; i++)
            {
                XSSFRow row = _sheet.GetRow(i + 1) as XSSFRow;
                if (row != null)
                {
                    ICell cell = row.GetCell(col, MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    if (col == 0)
                    {                      
                        if (cell!=null)
                        {
                            cell.SetCellValue(refrigerator[i - 4].id);
                        }
                        else
                        {
                            cell = row.CreateCell(col);
                            cell.SetCellValue(refrigerator[i - 4].id);
                        }
                    }
                    else if (col == 1)
                    {
                        if (cell != null)
                        {
                            cell.SetCellValue(refrigerator[i - 4].path);
                        }
                        else
                        {
                            cell = row.CreateCell(col);
                            cell.SetCellValue(refrigerator[i - 4].path);
                        }
                    }
                    else
                    {
                        string v = string.Empty;
                        for (int k = 0; k < refrigerator[i - 4].plat.Count; k++)
                        {
                            v += refrigerator[i - 4].plat[k].ToString();
                            if (k < refrigerator[i - 4].plat.Count - 1)
                            {
                                v += ";";
                            }
                        }
                        if (cell != null)
                        {
                            cell.SetCellValue(v);
                        }
                        else
                        {
                            cell = row.CreateCell(col);
                            cell.SetCellValue(v);
                        }
                    }
                }
                else
                {
                    IRow rowX = _sheet.CreateRow(i + 1);
                    ICell cell= rowX.CreateCell(col);
                    if (col == 0)
                    {
                        cell.SetCellValue(refrigerator[i - 4].id);
                    }
                    else if (col == 1)
                    {
                        cell.SetCellValue(refrigerator[i - 4].path);
                    }
                    else
                    {
                        string v = string.Empty;
                        for (int k = 0; k < refrigerator[i - 4].plat.Count; k++)
                        {
                            v += refrigerator[i - 4].plat[k].ToString();
                            if (k < refrigerator[i - 4].plat.Count - 1)
                            {
                                v += ";";
                            }
                        }
                        cell.SetCellValue(v);
                    }
                    
                }
            }
        }
    }
    static void SaveData(string excelPath)
    {
        using (FileStream file = new FileStream(excelPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        {
            if (file == null)
            {
                Debug.LogError("路径为:" + excelPath + "的文件不存在");
                return;
            }
            _workbook.Write(file);
            file.Close();
        }
    }
    static void RandomCreateBasket()
    {

        for (int i = 0; i < levelcount; i++)
        {
            int curVol = Vol * refrigerator[i].plat.Count;
            BasketExcel ex = new BasketExcel();
            ex.id = i + 1;
            int count;
            if (refrigerator[i].plat.Count == 2)
            {
                count = Random.Range(3, 5);
            }
            else if (refrigerator[i].plat.Count == 3)
            {
                count = Random.Range(5, 7);
            }
            else
            {
                count = Random.Range(6, 9);
            }
            ex.basketInfo = new List<BasketInfo>();
            List<int> existGoods = new List<int>();
            List<(int, int)> values = new List<(int, int)>();
            for (int j = 0; j < count; j++)
            {
                BasketInfo info = new BasketInfo();
                info.color = Random.Range(1, 4);
                int goodtype = Random.Range(1, goodtypecount + 1);
                List<int> list = GetGooid(goodtype, existGoods);
                while (list.Count == 0)
                {
                    goodtype = Random.Range(1, goodtypecount + 1);
                    list = GetGooid(goodtype, existGoods);
                }
                int acount = list.Count;
                info.goodId = list[Random.Range(0, list.Count)];

                ex.basketInfo.Add(info);
                existGoods.Add(info.goodId);
                values.Add((info.goodId, goods.Find(x => x.id == info.goodId).V));
            }
            bool start = true;
            int co = 0;
            ex.basketInfo.Sort((x, y) => -x.goodId.CompareTo(y.goodId));
            values.Sort((x, y) => -x.Item1.CompareTo(y.Item1));
            int per = curVol / values.Count;
            while (start)
            {
                co++;
                if (co > 100000)
                {
                    start = false;
                    Debug.LogError($"请舍弃掉当前id为:{i + 1}的数据");
                }

                int all = 0;
                for (int k = 0; k < values.Count; k++)
                {
                    //int num;
                    //int vvv;
                    if (k == values.Count - 1)
                    {
                        int v = (curVol - all) / values[k].Item2;
                        int y = v % 4;
                        if (y != 0 /*|| v >= 28*/)
                        {
                            break;
                        }
                        else
                        {
                            ex.basketInfo[k].num = v;
                            all += v * values[k].Item2;
                            break;
                        }
                    }
                    //int bv=0;
                    //bv += refrigerator[i].plat.Count-2;
                    //if (values[k].Item2==6)
                    //{

                    //    bv += Random.Range(1, 4);
                    //    bv = bv * 2;
                    //}
                    //if (values[k].Item2==8)
                    //{
                    //    bv += Random.Range(1,4);
                    //    bv = bv * 2;
                    //}
                    //else if (values[k].Item2 == 10)
                    //{
                    //    bv += Random.Range(1, 3);
                    //    bv = bv * 2;
                    //}
                    //else if(values[k].Item2 ==16 )
                    //{
                    //     bv += Random.Range(1, 3);
                    //    bv = bv * 2;
                    //}
                    //else if (values[k].Item2 == 20)
                    //{
                    //     bv += Random.Range(1, 3);
                    //    bv = bv * 2;
                    //}
                    //else if (values[k].Item2 == 36)
                    //{
                    //     bv = Random.Range(2, 4);
                    //}
                    //else if (values[k].Item2 == 42)
                    //{
                    //     bv = Random.Range(1, 3);
                    //}
                    //else
                    //{
                    //     bv = Random.Range(1, 2);
                    //}
                    //num = bv * 4;
                    //vvv = num * values[k].Item2;
                    //ex.basketInfo[k].num = num;
                    //all += vvv;
                    int num = 0;
                    if (values[k].Item2 <= 20)
                    {
                        num = per / values[k].Item2;
                        num = num / 4;
                        if (num != 0)
                        {
                            num = num * 4;
                            //if (num/4<2)
                            //{
                            //    num += 4 * Random.Range(-1, 1);
                            //}
                            //else
                            //{
                            //    num += 4 * Random.Range(-2, 2);
                            //}
                            num += 4 * Random.Range(-2, 2);
                        }
                        else
                        {
                            num = 4;
                        }

                    }
                    else
                    {
                        num = per / values[k].Item2;
                        num = num / 4;
                        if (num != 0)
                        {
                            num = num * 4;
                        }
                        else
                        {
                            num = 4;
                        }
                    }


                    int vvv = num * values[k].Item2;
                    ex.basketInfo[k].num = num;
                    all += vvv;
                }
                if (all == curVol)
                {
                    start = false;
                }
                //else
                //{
                //    ex.basketInfo.Clear();
                //}
            }
            basket.Add(ex);
        }
    }
    static List<int> GetGooid(int goodtype, List<int> existGoods)
    {
        List<int> list = new List<int>();
        foreach (var item in goods)//有可能种类没那么多，暂时不管
        {
            if (item.id / 100 == goodtype)
            {
                if (existGoods.Contains(item.id))
                {
                    continue;
                }
                list.Add(item.id);
            }
        }
        return list;
    }
    static void RandomCreateRefrigerator()
    {
        for (int i = 0; i < levelcount; i++)
        {
            RefrigeratorExcel ex = new RefrigeratorExcel();
            ex.id = i + 1;
            int count = 0;
            int v = UnityEngine.Random.Range(0, 100);
            if (v < 20)
            {
                count = 2;
                ex.path = "Assets/Res/Art/prefab/Refrigerator/Level_1.prefab";
            }
            else if (v < 60)
            {
                count = 3;
                ex.path = "Assets/Res/Art/prefab/Refrigerator/Level_2.prefab";
            }
            else
            {
                count = 4;
                ex.path = "Assets/Res/Art/prefab/Refrigerator/Level_3.prefab";
            }
            ex.plat = new List<int>();
            for (int j = 0; j < count; j++)
            {
                int type = UnityEngine.Random.Range(1, 4);
                ex.plat.Add(type);
            }
            refrigerator.Add(ex);
        }
        foreach (var item in refrigerator)
        {
            //Debug.Log("冰箱表" + item.id + "  " + item.path + "   " + item.plat.Count);
        }
    }
    static void ReadGoods()
    {
        string goodsPath = Application.dataPath.Replace("Assets", "Config") + "/cs_goods.xlsx";
        CreateSheet(goodsPath);
        for (int col = 0; col < 3; col++)
        {
            if (col == 1) continue;
            for (int i = 4; i < _sheet.LastRowNum + 1; i++)
            {
                XSSFRow row = _sheet.GetRow(i + 1) as XSSFRow;
                if (row != null)
                {
                    ICell cell = row.GetCell(col, MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    if (cell != null)
                    {
                        if (col == 0)
                        {
                            GoodsExcel tm = new GoodsExcel();
                            goods.Add(tm);
                            tm.id = int.Parse(cell.ToString());
                        }
                        if (col == 2)
                        {
                            goods[i - 4].info = cell.ToString();
                            string[] tmp = goods[i - 4].info.Split('|');
                            int v = 1;
                            foreach (var item in tmp)
                            {
                                int a = int.Parse(item);
                                v = v * a;
                            }
                            goods[i - 4].V = v;
                        }
                    }
                }
            }
        }
        foreach (var item in goods)
        {
            // Debug.Log("物品表"+item.id+"  "+item.info+"   "+item.V);
        }
    }
    static void SetSheet(ISheet sheet)
    {
        for (int col = 0; col < 4; col++)
        {
            if (col == 1) continue;
            for (int i = 0; i < _sheet.LastRowNum + 1; i++)
            {
                XSSFRow row = _sheet.GetRow(i + 1) as XSSFRow;
                if (row != null)
                {
                    ICell cell = row.GetCell(col, MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    if (cell != null)
                    {
                        Debug.LogError(cell.ToString());
                    }
                }
            }
        }
    }
    static void CreateSheet(string excelPath)
    {
        try
        {
            using (FileStream fileStream = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                string extension = GetSuffix(excelPath);
                if (extension == "xls")
                    _workbook = new HSSFWorkbook(fileStream);
                else if (extension == "xlsx")
                {
                    _workbook = new XSSFWorkbook(fileStream);
                }
                else
                {
                    throw new Exception("Wrong file.");
                }
                _sheet = _workbook.GetSheetAt(0);
                if (_sheet == null)
                {
                    Debug.LogError("未读取到sheetName");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    static string GetSuffix(string path)
    {
        string ext = Path.GetExtension(path);
        string[] arg = ext.Split(new char[] { '.' });
        return arg[1];
    }
    void SaveSheet(string excelPath)
    {
        using (FileStream file = new FileStream(excelPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        {
            if (file == null)
            {
                Debug.LogError("路径为:" + excelPath + "的文件不存在");
                return;
            }
            _workbook.Write(file);
            file.Close();
        }
    }
}
public class BasketExcel
{
    public int id;
    public List<BasketInfo> basketInfo = new List<BasketInfo>();
}
public class BasketInfo
{
    public int color;
    public int goodId;
    public int num;
}
public class RefrigeratorExcel
{
    public int id;
    public string path;
    public List<int> plat = new List<int>();
}
public class GoodsExcel
{
    public int id;
    public int V;
    public string info;
}

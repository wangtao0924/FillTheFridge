/*
 *   This file was generated by a tool.
 *   Do not edit it, otherwise the changes will be overwritten.
 */

using System;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MPStudio;
using ExcelDataClass;

[Serializable]
public class DataManager : MPSingletonMono<DataManager>
{
	public BasketData p_BasketData;
	public Goods p_Goods;
	public Language p_Language;
	public Level p_Level;
	public RefrigeratorData p_RefrigeratorData;

	public BasketDataItem GetBasketDataItemByID(Int32 id)
	{
		BasketDataItem t = null;
		p_BasketData.Dict.TryGetValue(id, out t);
		if (t == null) Debug.LogError("can't find the id " + id + " in BasketData");
		return t;
	}

	public GoodsItem GetGoodsItemByID(Int32 id)
	{
		GoodsItem t = null;
		p_Goods.Dict.TryGetValue(id, out t);
		if (t == null) Debug.LogError("can't find the id " + id + " in Goods");
		return t;
	}

	public LanguageItem GetLanguageItemByID(Int32 id)
	{
		LanguageItem t = null;
		p_Language.Dict.TryGetValue(id, out t);
		if (t == null) Debug.LogError("can't find the id " + id + " in Language");
		return t;
	}

	public LevelItem GetLevelItemByID(Int32 id)
	{
		LevelItem t = null;
		p_Level.Dict.TryGetValue(id, out t);
		if (t == null) Debug.LogError("can't find the id " + id + " in Level");
		return t;
	}

	public RefrigeratorDataItem GetRefrigeratorDataItemByID(Int32 id)
	{
		RefrigeratorDataItem t = null;
		p_RefrigeratorData.Dict.TryGetValue(id, out t);
		if (t == null) Debug.LogError("can't find the id " + id + " in RefrigeratorData");
		return t;
	}

	public void LoadAll()
	{
		p_BasketData = Load("BasketData") as BasketData;
		p_Goods = Load("Goods") as Goods;
		p_Language = Load("Language") as Language;
		p_Level = Load("Level") as Level;
		p_RefrigeratorData = Load("RefrigeratorData") as RefrigeratorData;
	}

	private System.Object Load(string name)
	{
		IFormatter f = new BinaryFormatter();
		TextAsset text = Resources.Load<TextAsset>("BinConfigData/" + name);
		Stream s = new MemoryStream(text.bytes);
		System.Object obj = f.Deserialize(s);
		s.Close();
		return obj;
	}
}


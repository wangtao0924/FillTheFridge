using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using MPStudio;


//窗体的层级
public enum E_UIRootType
{
    Back, //背景

    Default, //默认

    Pop, //弹窗

    Message, //消息、飘字

    Top, //顶层
}

//窗体的显示方式
public enum E_ShowUIMode
{
    //  窗体显示出来的时候，不会去隐藏任何窗体
    DoNothing,

    //  窗体显示出来的时候,会隐藏掉所有的普通窗体，但是不会隐藏保持在最前方的窗体
    HideOther,

    //  窗体显示出来的时候,会隐藏所有的窗体，不管是普通的还是保持在最前方的
    HideAll
}

public class GameDefine
{
    //  导入UI预制体文件（名字，路径）
    public static Dictionary<E_UiId, string> dicPath = new Dictionary<E_UiId, string>();

    public static void InitUIDictionary()
    {
        string path = MPAppConst.UIPrefabPath;
        foreach (var item in Enum.GetValues(typeof(E_UiId)))
        {
            if ((E_UiId) item == E_UiId.NullUI) continue;
            dicPath[(E_UiId) item] = string.Format(path, ((E_UiId) item).ToString());
        }
    }

    public static Type GetTypeByString(E_UiId em)
    {
        Type t = Type.GetType(em.ToString());
        Type a = typeof(BaseUI);
        if (a.IsAssignableFrom(t))
        {
            return t;
        }

        Debug.LogError("UI脚本没有继承BaseUI!!---" + a.Name);
        return null;
    }
}

public class EventConst
{
    public const string HeroChangeState = "HeroChangeState";

    public const string BattleFlowChange = "BattleFlowChange";

    public const string HeroItemSelect = "HeroItemSelect";

    public const string SkillBookItemSelect = "SkillBookItemSelect";

    public const string SkillItemSelect = "SkillItemSelect";

    public const string UpdateSkillInfoItem = "UpdateSkillInfoItem";

    public const string UpdateSelectedSkillBook = "UpdateSelectedSkillBook";

    public const string UpdateHeroInfoItem = "UpdateHeroInfoItem";

    public const string SkillBookPropItemSelect = "UpdatePropItemSelect";

    public const string SkillDetailItemSelect = "SkillDetailItemSelect";

    public const string TalentItemSelect = "TalentItemSelect";
}
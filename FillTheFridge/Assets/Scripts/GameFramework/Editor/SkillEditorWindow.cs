using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationClipViewer : EditorWindow
{
    [UnityEditor.MenuItem("动画/技能工具", false)]
    public static void ShowWindow()
    {
        AnimationClipViewer animationClipViewer = EditorWindow.GetWindow<AnimationClipViewer>(true, "动画预览工具");
        animationClipViewer.position = new Rect(300, 200, 800, 600);
    }
    Vector2 scrollPos = new Vector2();

    private AnimationClip clip;
    private static Dictionary<GameObject, AnimationClip[]> clipsDic = new Dictionary<GameObject, AnimationClip[]>();
    private static Dictionary<GameObject, int> clipIndexDic = new Dictionary<GameObject, int>();
    private static float m_SliderValue = 0f, m_SliderValue1 = 0;
    private static bool loop = false;
    private static bool isPlaying = false;
    private static bool autoPlay = false;
    private static float startTime = 0;
    private static float speed = 1f;
    private static float length = 0f;

    private static int skillID;
    private static bool isCombo;
    private static bool isCollide;
    private static bool isBreak;

    private static int curSkillActionIndex;
    private static int curSkillMoveIndex;
    private static int curSkillShowIndex;

    private SerializedObject skillSerializedObject;
    public List<SkillActionConfig> skillActionConfigs = new List<SkillActionConfig>();
    public List<SkillMoveConfig> skillMoveConfigs = new List<SkillMoveConfig>();
    public List<SkillShowConfig> skillShowConfigs = new List<SkillShowConfig>();
    private SerializedProperty skillActionsProperty;
    private SerializedProperty skillMovesProperty;
    private SerializedProperty skillShowsProperty;

    private void OnEnable()
    {
        skillSerializedObject = new SerializedObject(this);
        skillActionsProperty = skillSerializedObject.FindProperty("skillActionConfigs");
        skillMovesProperty = skillSerializedObject.FindProperty("skillMoveConfigs");
        skillShowsProperty = skillSerializedObject.FindProperty("skillShowConfigs");
    }

    void OnInspectorUpdate() { }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(position.width), GUILayout.Height(position.height));
        EditorGUILayout.LabelField("选择预制体", new[] { GUILayout.Height(20), GUILayout.Width(500) });
        if (GUILayout.Button("重新载入动画", new[] { GUILayout.Height(20), GUILayout.Width(80) }))
        {
            clipsDic.Clear();
            clipIndexDic.Clear();
            if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
            {
                GameObject[] gos = Selection.gameObjects;
                foreach (var go in gos)
                {
                    Animation[] animations = go.GetComponentsInChildren<Animation>(true);
                    if (animations != null && animations.Length > 0)
                    {
                        foreach (var animation in animations)
                        {
                            List<AnimationClip> clips = new List<AnimationClip>();
                            foreach (AnimationState _state in animation)
                            {
                                clips.Add(animation.GetClip(_state.name));
                                Debug.Log(_state.name);
                            }
                            clipsDic.Add(animation.gameObject, clips.ToArray());
                            clipIndexDic.Add(animation.gameObject, 0);
                        }
                    }
                    Animator[] animators = go.GetComponentsInChildren<Animator>(true);
                    if (animators != null && animators.Length > 0)
                    {
                        foreach (var animator in animators)
                        {
                            AnimatorController controller = (AnimatorController)animator.runtimeAnimatorController;
                            clipsDic.Add(animator.gameObject, controller.animationClips);
                            clipIndexDic.Add(animator.gameObject, 0);
                        }
                    }
                }
            }
        }
        if (clipsDic.Count == 0) return;
        EditorGUI.BeginChangeCheck();
        foreach (var kvp in clipsDic)
        {
            int selectIndex = clipIndexDic[kvp.Key];

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(kvp.Key.name, new[] { GUILayout.Height(20), GUILayout.Width(100) });
            selectIndex = EditorGUILayout.Popup("选择动画：", selectIndex, kvp.Value.Select(pkg => pkg.name).ToArray(), new[] { GUILayout.Height(20), GUILayout.Width(400) });
            clip = kvp.Value[selectIndex];
            float time = clip.length * m_SliderValue;
            if (clip.length > length)//取最长的
                length = clip.length;
            EditorGUILayout.LabelField($"长度：{time}/{clip.length}s", new[] { GUILayout.Height(20), GUILayout.Width(300) });
            clipIndexDic[kvp.Key] = selectIndex;
            EditorGUILayout.EndHorizontal();
        }
        m_SliderValue = EditorGUILayout.Slider("播放进度", m_SliderValue, 0f, 1f, new[] { GUILayout.Height(20), GUILayout.Width(800) });
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        speed = EditorGUILayout.Slider("倍速", speed, 0f, 10f, new[] { GUILayout.Height(20), GUILayout.Width(400) });
        if (GUILayout.Button("正常", new[] { GUILayout.Height(20), GUILayout.Width(80) }))
        {
            speed = 1f;
        }
        EditorGUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck())
        {
            autoPlay = false;
            foreach (var kvp in clipsDic)
            {
                int selectIndex = clipIndexDic[kvp.Key];
                AnimationClip clip = kvp.Value[selectIndex];
                float time = clip.length * m_SliderValue;
                clip.SampleAnimation(kvp.Key, time);
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        loop = GUILayout.Toggle(loop, "循环", new[] { GUILayout.Height(20), GUILayout.Width(200) });
        if (isPlaying)
        {
            if (GUILayout.Button("停止", new[] { GUILayout.Height(20), GUILayout.Width(100) }))
            {
                autoPlay = false;
                isPlaying = false;
            }
        }
        else
        {
            if (GUILayout.Button("播放", new[] { GUILayout.Height(20), GUILayout.Width(100) }))
            {
                m_SliderValue = 0f;
                isPlaying = true;
                autoPlay = true;
                startTime = Time.realtimeSinceStartup;
                Debug.Log(startTime);
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        skillID = EditorGUILayout.IntField("技能ID", skillID, new[] { GUILayout.Height(20), GUILayout.Width(250) });
        EditorGUILayout.Space();
        isCombo = GUILayout.Toggle(isCombo, "是否是Combo技能", new[] { GUILayout.Height(20), GUILayout.Width(200) });
        EditorGUILayout.Space();
        isCollide = GUILayout.Toggle(isCollide, "是否无视地形", new[] { GUILayout.Height(20), GUILayout.Width(200) });
        EditorGUILayout.Space();
        isBreak = GUILayout.Toggle(isBreak, "技能是否能被打断", new[] { GUILayout.Height(20), GUILayout.Width(200) });
        EditorGUILayout.Space();
        GUISkillActionArea();
        EditorGUILayout.Space();
        GUISkillMoveArea();
        EditorGUILayout.Space();
        GUISkillShowArea();
        EditorGUILayout.Space();
        if (GUILayout.Button("将数据写入表格", new[] { GUILayout.Height(20), GUILayout.Width(100) }))
        {
            SaveSkillConfigsDate();
            SaveSkillActionConfigsData();
            SaveSkillMoveConfigsData();
            SaveSkillShowConfigsData();
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.EndScrollView();
        if (autoPlay)
        {
            float diff = Time.realtimeSinceStartup - startTime;
            diff *= speed;
            m_SliderValue = diff / length;
            foreach (var kvp in clipsDic)
            {
                int selectIndex = clipIndexDic[kvp.Key];
                AnimationClip clip = kvp.Value[selectIndex];
                float time = clip.length * m_SliderValue;
                clip.SampleAnimation(kvp.Key, time);
            }
            if (diff >= length)
            {
                startTime = Time.realtimeSinceStartup;
                if (!loop)
                {
                    autoPlay = false;
                    isPlaying = false;
                }
            }
        }
        Repaint();
    }


    private void GUISkillActionArea()
    {
        EditorGUILayout.PropertyField(skillActionsProperty, true);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加技能Action", new[] { GUILayout.Height(20), GUILayout.Width(150) }))
        {
            skillActionsProperty.arraySize += 1;
            curSkillActionIndex = skillActionsProperty.arraySize;
            skillSerializedObject.ApplyModifiedProperties();
        }
        curSkillActionIndex = EditorGUILayout.IntField("当前选择的技能Action", curSkillActionIndex, new[] { GUILayout.Height(20), GUILayout.Width(250) });
        if (GUILayout.Button("记录动画Action时间", new[] { GUILayout.Height(20), GUILayout.Width(150) }))
        {
            if (skillActionsProperty.arraySize >= curSkillActionIndex)
            {
                SerializedProperty skillAction = skillActionsProperty.GetArrayElementAtIndex(skillActionsProperty.arraySize - 1);
                float time = clip.length * m_SliderValue;
                if (clip.length > length)
                    length = clip.length;
                skillAction.FindPropertyRelative("delayTime").intValue = (int)(time * 1000);
            }
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("删除技能Action", new[] { GUILayout.Height(20), GUILayout.Width(150) }))
        {
            if (skillActionsProperty.arraySize > 0)
            {
                skillActionsProperty.arraySize -= 1;
                curSkillActionIndex = skillActionsProperty.arraySize;
                skillSerializedObject.ApplyModifiedProperties();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void GUISkillMoveArea()
    {
        EditorGUILayout.PropertyField(skillMovesProperty, true);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加技能Move", new[] { GUILayout.Height(20), GUILayout.Width(150) }))
        {
            skillMovesProperty.arraySize += 1;
            curSkillMoveIndex = skillMovesProperty.arraySize;
            skillSerializedObject.ApplyModifiedProperties();
        }
        curSkillMoveIndex = EditorGUILayout.IntField("当前选择的技能Move", curSkillMoveIndex, new[] { GUILayout.Height(20), GUILayout.Width(250) });
        if (GUILayout.Button("记录动画Move时间", new[] { GUILayout.Height(20), GUILayout.Width(150) }))
        {
            if (skillMovesProperty.arraySize >= curSkillMoveIndex)
            {
                SerializedProperty skillMove = skillMovesProperty.GetArrayElementAtIndex(skillMovesProperty.arraySize - 1);
                float time = clip.length * m_SliderValue;
                if (clip.length > length)
                    length = clip.length;
                skillMove.FindPropertyRelative("delayTime").intValue = (int)(time * 1000);
            }
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("删除技能Move", new[] { GUILayout.Height(20), GUILayout.Width(150) }))
        {
            if (skillMovesProperty.arraySize > 0)
            {
                skillMovesProperty.arraySize -= 1;
                curSkillMoveIndex = skillMovesProperty.arraySize;
                skillSerializedObject.ApplyModifiedProperties();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void GUISkillShowArea()
    {
        EditorGUILayout.PropertyField(skillShowsProperty, true);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加技能Show", new[] { GUILayout.Height(20), GUILayout.Width(150) }))
        {
            skillShowsProperty.arraySize += 1;
            curSkillShowIndex = skillShowsProperty.arraySize;
            skillSerializedObject.ApplyModifiedProperties();
        }
        curSkillShowIndex = EditorGUILayout.IntField("当前选择的技能Show", curSkillShowIndex, new[] { GUILayout.Height(20), GUILayout.Width(250) });
        EditorGUILayout.Space();
        if (GUILayout.Button("删除技能Show", new[] { GUILayout.Height(20), GUILayout.Width(150) }))
        {
            if (skillShowsProperty.arraySize > 0)
            {
                skillShowsProperty.arraySize -= 1;
                curSkillShowIndex = skillShowsProperty.arraySize;
                skillSerializedObject.ApplyModifiedProperties();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("记录动画震动开始时间", new[] { GUILayout.Height(20), GUILayout.Width(150) }))
        {
            if (skillShowsProperty.arraySize >= curSkillShowIndex)
            {
                SerializedProperty skillShow = skillShowsProperty.GetArrayElementAtIndex(skillShowsProperty.arraySize - 1);
                float time = clip.length * m_SliderValue;
                if (clip.length > length)
                    length = clip.length;
                skillShow.FindPropertyRelative("shakeStartTime").intValue = (int)(time * 1000);
            }
        }

        if (GUILayout.Button("记录动画震动停止时间", new[] { GUILayout.Height(20), GUILayout.Width(150) }))
        {
            if (skillShowsProperty.arraySize >= curSkillShowIndex)
            {
                SerializedProperty skillShow = skillShowsProperty.GetArrayElementAtIndex(skillShowsProperty.arraySize - 1);
                float time = clip.length * m_SliderValue;
                if (clip.length > length)
                    length = clip.length;
                skillShow.FindPropertyRelative("shakeEndTime").intValue = (int)(time * 1000);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("记录动画顿帧开始时间", new[] { GUILayout.Height(20), GUILayout.Width(150) }))
        {
            if (skillShowsProperty.arraySize >= curSkillShowIndex)
            {
                SerializedProperty skillShow = skillShowsProperty.GetArrayElementAtIndex(skillShowsProperty.arraySize - 1);
                float time = clip.length * m_SliderValue;
                if (clip.length > length)
                    length = clip.length;
                skillShow.FindPropertyRelative("stopFrameStartTime").intValue = (int)(time * 1000);
            }
        }
        if (GUILayout.Button("记录动画顿帧停止时间", new[] { GUILayout.Height(20), GUILayout.Width(150) }))
        {
            if (skillShowsProperty.arraySize >= curSkillShowIndex)
            {
                SerializedProperty skillShow = skillShowsProperty.GetArrayElementAtIndex(skillShowsProperty.arraySize - 1);
                float time = clip.length * m_SliderValue;
                if (clip.length > length)
                    length = clip.length;
                skillShow.FindPropertyRelative("stopFrameDuration").intValue = (int)(time * 1000);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void SaveSkillConfigsDate()
    {
        CreateSheet(skillExcelPath);
        for (int i = 6; i < _sheet.LastRowNum; i++)
        {
            ChangeSkillExcelRow(_sheet, i);
        }
        SaveSheet(skillExcelPath);
    }

    private void SaveSkillActionConfigsData()
    {
        CreateSheet(skillActionExcelPath);
        for (int i = 6; i < _sheet.LastRowNum; i++)
        {
            ChangeSkillActionExcelRow(_sheet, i);
        }
        SaveSheet(skillActionExcelPath);
    }
    private void SaveSkillMoveConfigsData()
    {
        CreateSheet(skillMoveExcelPath);
        for (int i = 6; i < _sheet.LastRowNum; i++)
        {
            ChangeSkillMoveExcelRow(_sheet, i);
        }
        SaveSheet(skillMoveExcelPath);
    }
    private void SaveSkillShowConfigsData()
    {
        CreateSheet(skillShowExcelPath);
        for (int i = 6; i < _sheet.LastRowNum; i++)
        {
            ChangeSkillShowExcelRow(_sheet, i);
        }
        SaveSheet(skillShowExcelPath);
    }

    ISheet _sheet = null;
    IWorkbook _workbook = null;
    string skillExcelPath = System.Environment.CurrentDirectory + "/ConfigExcel/CfgSkill.xlsx";
    string skillActionExcelPath = System.Environment.CurrentDirectory + "/ConfigExcel/CfgSkillAction.xlsx";
    string skillMoveExcelPath = System.Environment.CurrentDirectory + "/ConfigExcel/CfgSkillMove.xlsx";
    string skillShowExcelPath = System.Environment.CurrentDirectory + "/ConfigExcel/CfgSkillShow.xlsx";

    void CreateSheet(string excelPath)
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

    void ChangeSkillExcelRow(ISheet sheet, int index)
    {
        XSSFRow row = sheet.GetRow(index - 1) as XSSFRow;
        if (row != null)
        {
            ICell cell = row.GetCell(0, MissingCellPolicy.RETURN_NULL_AND_BLANK);
            if (cell != null)
            {
                if (int.TryParse(cell.ToString(), out int id))
                {
                    if (id == skillID)
                    {
                        if (isCombo)
                        {
                            row.GetCell(5, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(1);
                        }
                        else
                        {
                            row.GetCell(5, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(0);
                        }
                        if (isCollide)
                        {
                            row.GetCell(6, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(1);
                        }
                        else
                        {
                            row.GetCell(6, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(0);
                        }
                        if (isBreak)
                        {
                            row.GetCell(7, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(1);
                        }
                        else
                        {
                            row.GetCell(7, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(0);
                        }

                        string skillMoveLst = "";
                        for (int i = 0; i < skillMovesProperty.arraySize; i++)
                        {
                            SerializedProperty skillMove = skillMovesProperty.GetArrayElementAtIndex(i);
                            int moveId = skillMove.FindPropertyRelative("id").intValue;
                            skillMoveLst += moveId;
                            if (i < skillMovesProperty.arraySize - 1)
                            {
                                skillMoveLst += "|";
                            }
                        }
                        row.GetCell(9, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(skillMoveLst);
                        string skillActionLst = "";
                        for (int i = 0; i < skillActionsProperty.arraySize; i++)
                        {
                            SerializedProperty skillAction = skillActionsProperty.GetArrayElementAtIndex(i);
                            int actionId = skillAction.FindPropertyRelative("id").intValue;
                            skillActionLst += actionId;
                            if (i < skillMovesProperty.arraySize - 1)
                            {
                                skillActionLst += "|";
                            }
                        }
                        row.GetCell(10, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(skillActionLst);
                        string skillShowLst = "";
                        for (int i = 0; i < skillShowsProperty.arraySize; i++)
                        {
                            SerializedProperty skillShow = skillShowsProperty.GetArrayElementAtIndex(i);
                            int showId = skillShow.FindPropertyRelative("id").intValue;
                            skillShowLst += showId;
                            if (i < skillMovesProperty.arraySize - 1)
                            {
                                skillShowLst += "|";
                            }
                        }
                        row.GetCell(11, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(skillShowLst);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("读取表格数据异常");
        }
    }

    void ChangeSkillActionExcelRow(ISheet sheet, int index)
    {
        XSSFRow row = sheet.GetRow(index - 1) as XSSFRow;
        if (row != null)
        {
            ICell cell = row.GetCell(0, MissingCellPolicy.RETURN_NULL_AND_BLANK);
            if (cell != null)
            {
                if (int.TryParse(cell.ToString(), out int id))
                {
                    for (int i = 0; i < skillActionsProperty.arraySize; i++)
                    {
                        SerializedProperty skillAction = skillActionsProperty.GetArrayElementAtIndex(i);
                        int actionId = skillAction.FindPropertyRelative("id").intValue;
                        if (id == actionId)
                        {
                            int delayTime = skillAction.FindPropertyRelative("delayTime").intValue;
                            row.GetCell(1, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(delayTime);
                            float radius = skillAction.FindPropertyRelative("radius").floatValue;
                            row.GetCell(2, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(radius);
                            int angle = skillAction.FindPropertyRelative("angle").intValue;
                            row.GetCell(3, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(angle);
                            int damage = skillAction.FindPropertyRelative("damage").intValue;
                            row.GetCell(4, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(damage);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("读取表格数据异常");
        }
    }

    void ChangeSkillMoveExcelRow(ISheet sheet, int index)
    {
        XSSFRow row = sheet.GetRow(index - 1) as XSSFRow;
        if (row != null)
        {
            ICell cell = row.GetCell(0, MissingCellPolicy.RETURN_NULL_AND_BLANK);
            if (cell != null)
            {
                if (int.TryParse(cell.ToString(), out int id))
                {
                    for (int i = 0; i < skillMovesProperty.arraySize; i++)
                    {
                        SerializedProperty skillMove = skillMovesProperty.GetArrayElementAtIndex(i);
                        int moveId = skillMove.FindPropertyRelative("id").intValue;
                        if (id == moveId)
                        {
                            int delayTime = skillMove.FindPropertyRelative("delayTime").intValue;
                            row.GetCell(1, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(delayTime);
                            int moveTime = skillMove.FindPropertyRelative("moveTime").intValue;
                            row.GetCell(2, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(moveTime);
                            float moveDis = skillMove.FindPropertyRelative("moveDis").floatValue;
                            row.GetCell(3, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(moveDis);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("读取表格数据异常");
        }
    }

    void ChangeSkillShowExcelRow(ISheet sheet, int index)
    {
        XSSFRow row = sheet.GetRow(index - 1) as XSSFRow;
        if (row != null)
        {
            ICell cell = row.GetCell(0, MissingCellPolicy.RETURN_NULL_AND_BLANK);
            if (cell != null)
            {
                if (int.TryParse(cell.ToString(), out int id))
                {
                    for (int i = 0; i < skillShowsProperty.arraySize; i++)
                    {
                        SerializedProperty skillShow = skillShowsProperty.GetArrayElementAtIndex(i);
                        int showId = skillShow.FindPropertyRelative("id").intValue;
                        if (id == showId)
                        {
                            int shakeStartTime = skillShow.FindPropertyRelative("shakeStartTime").intValue;
                            row.GetCell(4, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(shakeStartTime);
                            int shakeEndTime = skillShow.FindPropertyRelative("shakeEndTime").intValue;
                            row.GetCell(5, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(shakeEndTime);
                            float stopFrameStartTime = skillShow.FindPropertyRelative("stopFrameStartTime").intValue;
                            row.GetCell(6, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(stopFrameStartTime);
                            float stopFrameDuration = skillShow.FindPropertyRelative("stopFrameDuration").intValue;
                            row.GetCell(7, MissingCellPolicy.RETURN_NULL_AND_BLANK).SetCellValue(stopFrameDuration);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("读取表格数据异常");
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
class ConfigData
{
    public string Type;//数据类型
    public string Name;//字段名
    public string Data;//数据值
}
[System.Serializable]
public class SkillActionConfig
{
    public int id;
    public int delayTime;
    public float radius;
    public int angle;
    public int damage;
}
[System.Serializable]
public class SkillMoveConfig
{
    public int id;
    public int delayTime;
    public int moveTime;
    public float moveDis;
}
[System.Serializable]
public class SkillShowConfig
{
    public int id;
    public int shakeStartTime;
    public int shakeEndTime;
    public int stopFrameStartTime;
    public int stopFrameDuration;
}
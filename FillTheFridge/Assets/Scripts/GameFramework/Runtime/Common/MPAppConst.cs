using System;

namespace MPStudio
{
    public class MPAppConst
    {
        public static bool kIsDebug = true;

        // 测试服
        public static string kDebugServerHost = "https://api.test.meta-originx.com/api/v1";
        public static string kReleaseServerHost = "https://api.test.meta-originx.com/api/v1";
        // 内网
        // public static string kReleaseServerHost ="http://192.168.2.207:62211/api/v1";
        // public static string kDebugServerHost = "http://192.168.2.133:62211/api/v1";

        public const string kSocketAddress = "";
        public const int kSocketPort = 80;

        // 服务器接口
        public const string LoginAPI = "/user/login"; // 登陆接口
        public const string UserDataAPI = "/user"; // 获取用户信息接口

        public const string PropAPI = "/prop?prop_types={0}"; // 道具查询接口

        public const string BattlePrepare = "/battle/prepare";//战斗准备

        public const string Battle = "/battle/{0}";//战斗

        public const string StartBattle = "/battle";

        public const string Barrier = "/barrier/scene/{0}/barrier/{1}";//查询当前关卡信息

        public const string StoryPass = "/barrier/scene/{0}/max/pass/barrier";//查询当前已通关的关卡id

        public const string AllHeroInfo = "/hero/all/heros";//查询拥有英雄
        
        public const string HeroInfo = "/hero/{0}"; //查询英雄信息

        public const string ComposeSkillBook = "/hero/skill/book"; // 技能书合成

        public const string SkillStudy = "/hero/{0}/skill/study";
        
        public const string SkillUpgrade = "/hero/{0}/skill/{1}/upgrade";

        public const string HeroSkillList = "/hero/{0}/skills"; // 英雄技能列表
        
        // 天赋接口
        public const string HeroTalent = "/hero/{0}/talent"; // 0:hero_id
        public const string HeroStudyTalent = "/hero/{0}/talent/{1}/study/{2}"; // 0:hero_id 1:talent_id 2:level

        // UI Prefab目录
        public const string UIPrefabPath = "Assets/Res/UI/UIPrefab/{0}.prefab";
        
        
        // 流程名称
        public const string GameEnterProcedureName = "GameEnter";
        public const string LoginProcedureName = "Login";
        public const string HotFixProcedureName = "HotFix";
        public const string MainPageProcedureName = "Main";
        
    }
}
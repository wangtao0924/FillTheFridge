（1）将本工具放在\Assets\Editor文件夹下
（2）\Assets\Resources\ConfigBin 内存放所有生成的2进制数据文件
（3）\Assets\Plugins\WJExcelDataClass 内存放生成的一个 dll 包含所有由excel生成的类
（4）\Assets\Scripts\ExcelDataManager 内存放生成的DataManager管理所有数据 执行LoadAll 读取所有数据
（5）excel表的sheet页签名作为每个页签对应数据的类名
（6）excel表的前三行为预留行，可作他用，不会影响读取数据
（7）第四行为数据类型，第五行为字段名，第六行及以上为数据
（8）第一列固定为ID列，数据类型可变，推荐 int32 和 int64（int32溢出的情况下使用）
（9）支持的数据类型请在 ScriptGenerator 脚本的 SupportType 类内查看并进行拓展
（10）导表前要关闭所有数据表
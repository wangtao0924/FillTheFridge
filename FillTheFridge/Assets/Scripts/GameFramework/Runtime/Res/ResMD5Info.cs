using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class ResMD5Info
{
    /// <summary>
    /// 所有Bundle清单
    /// </summary>
    public MD5Info[] Infos;
}

[Serializable]
public class MD5Info
{
    /// <summary>
    /// Bundle文件名
    /// </summary>
    public string BundleName;

    /// <summary>
    /// MD5码
    /// </summary>
    public string MD5;

    /// <summary>
    /// 尺寸
    /// </summary>
    public long Size;
}
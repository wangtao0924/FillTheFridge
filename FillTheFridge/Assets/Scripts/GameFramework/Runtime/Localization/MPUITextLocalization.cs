using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MPStudio
{
    public class MPUITextLocalization : MPUILocalization<Text>
    {
        protected override void RefreshShow()
        {
            m_Target.text = GetLocalizationData();
        }
    }
}



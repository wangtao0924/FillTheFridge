using UnityEngine;
using UnityEngine.UI;
using MPStudio;
using System.Collections;
using System.Collections.Generic;

public partial class ModelTemplate : BaseUI
{
    protected override void Awake()
    {
        base.Awake();
        //uiType.uiRootType = E_UIRootType.Default;
    }
    protected override void Start()
    {
        base.Start();
        AddClick();
    }
    public override void ShowUI()
    {
        base.ShowUI();
        RefreshShow();
    }
    void AddClick()
    {

    }
    void RefreshShow()
    {

    }
    private void OnDestroy()
    {

    }
}

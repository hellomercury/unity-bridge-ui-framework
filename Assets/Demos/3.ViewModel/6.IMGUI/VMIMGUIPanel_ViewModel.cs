///*************************************************************************************
///* 作    者：       zouhunter
///* 创建时间：       2018-11-16 04:45:36
///* 说    明：       1.部分代码自动生成///                       2.尽量使用MVVM模式///                       3.宏定义内会读成注释
///* ************************************************************************************/
///<summary>
///[代码说明信息]
///<summary>
public abstract class VMIMGUIPanel_ViewModel : BridgeUI.Binding.ViewModelObject
{
    #region 属性列表
    protected string label
    {
        get { return GetValue<string>("label"); }
        set { SetValue<string>("label", value); }
    }
    protected string title
    {
        get { return GetValue<string>("title"); }
        set { SetValue<string>("title", value); }
    }
    protected BridgeUI.Binding.PanelAction<int> on_button_click
    {
        get
        {
            return GetValue<BridgeUI.Binding.PanelAction<int>>("on_button_click");
        }
        set
        {
            SetValue<BridgeUI.Binding.PanelAction<int>>("on_button_click", value);
        }
    }
    #endregion 属性列表

}

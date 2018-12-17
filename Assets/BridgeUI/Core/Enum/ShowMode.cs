﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using BridgeUI.Model;
namespace BridgeUI
{

    //互斥关系
    public enum MutexRule
    {
        NoMutex,//不排斥
        SameParentAndLayer,//排斥同父级中的同层级
        SameLayer,  //排斥同层级
    }
    //父级显示
    public enum BaseShow
    {
        NoChange,//不改变父级状态
        Hide,//隐藏父级(在本面板关闭时打开)
        Destroy,//销毁父级(接管因为父级面关闭的面板)
    }
    /// <summary>
    /// 界面的显示状态
    /// </summary>
    [System.Serializable]
    public struct ShowMode
    {
        private const int on = 1;
        private const int off = 0;

        public bool auto { get { return _auto == on ? true : false; } set { _auto = value ? on : off; } }//当上级显示时显示
        public bool single { get { return _single == on ? true : false; } set { _single = value ? on : off; } }//隐藏所有打开的面板
        public int _auto;
        public int _single;
        public MutexRule mutex;//排斥有相同类型面版
        public BaseShow baseShow;//父级的显示状态
    }
}
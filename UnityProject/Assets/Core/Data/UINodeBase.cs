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
/// <summary>
/// ui节点信息的抽象父级
/// [记录ui打开的规则]
/// </summary>
public abstract class UINodeBase :ScriptableObject{
    public string panelName;
    public int instanceID;
    public abstract string IDName { get; }
    public UIType type;
    public int layer;
    public UnityAction<GameObject> OnCreate;
}

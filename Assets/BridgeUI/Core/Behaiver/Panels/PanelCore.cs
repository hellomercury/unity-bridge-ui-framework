﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
using BridgeUI.Model;
using System;
using UnityEngine.EventSystems;

namespace BridgeUI
{
    public class PanelCore : UIBehaviour, IUIPanel
    {
        public int InstenceID
        {
            get
            {
                return GetInstanceID();
            }
        }
        public string Name { get { return name; } }
        public IPanelGroup Group
        {
            get
            {
                if (group == null)
                    group = GetComponentInParent<IPanelGroup>();
                return group;
            }
            set { group = value; }
        }
        public IUIPanel Parent { get; set; }
        public virtual Transform Content { get { return Group.Trans; } }
        public Transform Root { get { return transform.parent.parent; } }
        public UIType UType { get; set; }
        public List<IUIPanel> ChildPanels
        {
            get
            {
                return childPanels;
            }
        }
        public bool IsShowing
        {
            get
            {
                return _isShowing && !IsDestroyed();
            }
        }
        public bool IsAlive
        {
            get
            {
                return _isAlive && !IsDestroyed();
            }
        }
        protected AnimPlayer _enterAnim;
        protected AnimPlayer enterAnim
        {
            get
            {
                if (_enterAnim == null)
                {
                    _enterAnim = Instantiate(UType.enterAnim);
                    _enterAnim.SetContext(this);
                }
                return _enterAnim;
            }
        }
        protected AnimPlayer _quitAnim;
        protected AnimPlayer quitAnim
        {
            get
            {
                if (_quitAnim == null)
                {
                    _quitAnim = Instantiate(UType.quitAnim);
                    _quitAnim.SetContext(this);
                }
                return _quitAnim;
            }
        }
        protected IPanelGroup group;
        protected Bridge bridge;
        protected List<IUIPanel> childPanels;
        public event PanelCloseEvent onDelete;
        protected event UnityAction<object> onReceive;
        protected bool _isShowing = true;
        protected bool _isAlive = true;
        protected Dictionary<int, Transform> childDic;

        #region UNITYAPI
        protected override void Start()
        {
            base.Start();
            AppendComponentsByType();
            OnOpenInternal();
            TryMakeCover();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _isAlive = false;

            _isShowing = false;

            if (bridge != null)
            {
                bridge.Release();
            }

            if (onDelete != null)
            {
                onDelete.Invoke(this, true);
            }

        }
        #endregion

        #region Interface
        public void OnRegistOnRecevie(UnityAction<object> onReceive)
        {
            this.onReceive += onReceive;
        }

        public void SetParent(Transform Trans)
        {
            Utility.SetTranform(transform, UType.layer, UType.layerIndex,group.Trans, Trans,ref childDic);
        }

        public void CallBack(object data)
        {
            if (bridge != null)
            {
                bridge.CallBack(this, data);
            }
        }

        public void HandleData(Bridge bridge)
        {
            this.bridge = bridge;
            if (bridge != null)
            {
                HandleData(bridge.dataQueue);
                bridge.onGet = HandleData;
            }
        }

        public void Hide()
        {
            _isShowing = false;
            switch (UType.hideRule)
            {
                case HideRule.AlaphGameObject:
                    AlaphGameObject(true);
                    break;
                case HideRule.HideGameObject:
                    gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        public void OnDestroyHide()
        {
            _isShowing = false;
            gameObject.SetActive(false);
            if (onDelete != null)
            {
                onDelete.Invoke(this, false);
            }
        }

        public void UnHide()
        {
            gameObject.SetActive(true);
            if (UType.hideRule == HideRule.AlaphGameObject)
            {
                AlaphGameObject(false);
            }
            _isShowing = true;
            OnOpenInternal();
        }

        public virtual void Close()
        {
            if (IsShowing && UType.quitAnim != null)
            {
                quitAnim.PlayAnim(CloseInternal);
            }
            else
            {
                CloseInternal();
            }
        }

        public void RecordChild(IUIPanel childPanel)
        {
            if (childPanels == null)
            {
                childPanels = new List<IUIPanel>();
            }
            if (!childPanels.Contains(childPanel))
            {
                childPanel.onDelete += OnRemoveChild;
                childPanels.Add(childPanel);
            }
            childPanel.Parent = this;
        }

        public Image Cover()
        {
            var covername = Name + "_Cover";
            var rectt = new GameObject(covername, typeof(RectTransform)).GetComponent<RectTransform>();
            rectt.gameObject.layer = 5;
            rectt.SetParent(transform, false);
            rectt.SetSiblingIndex(0);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 10000);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10000);
            var img = rectt.gameObject.AddComponent<Image>();
            img.color = UType.maskColor;
            img.raycastTarget = true;
            return img;
        }
        #endregion

        #region Protected

        protected void HandleData(Queue<object> dataQueue)
        {
            if (dataQueue != null)
            {
                while (dataQueue.Count > 0)
                {
                    var data = dataQueue.Dequeue();
                    if (data != null)
                    {
                        HandleData(data);
                    }
                }
            }
        }

        protected virtual void HandleData(object data)
        {
            if (this.onReceive != null)
            {
                onReceive.Invoke(data);
            }
        }

        protected void CloseInternal()
        {
            _isShowing = false;

            switch (UType.closeRule)
            {
                case CloseRule.DestroyImmediate:
                    DestroyImmediate(gameObject);
                    break;
                case CloseRule.DestroyDely:
                    Destroy(gameObject, 0.02f);
                    break;
                case CloseRule.DestroyNoraml:
                    Destroy(gameObject);
                    break;
                case CloseRule.HideGameObject:
                    OnDestroyHide();
                    break;
                default:
                    break;
            }
        }

        protected void AppendComponentsByType()
        {
            if (UType.form == UIFormType.DragAble)
            {
                if (gameObject.GetComponent<DragPanel>() == null)
                {
                    gameObject.AddComponent<DragPanel>();
                }
            }
        }

        protected void OnRemoveChild(IUIPanel childPanel, bool remove)
        {
            if (childPanels != null && childPanels.Contains(childPanel) && remove)
            {
                childPanels.Remove(childPanel);
            }
        }

        protected void OnOpenInternal()
        {
            if (UType.enterAnim != null)
            {
                enterAnim.PlayAnim(null);
            }
        }


        /// <summary>
        /// 建立遮罩
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="info"></param>
        protected void TryMakeCover()
        {
            switch (UType.cover)
            {
                case UIMask.None:
                    break;
                case UIMask.Normal:
                    Cover();
                    break;
                case UIMask.ClickClose:
                    var img = Cover();
                    var btn = img.gameObject.AddComponent<Button>();
                    btn.onClick.AddListener(Close);
                    break;
                default:
                    break;
            }
        }

        protected void AlaphGameObject(bool hide)
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            if (hide)
            {
                canvasGroup.alpha = UType.hideAlaph;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
        #endregion

        #region Extend Of Open Close
        public IUIHandle Open(string panelName, object data = null)
        {
            return UIFacade.Instence.Open(this, panelName, data);
        }

        public IUIHandle Open(int index, object data = null)
        {
            return Group.bindingCtrl.OpenRegistedPanel(this, index, data);
        }

        public void Hide(string panelName)
        {
            UIFacade.Instence.Hide(Group, panelName);
        }
        public void Hide(int index)
        {
            Group.bindingCtrl.HideRegistedPanel(this, index);
        }

        public void Close(string panelName)
        {
            UIFacade.Instence.Close(Group, panelName);
        }
        public void Close(int index)
        {
            Group.bindingCtrl.CloseRegistedPanel(this, index);
        }
        public bool IsOpen(int index)
        {
            return Group.bindingCtrl.IsRegistedPanelOpen(this, index);
        }
        public bool IsOpen(string panelName)
        {
            var panels = Group.RetrivePanels(panelName);
            return (panels != null && panels.Count > 0);
        }
        #endregion
    }
}
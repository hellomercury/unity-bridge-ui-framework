﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

namespace BridgeUI.Binding
{
    public class ViewModel : IViewModel
    {
#if BridgeUI_Log
        public static bool log { get; set; }
#endif
        private List<IBindingContext> _contexts = new List<IBindingContext>();
        public List<IBindingContext> Contexts { get { return _contexts; } }
        private readonly Dictionary<string, IBindableProperty> innerDic = new Dictionary<string, IBindableProperty>();
        protected IBindableProperty this[string key]
        {
            get
            {
                if (innerDic.ContainsKey(key))
                {
                    return innerDic[key];
                }
                return null;
            }
            set
            {
                innerDic[key] = value;
            }
        }
        public bool ContainsKey(string key)
        {
            return innerDic.ContainsKey(key);
        }
        public virtual void SetBindableProperty(string keyward, IBindableProperty value)
        {
            if (this[keyward] == null)
            {
                this[keyward] = value;
            }
            else if (this[keyward] == value)
            {
                this[keyward].ValueBoxed = value.ValueBoxed;
            }
            else
            {
                this[keyward] = value;
            }
        }
        public virtual BindableProperty<T> GetBindableProperty<T>(string keyward)
        {
            if (!ContainsKey(keyward) || !(this[keyward] is BindableProperty<T>))
            {
                this[keyward] = new BindableProperty<T>();
            }
            return this[keyward] as BindableProperty<T>;
        }
        public virtual IBindableProperty GetBindableProperty(string keyward, System.Type type)
        {
            var fullType = typeof(BindableProperty<>).MakeGenericType(type);
            if (!ContainsKey(keyward) || this[keyward].GetType() != fullType)
            {
                this[keyward] = System.Activator.CreateInstance(fullType) as IBindableProperty;
            }
            return this[keyward] as IBindableProperty;
        }
        protected virtual T GetValue<T>(string keyward)
        {
            return GetBindableProperty<T>(keyward).Value;
        }
        protected virtual void SetValue<T>(string keyward, T value)
        {
            GetBindableProperty<T>(keyward).Value = value;
        }
        public virtual void OnBinding(IBindingContext context) { this._contexts.Add(context); }
        public virtual void OnUnBinding(IBindingContext context) { this._contexts.Remove(context); }
        public virtual void Monitor<T>(string sourceName, UnityAction<T> onValueChanged)
        {
            if (!string.IsNullOrEmpty(sourceName) && onValueChanged != null)
            {
                GetBindableProperty<T>(sourceName).RegistValueChanged(onValueChanged);
            }
        }
    }

    public class ViewModelObject : ScriptableObject, IViewModel
    {
        protected ViewModel viewModel = new ViewModel();
        protected List<IBindingContext> Contexts { get { return viewModel.Contexts; } }

        public virtual bool ContainsKey(string key)
        {
            return viewModel.ContainsKey(key);
        }

        public virtual IBindableProperty GetBindableProperty(string keyward, Type type)
        {
            return viewModel.GetBindableProperty(keyward,type);
        }

        public virtual BindableProperty<T> GetBindableProperty<T>(string keyward)
        {
            return viewModel.GetBindableProperty<T>(keyward);
        }

        public virtual void OnBinding(IBindingContext context)
        {
            viewModel.OnBinding(context);
        }

        public virtual void OnUnBinding(IBindingContext context)
        {
            viewModel.OnUnBinding(context);
        }

        public virtual void SetBindableProperty(string keyward, IBindableProperty value)
        {
            viewModel.SetBindableProperty(keyward,value);
        }
        #region 
        protected virtual T GetValue<T>(string keyward)
        {
            return viewModel.GetBindableProperty<T>(keyward).Value;
        }
        protected virtual void SetValue<T>(string keyward, T value)
        {
            viewModel.GetBindableProperty<T>(keyward).Value = value;
        }
        protected virtual void Monitor<T>(string sourceName, UnityAction<T> onValueChanged)
        {
            viewModel.Monitor(sourceName, onValueChanged);
        }
        #endregion
    }

}

using System;
using System.Collections.Generic;
namespace QDuck.UI
{
    public class UIPool
    {
        private Dictionary<Type, Queue<UIPanel>> _dic;
        private UIContext _uiContext;
        private int _generateIndex = 0;
    
        public UIPool(UIContext uiContext)
        {
            _uiContext = uiContext;
            _dic = new Dictionary<Type, Queue<UIPanel>>();
        }
    
        public UIPanel Get(Type type)
        {
            UIPanel uiPanel = null;
            if (_dic.ContainsKey(type))
            {
                Queue<UIPanel> list = _dic[type];
                if (list.Count > 0)
                { 
                    uiPanel = list.Dequeue();
                }
            }
            //new T 时传入context
            if (uiPanel == null)
            {
                uiPanel = Activator.CreateInstance(type) as UIPanel;
                uiPanel.Init(_uiContext);
            }
            uiPanel.UIIndex = ++_generateIndex;
            return uiPanel;
        }
        
        
        public bool Contains(Type type)
        {
            if(_dic.TryGetValue(type, out Queue<UIPanel> list))
            {
                return list.Count > 0;
            }
            return false;
        }
    
        public bool TryRecycle(UIPanel uiPanel)
        {
            if (uiPanel.CacheCount > 0)
            {
                Type t = uiPanel.GetType();
                if (!_dic.TryGetValue(t, out Queue<UIPanel> list))
                {
                    _dic[t] = list = new Queue<UIPanel>(uiPanel.CacheCount);
                }
                if(list.Count < uiPanel.CacheCount)
                {
                    list.Enqueue(uiPanel);
                    return true;
                }
            }
            return false;
        }
    }
}
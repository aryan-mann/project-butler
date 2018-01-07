using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butler {

    public class QueueList<T> {
        
        public int MaxSize { get; private set; }
        public int Count => _nodes.Count;
        private int LastNodePosition { get; set; }

        private Dictionary<int, T> _nodes = new Dictionary<int, T>();
        public T this[int i]{
            get {
                LastNodePosition = i;
                return _nodes.Count <= i ? default(T) : _nodes[i];
            }
        }

        public T CurrentNode => LastNodePosition >= 0 && LastNodePosition <= _nodes.Count-1 ? _nodes[LastNodePosition] : default(T);
        public void SetToNextNode() {
            LastNodePosition = Clamp(LastNodePosition + 1, 0, _nodes.Count - 1);
        }
        public void SetToPreviousNode() {
            LastNodePosition = Clamp(LastNodePosition - 1, 0, _nodes.Count - 1);
        }

        public QueueList(int maxSize = 1, params T[] list) {
            MaxSize = maxSize;
            AddRange(list);
        }

        public QueueList(int maxSize = 1) { 
            MaxSize = Clamp(maxSize, 1, int.MaxValue);
        }
        
        public void Add(T item) {
            if(_nodes.Count == 0) { _nodes[0] = item; return; }

            var topLoopIndex = _nodes.Count >= MaxSize ? _nodes.Count - 2 : _nodes.Count - 1;
            
            for (var i = topLoopIndex; i >= 0; i--) {
                _nodes[i + 1] = _nodes[i];
            }

            _nodes[0] = item;
            LastNodePosition = _nodes.Count - 1;
        }

        public void AddDistinct(T item) {
            if(_nodes.ContainsValue(item)) { return; }
            Add(item);
        }

        public void AddRange(params T[] list) {
            foreach (var val in list) {
                Add(val);
            }
        }

        public void AddDistinctRange(params T[] list) {
            foreach (var val in list) {
                AddDistinct(val);
            }
        }

        public void Clear() => _nodes.Clear();

        private int Clamp(int value, int min, int max) => Math.Max(min, Math.Min(value, max));
    }

}

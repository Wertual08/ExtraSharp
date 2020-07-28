using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace ExtraSharp
{
    public enum StoryAction
    {
        None,
        Changed,
        Removed,
        Created,
    }
    public class StoryList<T> : IList<T>
    {
        private struct Event
        {
            public StoryAction Type;
            public int Index;
            public T[] Value;
            public bool Link;
            public Event(StoryAction type, int index, T value, bool link = false)
            {
                Type = type;
                Index = index;
                Value = new T[] { value };
                Link = link;
            }
            public Event(StoryAction type, int index, T value, T cvalue, bool link = false)
            {
                Type = type;
                Index = index;
                Value = new T[] { value, cvalue };
                Link = link;
            }
        }
        private List<T> Data = null;
        private List<Event> Story = new List<Event>();
        private void TrimStory()
        {
            Story.RemoveRange(StoryIndex + 1, Story.Count - StoryIndex - 1);
        }

        public int StoryCount { get { return Story.Count; } }
        public int StoryIndex { get; private set; } = -1;
        public bool PrevState { get { return StoryIndex >= 0; } }
        public bool NextState { get { return StoryIndex < Story.Count - 1; } }

        public void Undo()
        {
            if (StoryIndex < 0) return;
            var act = Story[StoryIndex];
            switch (act.Type)
            {
                case StoryAction.Changed: Data[act.Index] = act.Value[0]; break;
                case StoryAction.Created: Data.RemoveAt(act.Index); break;
                case StoryAction.Removed: Data.Insert(act.Index, act.Value[0]); break;
            }
            StoryIndex--;
            if (act.Type == StoryAction.Created) act.Type = StoryAction.Removed;
            else if(act.Type == StoryAction.Removed) act.Type = StoryAction.Created;
            ListChanged?.Invoke(this, new StoryEventArgs(act.Index, act.Type));
            if (StoryIndex >= 0 && Story[StoryIndex].Link) Undo();
        }
        public void Redo()
        {
            if (StoryIndex >= Story.Count - 1) return;
            StoryIndex++;
            var act = Story[StoryIndex];
            switch (act.Type)
            {
                case StoryAction.Changed: Data[act.Index] = act.Value[1]; break;
                case StoryAction.Created: Data.Insert(act.Index, act.Value[0]); break;
                case StoryAction.Removed: Data.RemoveAt(act.Index); break;
            }

            ListChanged?.Invoke(this, new StoryEventArgs(act.Index, act.Type));
            if (act.Link) Redo();
        }
        public void Forget()
        {
            StoryIndex = -1;
            TrimStory();
        }
        public void Forget(int from)
        {
            throw new NotImplementedException();
        }
        public void Forget(int from, int count)
        {
            throw new NotImplementedException();
        }
        public void AppendAction()
        {
            if (StoryIndex >= 0 && StoryIndex < Story.Count)
            {
                var x = Story[StoryIndex];
                x.Link = true;
                Story[StoryIndex] = x;
            }
        }
        public void BeginSequence()
        {
            throw new NotImplementedException();
        }
        public void EndSequence()
        {
            throw new NotImplementedException();
        }
        public void Swap(int first, int second)
        {
            TrimStory();
            var t = Data[first];

            StoryIndex++;
            Story.Add(new Event(StoryAction.Changed, first, t, Data[second], true));
            Data[first] = Data[second];
            ListChanged?.Invoke(this, new StoryEventArgs(first, StoryAction.Changed));

            StoryIndex++;
            Story.Add(new Event(StoryAction.Changed, second, Data[second], t));
            Data[second] = t;
            ListChanged?.Invoke(this, new StoryEventArgs(second, StoryAction.Changed));
        }
        public void SilentSet(int index, T item)
        {
            Data[index] = item;
        }

        public event ListChangedEventHandler ListChanged;

        public StoryList()
        {
            Data = new List<T>();
        }
        public StoryList(List<T> list)
        {
            Data = list;
        }
        public StoryList(int capacity)
        {
            Data = new List<T>(capacity);
        }
        public StoryList(IEnumerable<T> collection)
        {
            Data = new List<T>(collection);
        }

        public T this[int i]
        {
            get
            {
                return Data[i];
            }
            set
            {
                if (value.Equals(Data[i])) return;
                TrimStory();
                StoryIndex++;
                Story.Add(new Event(StoryAction.Changed, i, Data[i], value));
                Data[i] = value;
                ListChanged?.Invoke(this, new StoryEventArgs(i, StoryAction.Changed));
            }
        }
        public int Count { get { return Data.Count; } }
        public bool IsReadOnly { get { return false; } }

        public void Add(T item)
        {
            Insert(Count, item);
        }
        public void Clear()
        {
            if (Count > 0)
            {
                TrimStory();
                while (Count > 0)
                {
                    StoryIndex++;
                    Story.Add(new Event(StoryAction.Removed, 0, Data[Data.Count - 1], Count > 1));
                    Data.RemoveAt(Data.Count - 1);
                    ListChanged?.Invoke(this, new StoryEventArgs(Data.Count - 1, StoryAction.Removed));
                }
            }
        }
        public bool Contains(T item)
        {
            return Data.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Data.GetEnumerator();
        }
        public int IndexOf(T item)
        {
            return Data.IndexOf(item);
        }
        public void Insert(int index, T item)
        {
            TrimStory();
            StoryIndex++;

            Story.Add(new Event(StoryAction.Created, index, item));
            Data.Insert(index, item);
            ListChanged?.Invoke(this, new StoryEventArgs(index, StoryAction.Created));
        }
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0) return false;
            Data.RemoveAt(index);
            ListChanged?.Invoke(this, new StoryEventArgs(index, StoryAction.Removed));
            return true;
        }
        public void RemoveAt(int index)
        {
            TrimStory();
            StoryIndex++;

            Story.Add(new Event(StoryAction.Removed, index, Data[index]));
            Data.RemoveAt(index);
            ListChanged?.Invoke(this, new StoryEventArgs(index, StoryAction.Removed));
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }
    }
    public delegate void ListChangedEventHandler(object sender, StoryEventArgs e);
    public class StoryEventArgs : EventArgs
    {
        public int Index { get; private set; }
        public StoryAction Action { get; private set; }
        public StoryEventArgs(int index, StoryAction action)
        {
            Index = index;
            Action = action;
        }
    }
}

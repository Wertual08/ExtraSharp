using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ExtraSharp
{
    public class StoryItem<T> : IStory
    {
        private List<T> Story = new List<T>();
        private T Final;
        private void TrimStory()
        {
            Story.RemoveRange(StoryIndex + 1, Story.Count - StoryIndex - 1);
        }

        private T Data;

        public StoryItem(T item)
        {
            Data = item;
            Final = item;
        }
        public T Item
        {
            get
            {
                return Data;
            }
            set
            {
                if (Data.Equals(value)) return;
                TrimStory();
                StoryIndex++;
                Story.Add(Data);
                Data = value;
                Final = value;
                ValueChanged?.Invoke(this, new EventArgs());
            }
        }

        public delegate void ValueChangedEventHandler(object sender, EventArgs e);
        public event ValueChangedEventHandler ValueChanged;

        public int StoryCount { get { return Story.Count; } }
        public int StoryIndex { get; private set; } = -1;
        public bool PrevState { get { return StoryIndex >= 0; } }
        public bool NextState { get { return StoryIndex < StoryCount - 1; } }

        public void Undo()
        {
            if (StoryIndex < 0) return;

            Data = Story[StoryIndex];
            StoryIndex--;
            ValueChanged?.Invoke(this, new EventArgs());
        }
        public void Redo()
        {
            if (StoryIndex >= Story.Count - 1) return;
            StoryIndex++;
            if (StoryIndex + 1 == Story.Count)
            {
                Data = Final;
            }
            else Data = Story[StoryIndex + 1];
            ValueChanged?.Invoke(this, new EventArgs());
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
    }
}

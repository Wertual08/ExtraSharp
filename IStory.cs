using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraSharp
{
    public interface IStory
    {
        int StoryCount { get; }
        int StoryIndex { get; }
        bool PrevState { get; } 
        bool NextState { get; }

        void Undo();
        void Redo();
        void Forget();
    }
}

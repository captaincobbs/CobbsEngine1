using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cobbs_Engine.Components
{
    public interface ISceneEvent
    {
        public string EventType { get; }
        public object Data { get; }
    }
}

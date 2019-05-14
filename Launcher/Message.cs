using System.Collections.Generic;

namespace Launcher
{
    class Message
    {
        public Types ID { get; set; }
        public string Version { get; set; }
        public string Download { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public List<string> Exceptions { get; set; }

        public enum Types
        {
            Update,
            Latest
        }
    }
}

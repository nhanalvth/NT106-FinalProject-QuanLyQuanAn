using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class ChatMessage
    {
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{UserName} ({Role}): {Message}";
        }
    }

}

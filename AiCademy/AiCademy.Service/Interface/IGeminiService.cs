using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCademy.Service.Interface
{
    public interface IGeminiService
    {
        public Task<string> SendText(string Question);
    }
}

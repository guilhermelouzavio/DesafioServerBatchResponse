using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apl_batch_readQueue.Services
{
    public interface IMessageService
    {
        Task ProcessaMensagem();
    }
}

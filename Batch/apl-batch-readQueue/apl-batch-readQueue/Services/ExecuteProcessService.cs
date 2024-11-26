using Microsoft.Extensions.Hosting;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apl_batch_readQueue.Services
{
    public class ExecuteProcessService : IJob
    {
        private readonly IMessageService _messageService;

        public ExecuteProcessService(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _messageService.ProcessaMensagem();
            return Task.CompletedTask;
        }
    }
}

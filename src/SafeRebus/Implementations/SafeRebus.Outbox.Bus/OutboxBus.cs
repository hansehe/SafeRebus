using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Bus.Advanced;
using SafeRebus.Outbox.Abstractions;

namespace SafeRebus.Outbox.Bus
{
    public class OutboxBus : IOutboxBus
    {
        private IList<Func<Task>> FuncsToCommit = new List<Func<Task>>();
        
        private readonly IBus Bus;

        public OutboxBus(IBus bus)
        {
            Bus = bus;
        }
        
        public async Task Commit()
        {
            foreach (var func in FuncsToCommit)
            {
                await func.Invoke();
            }
            FuncsToCommit = new List<Func<Task>>();
        }

        public Task Rollback()
        {
            FuncsToCommit = new List<Func<Task>>();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Bus?.Dispose();
        }

        public Task SendLocal(object commandMessage, Dictionary<string, string> optionalHeaders = null)
        {
            FuncsToCommit.Add(() => Bus.SendLocal(commandMessage, optionalHeaders));
            return Task.CompletedTask;
        }

        public Task Send(object commandMessage, Dictionary<string, string> optionalHeaders = null)
        {
            FuncsToCommit.Add(() => Bus.Send(commandMessage, optionalHeaders));
            return Task.CompletedTask;
        }

        public Task DeferLocal(TimeSpan delay, object message, Dictionary<string, string> optionalHeaders = null)
        {
            FuncsToCommit.Add(() => Bus.DeferLocal(delay, message, optionalHeaders));
            return Task.CompletedTask;
        }

        public Task Defer(TimeSpan delay, object message, Dictionary<string, string> optionalHeaders = null)
        {
            FuncsToCommit.Add(() => Bus.Defer(delay, message, optionalHeaders));
            return Task.CompletedTask;
        }

        public Task Reply(object replyMessage, Dictionary<string, string> optionalHeaders = null)
        {
            FuncsToCommit.Add(() => Bus.Reply(replyMessage, optionalHeaders));
            return Task.CompletedTask;
        }

        public Task Subscribe<TEvent>()
        {
            FuncsToCommit.Add(() => Bus.Subscribe<TEvent>());
            return Task.CompletedTask;
        }

        public Task Subscribe(Type eventType)
        {
            FuncsToCommit.Add(() => Bus.Subscribe(eventType));
            return Task.CompletedTask;
        }

        public Task Unsubscribe<TEvent>()
        {
            FuncsToCommit.Add(() => Bus.Unsubscribe<TEvent>());
            return Task.CompletedTask;
        }

        public Task Unsubscribe(Type eventType)
        {
            FuncsToCommit.Add(() => Bus.Unsubscribe(eventType));
            return Task.CompletedTask;
        }

        public Task Publish(object eventMessage, Dictionary<string, string> optionalHeaders = null)
        {
            FuncsToCommit.Add(() => Bus.Publish(eventMessage, optionalHeaders));
            return Task.CompletedTask;
        }

        public IAdvancedApi Advanced => Bus.Advanced;
    }
}
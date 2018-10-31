using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Bus.Advanced;
using Rebus.Messages;
using Rebus.Transport;
using SafeRebus.Outbox.Abstractions;
using SafeRebus.Outbox.Abstractions.Entities;

namespace SafeRebus.Outbox.Bus
{
    public class OutboxBus : IOutboxBus
    {
        private IList<Tuple<Func<Task>, Guid>> FuncsToCommit;
        private TransportMessage TransportMessage;
        
        private readonly IBus Bus;
        private readonly IOutboxMessageRepository OutboxMessageRepository;

        public OutboxBus(
            IBus bus,
            IOutboxMessageRepository outboxMessageRepository)
        {
            Bus = bus;
            OutboxMessageRepository = outboxMessageRepository;
        }
        
        public async Task Commit()
        {
            foreach (var tuple in FuncsToCommit)
            {
                await tuple.Item1.Invoke();
                await OutboxMessageRepository.DeleteOutboxMessage(tuple.Item2);
            }
            
            FuncsToCommit = null;
        }

        public Task BeginTransaction(TransportMessage transportMessage)
        {
            TransportMessage = transportMessage;
            FuncsToCommit = new List<Tuple<Func<Task>, Guid>>();
            return Task.CompletedTask;
        }

        public Task ResendOutboxMessage(OutboxMessage outboxMessage)
        {
            switch (outboxMessage.SendFunction)
            {
               case nameof(SendLocal):
                   return Bus.SendLocal(outboxMessage.Message, outboxMessage.Headers);
               case nameof(Send):
                   return Bus.Send(outboxMessage.Message, outboxMessage.Headers);
               case nameof(DeferLocal):
                   return Bus.DeferLocal(TimeSpan.Zero, outboxMessage.Message, outboxMessage.Headers);
                case nameof(Defer):
                    return Bus.Defer(TimeSpan.Zero, outboxMessage.Message, outboxMessage.Headers);
                case nameof(Reply):
                    AssertReturnAddressExists(outboxMessage);
                    return Bus.Advanced.Routing.Send(outboxMessage.Headers[Headers.ReturnAddress], outboxMessage.Message, outboxMessage.Headers);
                case nameof(Publish):
                    return Bus.Publish(outboxMessage.Message, outboxMessage.Headers);
               default:
                   throw new ArgumentException(
                       $"Could not resend outbox message due to unknown send function: {outboxMessage.SendFunction}");
            }   
        }

        public Task SendLocal(object commandMessage, Dictionary<string, string> optionalHeaders = null)
        {
            Func<Task> funcToCommit = () => Bus.SendLocal(commandMessage, optionalHeaders);
            if (!TransactionIsInitiated())
            {
                return funcToCommit.Invoke();
            }
            const string sendFunction = nameof(SendLocal);
            return SaveOutboxMessage(funcToCommit, sendFunction, commandMessage, optionalHeaders);
        }

        public Task Send(object commandMessage, Dictionary<string, string> optionalHeaders = null)
        {
            Func<Task> funcToCommit = () => Bus.Send(commandMessage, optionalHeaders);
            if (!TransactionIsInitiated())
            {
                return funcToCommit.Invoke();
            }
            const string sendFunction = nameof(Send);
            return SaveOutboxMessage(funcToCommit, sendFunction, commandMessage, optionalHeaders);
        }

        public Task DeferLocal(TimeSpan delay, object message, Dictionary<string, string> optionalHeaders = null)
        {
            Func<Task> funcToCommit = () => Bus.DeferLocal(delay, message, optionalHeaders);
            if (!TransactionIsInitiated())
            {
                return funcToCommit.Invoke();
            }
            const string sendFunction = nameof(DeferLocal);
            return SaveOutboxMessage(funcToCommit, sendFunction, message, optionalHeaders);
        }

        public Task Defer(TimeSpan delay, object message, Dictionary<string, string> optionalHeaders = null)
        {
            Func<Task> funcToCommit = () => Bus.Defer(delay, message, optionalHeaders);
            if (!TransactionIsInitiated())
            {
                return funcToCommit.Invoke();
            }
            const string sendFunction = nameof(Defer);
            return SaveOutboxMessage(funcToCommit, sendFunction, message, optionalHeaders);
        }

        public Task Reply(object replyMessage, Dictionary<string, string> optionalHeaders = null)
        {
            Func<Task> funcToCommit = () => Bus.Reply(replyMessage, optionalHeaders);
            if (!TransactionIsInitiated())
            {
                return funcToCommit.Invoke();
            }
            const string sendFunction = nameof(Reply);
            return SaveOutboxMessage(funcToCommit, sendFunction, replyMessage, optionalHeaders);
        }

        public Task Subscribe<TEvent>()
        {
            return Bus.Subscribe<TEvent>();
        }

        public Task Subscribe(Type eventType)
        {
            return Bus.Subscribe(eventType);
        }

        public Task Unsubscribe<TEvent>()
        {
            return Bus.Unsubscribe<TEvent>();
        }

        public Task Unsubscribe(Type eventType)
        {
            return Bus.Unsubscribe(eventType);
        }

        public Task Publish(object eventMessage, Dictionary<string, string> optionalHeaders = null)
        {
            Func<Task> funcToCommit = () => Bus.Publish(eventMessage, optionalHeaders);
            if (!TransactionIsInitiated())
            {
                return funcToCommit.Invoke();
            }
            const string sendFunction = nameof(Publish);
            return SaveOutboxMessage(funcToCommit, sendFunction, eventMessage, optionalHeaders);
        }

        public IAdvancedApi Advanced => Bus.Advanced;

        private Task SaveOutboxMessage(Func<Task> funcToCommit, string sendFunction, object message, Dictionary<string, string> optionalHeaders)
        {
            AssertTransactionInitiated();
            var outboxMessage = new OutboxMessage
            {
                Headers = MergeWithOriginalHeaders(optionalHeaders),
                Message = message,
                SendFunction = sendFunction
            };
            FuncsToCommit.Add(new Tuple<Func<Task>, Guid>(funcToCommit, outboxMessage.Id));
            return OutboxMessageRepository.InsertOutboxMessage(outboxMessage);
        }

        private Dictionary<string, string> MergeWithOriginalHeaders(Dictionary<string, string> optionalHeaders)
        {
            var headers = new Dictionary<string, string>();
            TransportMessage?.Headers.ToList().ForEach(x => headers.Add(x.Key, x.Value));
            optionalHeaders?.ToList().ForEach(x => headers.Add(x.Key, x.Value));
            return headers;
        }

        private bool TransactionIsInitiated()
        {
            return FuncsToCommit != null;
        }

        private void AssertTransactionInitiated()
        {
            if (!TransactionIsInitiated())
            {
                throw new Exception("Outbox bus transaction is not initiated, please run 'BeginTransaction(..)'");
            }
        }

        private void AssertReturnAddressExists(OutboxMessage outboxMessage)
        {
            if (!outboxMessage.Headers.ContainsKey(Headers.ReturnAddress))
            {
                throw new Exception($"Outbox message headers does not container a return address value with key: {Headers.ReturnAddress}");
            }
        }

        public void Dispose()
        {
        }
    }
}
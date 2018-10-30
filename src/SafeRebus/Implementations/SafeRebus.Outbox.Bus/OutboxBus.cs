using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Bus.Advanced;
using Rebus.Messages;
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
                   return SendLocal(outboxMessage.Message, outboxMessage.Headers);
               case nameof(Send):
                   return Send(outboxMessage.Message, outboxMessage.Headers);
               case nameof(DeferLocal):
                   return DeferLocal(TimeSpan.Zero, outboxMessage.Message, outboxMessage.Headers);
                case nameof(Defer):
                    return Defer(TimeSpan.Zero, outboxMessage.Message, outboxMessage.Headers);
                case nameof(Reply):
                    return Reply(outboxMessage.Message, outboxMessage.Headers);
                case nameof(Publish):
                    return Publish(outboxMessage.Message, outboxMessage.Headers);
               default:
                   throw new ArgumentException(
                       $"Could not resend outbox message due to unknown send function: {outboxMessage.SendFunction}");
            }   
        }

        public Task SendLocal(object commandMessage, Dictionary<string, string> optionalHeaders = null)
        {
            var funcToCommit = new Func<Task>(() => Bus.SendLocal(commandMessage, optionalHeaders));
            const string sendFunction = nameof(SendLocal);
            return SaveOutboxMessageOrSendDirectIfTransactionNotInitiated(funcToCommit, sendFunction, commandMessage, optionalHeaders);
        }

        public Task Send(object commandMessage, Dictionary<string, string> optionalHeaders = null)
        {
            var funcToCommit = new Func<Task>(() => Bus.Send(commandMessage, optionalHeaders));
            const string sendFunction = nameof(Send);
            return SaveOutboxMessageOrSendDirectIfTransactionNotInitiated(funcToCommit, sendFunction, commandMessage, optionalHeaders);
        }

        public Task DeferLocal(TimeSpan delay, object message, Dictionary<string, string> optionalHeaders = null)
        {
            var funcToCommit = new Func<Task>(() => Bus.DeferLocal(delay, message, optionalHeaders));
            const string sendFunction = nameof(DeferLocal);
            return SaveOutboxMessageOrSendDirectIfTransactionNotInitiated(funcToCommit, sendFunction, message, optionalHeaders);
        }

        public Task Defer(TimeSpan delay, object message, Dictionary<string, string> optionalHeaders = null)
        {
            var funcToCommit = new Func<Task>(() => Bus.Defer(delay, message, optionalHeaders));
            const string sendFunction = nameof(Defer);
            return SaveOutboxMessageOrSendDirectIfTransactionNotInitiated(funcToCommit, sendFunction, message, optionalHeaders);
        }

        public Task Reply(object replyMessage, Dictionary<string, string> optionalHeaders = null)
        {
            var funcToCommit = new Func<Task>(() => Bus.Reply(replyMessage, optionalHeaders));
            const string sendFunction = nameof(Reply);
            return SaveOutboxMessageOrSendDirectIfTransactionNotInitiated(funcToCommit, sendFunction, replyMessage, optionalHeaders);
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
            var funcToCommit = new Func<Task>(() => Bus.Publish(eventMessage, optionalHeaders));
            const string sendFunction = nameof(Publish);
            return SaveOutboxMessageOrSendDirectIfTransactionNotInitiated(funcToCommit, sendFunction, eventMessage, optionalHeaders);
        }

        public IAdvancedApi Advanced => Bus.Advanced;

        private Task SaveOutboxMessageOrSendDirectIfTransactionNotInitiated(Func<Task> funcToCommit,
            string sendFunction, object message, Dictionary<string, string> optionalHeaders)
        {
            return !TransactionIsInitiated() ? funcToCommit.Invoke() : SaveOutboxMessage(funcToCommit, sendFunction, message, optionalHeaders);
        }

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
            optionalHeaders?.ToList().ForEach(x => headers.Add(x.Key, x.Value));
            TransportMessage?.Headers.ToList().ForEach(x => headers.Add(x.Key, x.Value));
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

        public void Dispose()
        {
        }
    }
}
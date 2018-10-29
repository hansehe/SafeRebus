using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SafeRebus.MessageHandler.Contracts.Responses;

namespace SafeRebus.MessageHandler.Abstractions
{
    public interface IResponseRepository
    {
        Task InsertResponse(SafeRebusResponse response);
        Task<SafeRebusResponse> SelectResponse(Guid id);
        Task<IEnumerable<SafeRebusResponse>> SelectResponses(IEnumerable<Guid> ids);
    }
}
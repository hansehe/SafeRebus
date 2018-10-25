using System;
using System.Threading.Tasks;
using SafeRebus.Contracts.Responses;

namespace SafeRebus.Abstractions
{
    public interface IResponseRepository
    {
        Task InsertResponse(SafeRebusResponse response);
        Task<SafeRebusResponse> SelectResponse(Guid id);
    }
}
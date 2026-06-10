using Org.BouncyCastle.Pqc.Crypto.Falcon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameServer.Logic.Service
{
    internal class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public ServiceErrorCode ErrorCode { get; set; }
        public object? Data { get; set; }

        public T? GetData<T>() where T : class => Data as T;
        public T GetValue<T>() where T : struct => (T)Data!;
        public static ServiceResult Success() => new ServiceResult { IsSuccess = true, ErrorCode = ServiceErrorCode.ErrorNone };
        public static ServiceResult Failure(ServiceErrorCode errorCode) => new ServiceResult { IsSuccess = false, ErrorCode = errorCode };

    }
}

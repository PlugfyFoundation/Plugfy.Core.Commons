using System;
using System.Threading.Tasks;

namespace Plugfy.Core.Commons.Runtime
{
    public interface IDataCommunication
    {

        string Name { get; }

        string Description { get; }

        event EventHandler<DataReceivedEventArgs> DataReceived;

        Task SendDataAsync(object data);

        Task CloseAsync();

        Task InitializeAsync(dynamic parameters);

        Task StartListeningAsync();
        bool IsClosed { get; }

    }


    public class DataReceivedEventArgs : EventArgs
    {

        public object Data { get; set; }

        public bool ContinueReceiving { get; set; } = true;

        public DataReceivedEventArgs(object data)
        {
            Data = data;
        }
    }
}

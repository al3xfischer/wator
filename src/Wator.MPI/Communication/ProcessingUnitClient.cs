using System;
using System.Runtime.InteropServices.ComTypes;
using MPI;

namespace Wator.MPI.Communication
{
    public class ProcessingUnitClient
    {
        private readonly Intracommunicator _communicator = Communicator.world;
        private readonly int _lowerProcessRank;
        private readonly int _upperProcessRank;

        public ProcessingUnitClient()
        {
            _lowerProcessRank = TargetRankHelper.GetLowerProcessRank(_communicator.Rank, _communicator.Size);
            _upperProcessRank = TargetRankHelper.GetUpperProcessRank(_communicator.Rank, _communicator.Size);
        }

        public void SendToLowerProcess<T>(T message, int tag)
        {
            Console.WriteLine($"Send message {message} to {_lowerProcessRank}");
            _communicator.Send(message, _lowerProcessRank, tag);
        }

        public void SendToUpperProcess<T>(T message, int tag)
        {
            _communicator.Send(message, _upperProcessRank, tag);
        }

        public (T Message, CompletedStatus Status) ReceiveFromUpperProcess<T>(int tag)
        {
            _communicator.Receive<T>(_upperProcessRank, tag, out var received, out var status);
            return (received, status);
        }

        public (T Message, CompletedStatus Status) SendLowerReceiveUpper<T>(T message, int sendTag, int receiveTag)
        {
            _communicator.SendReceive(message,
                _lowerProcessRank,
                sendTag,
                _upperProcessRank,
                receiveTag,
                out var received,
                out var status);
            return (received, status);
        }

        public (T Message, CompletedStatus Status) SendUpperReceiveLower<T>(T message, int sendTag, int receiveTag)
        {
            _communicator.SendReceive(message,
                _upperProcessRank,
                sendTag,
                _lowerProcessRank,
                receiveTag,
                out var received,
                out var status);
            return (received, status);
        }

        public (T Message, CompletedStatus Status) ReceiveFromLowerProcess<T>(int tag)
        {
            _communicator.Receive<T>(_lowerProcessRank, tag, out var received, out var status);
            return (received, status);
        }
    }
}

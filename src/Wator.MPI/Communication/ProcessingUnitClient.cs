using System;
using System.Runtime.InteropServices.ComTypes;
using MPI;

namespace Wator.MPI.Communication
{
    public class ProcessingUnitClient
    {
        private readonly Intracommunicator _communicator = Communicator.world;
        private readonly int _nextProcessRank;
        private readonly int _previousProcessRank;

        public ProcessingUnitClient()
        {
            _nextProcessRank = TargetRankHelper.GetNextProcessRank(_communicator.Rank, _communicator.Size);
            _previousProcessRank = TargetRankHelper.GetPreviousProcessRank(_communicator.Rank, _communicator.Size);
        }

        public void SendToNextProcess<T>(T message, int tag)
        {
            Console.WriteLine($"Send message {message} to {_nextProcessRank}");
            _communicator.Send(message, _nextProcessRank, tag);
        }

        public void SendToPreviousProcess<T>(T message, int tag)
        {
            _communicator.Send(message, _previousProcessRank, tag);
        }

        public (T Message, CompletedStatus Status) ReceiveFromPreviousProcess<T>(int tag)
        {
            _communicator.Receive<T>(_previousProcessRank, tag, out var received, out var status);
            return (received, status);
        }

        public (T Message, CompletedStatus Status) ReceiveFromNextProcess<T>(int tag)
        {
            _communicator.Receive<T>(_nextProcessRank, tag, out var received, out var status);
            return (received, status);
        }
    }
}

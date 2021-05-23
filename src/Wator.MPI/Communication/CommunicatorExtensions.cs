using MPI;

namespace Wator.MPI.Communication
{
    public static class CommunicatorExtensions
    {
        /// <summary>
        /// Sends the lower receive upper.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comm">The comm.</param>
        /// <param name="message">The message.</param>
        /// <param name="sendTag">The send tag.</param>
        /// <param name="receiveTag">The receive tag.</param>
        /// <returns></returns>
        public static (T Message, CompletedStatus Status) SendLowerReceiveUpper<T>(this Communicator comm, T message, int sendTag, int receiveTag)
        {
            comm.SendReceive(message,
                comm.GetLowerProcessRank(),
                sendTag,
                comm.GetUpperProcessRank(),
                receiveTag,
                out var received,
                out var status);
            return (received, status);
        }

        /// <summary>
        /// Sends the upper receive lower.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comm">The comm.</param>
        /// <param name="message">The message.</param>
        /// <param name="sendTag">The send tag.</param>
        /// <param name="receiveTag">The receive tag.</param>
        /// <returns></returns>
        public static (T Message, CompletedStatus Status) SendUpperReceiveLower<T>(this Communicator comm, T message, int sendTag, int receiveTag)
        {
            comm.SendReceive(message,
                comm.GetUpperProcessRank(),
                sendTag,
                comm.GetLowerProcessRank(),
                receiveTag,
                out var received,
                out var status);
            return (received, status);
        }

        private static int GetLowerProcessRank(this Communicator comm)
        {
            return TargetRankHelper.GetLowerProcessRank(comm.Rank, comm.Size);
        }

        private static int GetUpperProcessRank(this Communicator comm)
        {
            return TargetRankHelper.GetUpperProcessRank(comm.Rank, comm.Size);
        }
    }
}

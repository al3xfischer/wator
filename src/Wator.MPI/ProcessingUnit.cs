using System;
using System.Collections.Generic;
using System.Text;
using MPI;

namespace Wator.MPI
{
    public class ProcessingUnit
    {
        private readonly Intracommunicator _comm;

        public ProcessingUnit(Intracommunicator communicator)
        {
            _comm = communicator ?? throw new ArgumentNullException(nameof(communicator));
        }

        public void Process()
        {
            var target = (_comm.Rank - 2 + _comm.Size) % _comm.Size;

            // Algorithm for processing unit:
            // wait for processing task from master process
            // Process field except borders
            // Send/Receive field from border process
            // If sender
            // send local border
            // receive updated border
            // update local border
            // Else
            // received local border
            // calculate updated borders
            // send updated border
            // Send complete result to master process
        }
    }
}

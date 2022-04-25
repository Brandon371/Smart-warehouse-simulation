// File name: Job.cs
/*Description:Defining the entity: Job in this model */
//Tables: Nothing
//Author: Li Yunmiao (Brandon)
//Modification Date: 2022/04/08 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Warehouse_Simulation
{
    class Job
    {
        #region dynamic properties
        public Grid origin {get; set;}
        public Grid destination { get; set; }

        public Vehicles Job_To_Vehicle { get; set; }

        public DateTime Generate_time { get; set; }

        public DateTime Finish_time { get; set; }

        #endregion

        //Two Constructors of Job
        public Job(Grid origin, Grid destination)
        {
            this.origin = origin;
            this.destination = destination;

        }

        public Job()
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Warehouse_Simulation
{
    class Vehicles
    {
        #region Dynamic Properties
        public Job Assigned_Job { get; set; } = new Job();

        public int Index { get; set; }
        public Grid Parking_Position { get; set; }

        public Grid Current_Position { get; set; }
        public List<Grid> Path { get; set; } = new List<Grid>();
        public List<Grid> Reservation { get; set; } = new List<Grid>();

        public int Status { get; set; }

        public List<Grid> Reservation_Pending { get; set; } = new List<Grid>();
        #endregion

        public Vehicles()
        {

        }
    }
}

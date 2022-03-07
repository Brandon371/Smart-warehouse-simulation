using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Warehouse_Simulation
{
    class Grid
    {
        #region Static properties
        public int Row_Index { get; set; } = new int();
        public int Column_Index { get; set; } = new int();

        public bool Obstacle { get; set; }

        public int G { get; set; }
        public int H { get; set; }

        public int F { get; set; }


        #endregion

        #region Dynamic propertises
        public List<Job> Job_From { get; set; } = new List<Job>();
        public List<Job> Job_To { get; set; } = new List<Job>();

        public Grid ParentGrid { get; set; }

        public List<Vehicles> Vehicles_Parking { get; set; }
        public List<Vehicles> Vehicles_Via { get; set; }
        public List<Vehicles> Vehcles_Reserving { get; set; }
        public List<Vehicles> Vehicles_Pending { get; set; } = new List<Vehicles>();

        #endregion

        public void InitGrid(Grid parent, Grid end)
        {
            this.ParentGrid= parent;
            if (parent == null)
            {
                this.G = 0;
            }
            if (parent.Row_Index == this.Row_Index||parent.Column_Index==this.Column_Index)
                this.G = parent.G + 10;
            else
                this.G = parent.G + 14;
            this.H = (Math.Abs(this.Row_Index - end.Row_Index) + Math.Abs(this.Column_Index - end.Column_Index))*10;
            this.F = this.G + this.H;
        }
        public Grid(int row_index, int column_Index)
        {
            this.Row_Index = row_index;
            this.Column_Index = column_Index;
        }

    }
}

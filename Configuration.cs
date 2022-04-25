// File name: Configuration.cs
/*Description: Initial condition of the experiment*/
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
    class Configuration
    {
        public List<Vehicles> total_vehicles { get; private set; } = new List<Vehicles>();


        public void vehcile_generate(int x)
        {
            List<Vehicles> vehicle_list = new List<Vehicles>();
            for (int i = 0; i < x; i++)
            {
                Vehicles AGV = new Vehicles(i);
                AGV.Status = 0;
                vehicle_list.Add(AGV);

            }

            Grid start = new Grid(0, 0);
            for (int i = 0; i < x; i++)
            {
                vehicle_list[i].Parking_Position = start;
            }


            total_vehicles = vehicle_list;
        }
        // HourCounter

        public List<Grid> Map(int x,int y)
        {
            List<Grid> list = new List<Grid>();
            
            for (int i = 0; i < x; i++)
            {
                
                for (int j = 0; j < y; j++)
                {
                    Grid individual = new Grid(i,j);
                    individual.Obstacle = false;
                    list.Add(individual);
                }
            }
            return list;
            
        }

    }
}

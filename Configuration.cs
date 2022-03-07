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


        public void vehcile_generate()
        {
            Vehicles AGV_1 = new Vehicles();
            Vehicles AGV_2 = new Vehicles();
            Vehicles AGV_3 = new Vehicles();
            Vehicles AGV_4 = new Vehicles();
            Vehicles AGV_5 = new Vehicles();

            AGV_1.Status = 0;
            AGV_2.Status = 0;
            AGV_3.Status = 0;
            AGV_4.Status = 0;
            AGV_5.Status = 0;

            AGV_1.Index = 1;
            AGV_2.Index = 2;
            AGV_3.Index = 3;
            AGV_4.Index = 4;
            AGV_5.Index = 5;

            List<Vehicles> vehicle_list = new List<Vehicles>();
            vehicle_list.Add(AGV_1);
            vehicle_list.Add(AGV_2);
            vehicle_list.Add(AGV_3);
            vehicle_list.Add(AGV_4);
            vehicle_list.Add(AGV_5);


            Grid start = new Grid(0, 0);
            for (int i = 0; i < 5; i++)
            {
                vehicle_list[i].Parking_Position = start;
            }


            total_vehicles = vehicle_list;
        }
        // HourCounter

        public List<Grid> Map()
        {
            List<Grid> list = new List<Grid>();
            
            for (int i = 0; i < 10; i++)
            {
                
                for (int j = 0; j < 10; j++)
                {
                    Grid individual = new Grid(i,j);
                    individual.Obstacle = false;
                    list.Add(individual);
                }
            }

            list[8].Obstacle = true;
            list[13].Obstacle = true;
            list[23].Obstacle = true;
            list[35].Obstacle = true;
            list[42].Obstacle = true;
            list[57].Obstacle = true;

            return list;
            
        }

    }
}

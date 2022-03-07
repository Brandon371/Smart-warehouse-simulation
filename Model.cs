using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using O2DESNet;
using O2DESNet.Distributions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Smart_Warehouse_Simulation
{
    
    class Model : O2DESNet.Sandbox
    {
        System sys = new System();
        public int job_finished = 0;
        public int job_generate = 0;
        public List<int> time_spent = new List<int>();


        private Configuration Config { get; set; } 
        public Model(Configuration config, int seed=0) : base(seed)
        {
            Config = config;
            config.vehcile_generate();
            sys.Vehicles_Idle = config.total_vehicles;
            List<Grid> map = config.Map();
            sys.Grids_Releasing = map;
            sys.Grids_Releasing.RemoveAt(57);
            sys.Grids_Releasing.RemoveAt(42);
            sys.Grids_Releasing.RemoveAt(35);

            sys.Grids_Releasing.RemoveAt(23);
            sys.Grids_Releasing.RemoveAt(13);
            sys.Grids_Releasing.RemoveAt(8);
            
            sys.Obstacle_List.Add(map[8]);
            sys.Obstacle_List.Add(map[13]);
            sys.Obstacle_List.Add(map[23]);
            sys.Obstacle_List.Add(map[35]);
            sys.Obstacle_List.Add(map[42]);
            sys.Obstacle_List.Add(map[57]);
            sys.Grids_Reserved.Add(map[0]);

            Schedule(Job_Arrival, Exponential.Sample(DefaultRS, TimeSpan.FromSeconds(10)));
        }

        void Job_Arrival()
        {
            sys.Job_Generate(DefaultRS);
            job_generate++;
            sys.Job_Pending[0].Generate_time = ClockTime;
            
            Schedule(Job_Arrival, Exponential.Sample(DefaultRS, TimeSpan.FromSeconds(10)));
            Schedule(Job_Allocation);
            Console.WriteLine($"{ClockTime}\tJob_generated");

        }

        void Job_Allocation()
        {
            
            if (sys.Job_Pending.Count !=0&&sys.Vehicles_Idle.Count != 0)
            {
                List<int> distance = new List<int>();
                Dictionary<Vehicles,int> Dicdistance = new Dictionary<Vehicles, int>();

                for (int i = 0; i < sys.Vehicles_Idle.Count; i++)
                {
                    int dis = Math.Abs((sys.Vehicles_Idle[i].Parking_Position.Row_Index - sys.Job_Pending[0].origin.Row_Index)
                        + (sys.Vehicles_Idle[i].Parking_Position.Column_Index - sys.Job_Pending[0].origin.Column_Index));
                    distance.Add(dis);
                    Dicdistance.Add(sys.Vehicles_Idle[i], dis);

                }

                int minvalue = distance.Min();
                int num=0;
                for (int j = 0; j < sys.Vehicles_Idle.Count; j++)
                {
                    if (Dicdistance[sys.Vehicles_Idle[j]] == minvalue)
                    {
                        sys.Vehicles_Idle[j].Assigned_Job = sys.Job_Pending[0];
                        num = j;
                        break;
                    }
                }
                
                sys.Pending_Route.Add(sys.Vehicles_Idle[num]);
                sys.Job_Pending[0].Job_To_Vehicle = sys.Vehicles_Idle[num];
                Vehicles vehicle = sys.Vehicles_Idle[num];
                vehicle.Status = 1;
                vehicle.Current_Position = sys.Vehicles_Idle[num].Parking_Position;
                
                sys.Vehicles_Idle.RemoveAt(num);
                sys.Job_Pending.RemoveAt(0);
                
                Schedule(() => Vehicle_Route(vehicle));
                Console.WriteLine($"{ClockTime}\tJob_allocated");
            }
        } 

        void Vehicle_Route(Vehicles vehicle)
        {

            if (vehicle.Status == 1 && vehicle.Current_Position.Row_Index == vehicle.Assigned_Job.origin.Row_Index
                && vehicle.Current_Position.Column_Index == vehicle.Assigned_Job.origin.Column_Index)
            {
                Schedule(() => Load(vehicle));
            }
            else if (vehicle.Status == 1)
            {
                Grid startpoint = vehicle.Parking_Position;
                Grid endpoint = vehicle.Assigned_Job.origin;
                vehicle.Path = sys.FindPath(startpoint, endpoint);
                if (vehicle.Path.Count == 0)
                {
                    Schedule(() => Vehicle_Route(vehicle), Exponential.Sample(DefaultRS, TimeSpan.FromSeconds(5)));
                    Console.WriteLine($"{ClockTime}\tCannot_find_path");
                }
                else
                {
                    Schedule(() => Reserve_Path(vehicle));
                    Console.WriteLine($"{ClockTime}\tRouting_completed");
                }
            }
            else if (vehicle.Status == 2)
            {
                Grid startpoint = vehicle.Assigned_Job.origin;
                Grid endpoint = vehicle.Assigned_Job.destination;
                vehicle.Path = sys.FindPath(startpoint, endpoint);
                if (vehicle.Path.Count == 0)
                {
                    Schedule(() => Vehicle_Route(vehicle), Exponential.Sample(DefaultRS, TimeSpan.FromSeconds(5)));
                    Console.WriteLine($"{ClockTime}\tCannot_find_path");
                }
                else
                {
                    Schedule(() => Reserve_Path(vehicle));
                    Console.WriteLine($"{ClockTime}\tRouting_completed");
                }
            }

        }
        
        void Reserve_Path(Vehicles vehicle)
        {
            
            
            if (vehicle.Path.Count!=0 )
            {
                
                sys.Grids_Reserved.Add(vehicle.Path[0]);
                sys.RemovePoint(sys.Grids_Releasing, vehicle.Path[0]);
                sys.RemovePoint(sys.Grids_Reserved, vehicle.Current_Position);

                Schedule(() => Complete_Partial_Path(vehicle), Exponential.Sample(DefaultRS, TimeSpan.FromSeconds(5)*vehicle.Path.Count));
                Console.WriteLine($"{ClockTime}\tReserved_completed");

            }
            else
            {
                
                sys.Vehicles_Pending.Add(vehicle);
                
                Schedule(() => Reserve_Path(vehicle), Exponential.Sample(DefaultRS, TimeSpan.FromSeconds(5)));
                Console.WriteLine($"{ClockTime}\tRedo_reserve_path");
            }
            
            
        }

        void Complete_Partial_Path(Vehicles vehicle)
        {
            
            vehicle.Current_Position = vehicle.Path.Last();
            vehicle.Path.RemoveAt(0) ;
               
            if (vehicle.Status == 1 && vehicle.Current_Position.Row_Index == vehicle.Assigned_Job.origin.Row_Index && 
                vehicle.Current_Position.Column_Index == vehicle.Assigned_Job.origin.Column_Index)
            {
                Schedule(() => Load(vehicle));
                Console.WriteLine($"{ClockTime}\tStart_Load");
            }

            else if (vehicle.Status == 2 && vehicle.Current_Position.Row_Index == vehicle.Assigned_Job.destination.Row_Index &&
                vehicle.Current_Position.Column_Index == vehicle.Assigned_Job.destination.Column_Index)
            {
                Schedule(() => Unload(vehicle));
                Console.WriteLine($"{ClockTime}\tStart_Unload");
            }
            else
            {
                Schedule(() => Reserve_Path(vehicle));
                Console.WriteLine($"{ClockTime}\tPartial_Path_Completed");
            }
            
            
        }

        void Load (Vehicles vehicle)
        {
            vehicle.Status = 2;
            
            Schedule(() => Vehicle_Route(vehicle));
            Console.WriteLine($"{ClockTime}\tLoad_Completed");

        }

        void Unload(Vehicles vehicle)
        {
            vehicle.Status = 0;
            sys.Vehicles_Idle.Add(vehicle);
            vehicle.Parking_Position = vehicle.Assigned_Job.destination;
            job_finished++;
            vehicle.Assigned_Job.Finish_time = ClockTime;
            TimeSpan time_cost = vehicle.Assigned_Job.Finish_time - vehicle.Assigned_Job.Generate_time;
            int time_for_second = time_cost.Seconds;
            time_spent.Add(time_for_second);
            Schedule(Job_Allocation);
            Console.WriteLine($"{ClockTime}\tUnload_Completed");
        }
    }
}

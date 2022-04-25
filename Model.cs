// File name: Model.cs
/*Description:The simulation model*/
//Tables: Nothing
//Author: Li Yunmiao (Brandon)
//Modification Date: 2022/04/08 

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
        public float job_finished = 0;
        public float job_generate = 0;
        public List<float> time_spent = new List<float>();
        public int Reserve_Size { get; set; }
        public int Max_Allocation_Distance { get; set; }

        public TimeSpan MaxPending { get; set; } = new TimeSpan();

        private Configuration Config { get; set; } 
        /*Initialization of the model:
         1. Create the map
         2. Generate the obstacles randomly
         */
        public Model(Configuration config, int seed=4) : base(seed)
        {
            Config = config;
            config.vehcile_generate(10);
            sys.Vehicles_Idle = config.total_vehicles;
            List<Grid> map = config.Map(30,30);
            sys.Grids_Releasing = map;
            Random rs = new Random(1);


            for (int i = 0; i < 200; i++)
            {
                int a = rs.Next(0, 700);
                
                sys.Obstacle_List.Add(map[a]);

            }
            for (int i = 0; i < sys.Obstacle_List.Count; i++)
            {
                for (int j = 0; j < map.Count; j++)
                {
                    if (sys.Obstacle_List[i].Row_Index == map[j].Row_Index&&sys.Obstacle_List[i].Column_Index == map[j].Column_Index)
                    {
                        map.Remove(map[j]);
                    }
                }
            }
            
            Schedule(Job_Arrival, Exponential.Sample(DefaultRS, TimeSpan.FromSeconds(10)));
        }

        /*Job Arrival:
         1. Randomly generate jobs
         2. Adding job to the pending list
         */
        void Job_Arrival()
        {
            sys.Job_Generate(DefaultRS,29,29);
            job_generate++;
            sys.Job_Pending[0].Generate_time = ClockTime;
            
            Schedule(Job_Arrival, Exponential.Sample(DefaultRS, TimeSpan.FromSeconds(40)));
            Schedule(Job_Allocation);
            Console.WriteLine($"{ClockTime}\tJob_generated");

        }

        /*Assigning job to the nearest vehicle
         */
        void Job_Allocation()
        {

            if (sys.Job_Pending.Count != 0 && sys.Vehicles_Idle.Count != 0)
            {
                List<int> distance = new List<int>();
                List<int> distance_1 = new List<int>();
                Dictionary<Vehicles, int> Dicdistance = new Dictionary<Vehicles, int>();
                Dictionary<Vehicles, int> Dicdistance_1 = new Dictionary<Vehicles, int>();
                List<TimeSpan> Pending_Time = new List<TimeSpan>();
                Dictionary<Job, TimeSpan> DicPendingTime = new Dictionary<Job, TimeSpan>();

                int index = 0;
                int ind = 0;
                int num = 0;
                int numb = 0;
                for (int i = 0; i < sys.Job_Pending.Count; i++)
                {
                    TimeSpan Pending = ClockTime - sys.Job_Pending[i].Generate_time;
                    Pending_Time.Add(Pending);
                    DicPendingTime.Add(sys.Job_Pending[i], Pending);

                }

                if(Pending_Time.Max() >= MaxPending)
                {
                    for (int j = -1; j < (-1)*sys.Job_Pending.Count; j--)
                    {
                        if (DicPendingTime[sys.Job_Pending[j]] == Pending_Time.Max())
                        {
                            ind = j;
                            break;
                        }
                    }

                    for (int i = 0; i < sys.Vehicles_Idle.Count; i++)
                    {
                        int dis = Math.Abs((sys.Vehicles_Idle[i].Parking_Position.Row_Index - sys.Job_Pending[ind].origin.Row_Index) * 10
                            + (sys.Vehicles_Idle[i].Parking_Position.Column_Index - sys.Job_Pending[ind].origin.Column_Index) * 10);
                        distance.Add(dis);
                        Dicdistance.Add(sys.Vehicles_Idle[i], dis);


                    }

                    for (int k = 0; k < sys.Vehicles_Idle.Count; k++)
                    {
                        if(Dicdistance[sys.Vehicles_Idle[k]] == distance.Min())
                        {
                            index = k;
                            break;
                        }
                    }

                    sys.Pending_Route.Add(sys.Vehicles_Idle[index]);
                    sys.Job_Pending[ind].Job_To_Vehicle = sys.Vehicles_Idle[index];
                    Vehicles vehicle = sys.Vehicles_Idle[index];
                    vehicle.Status = 1;
                    vehicle.Current_Position = sys.Vehicles_Idle[index].Parking_Position;
                    vehicle.Assigned_Job = sys.Job_Pending[ind];

                    sys.Vehicles_Idle.RemoveAt(index);
                    sys.Job_Pending.RemoveAt(ind);

                    Schedule(() => Vehicle_Route(vehicle));
                    Console.WriteLine($"{ClockTime}\tJob_allocated");

                }
                else
                {
                    for (int m = 0; m < sys.Job_Pending.Count; m++)
                    {
                        for (int i = 0; i < sys.Vehicles_Idle.Count; i++)
                        {
                            int dist = Math.Abs((sys.Vehicles_Idle[i].Parking_Position.Row_Index - sys.Job_Pending[m].origin.Row_Index) * 10
                                + (sys.Vehicles_Idle[i].Parking_Position.Column_Index - sys.Job_Pending[m].origin.Column_Index) * 10);
                            distance_1.Add(dist);
                            Dicdistance_1.Add(sys.Vehicles_Idle[i], dist);

                        }
                        if (distance_1.Min() <= Max_Allocation_Distance)
                        {
                            num = m;

                            break;
                        }

                        else
                        {
                            distance_1.RemoveRange(0, distance_1.Count);
                            Dicdistance_1.Clear();
                        }

                    }

                    if (distance_1.Count == 0)
                    {
                        Schedule(Job_Allocation, Exponential.Sample(DefaultRS, TimeSpan.FromSeconds(5)));

                    }
                    else
                    {
                        for (int k = 0; k < sys.Vehicles_Idle.Count; k++)
                        {
                            if (Dicdistance_1[sys.Vehicles_Idle[k]] == distance_1.Min())
                            {
                                numb = k;
                                break;
                            }
                        }

                        sys.Pending_Route.Add(sys.Vehicles_Idle[numb]);
                        sys.Job_Pending[num].Job_To_Vehicle = sys.Vehicles_Idle[numb];
                        Vehicles vehicle = sys.Vehicles_Idle[numb];
                        vehicle.Status = 1;
                        vehicle.Current_Position = sys.Vehicles_Idle[numb].Parking_Position;
                        vehicle.Assigned_Job = sys.Job_Pending[num];

                        sys.Vehicles_Idle.RemoveAt(numb);
                        sys.Job_Pending.RemoveAt(num);

                        Schedule(() => Vehicle_Route(vehicle));
                        Console.WriteLine($"{ClockTime}\tJob_allocated");

                    }
                }

            }
        }

        /*Using A* algorithm to search the shortest path
         */
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

        /*Reserve partial path and delay the time of movement:
         */
        void Reserve_Path(Vehicles vehicle)
        {
            if (vehicle.Path.Count!=0 )
            {
                

                if (vehicle.Path.Count <= Reserve_Size)
                {
                    for (int m = 0; m < vehicle.Path.Count; m++)
                    {
                        if(vehicle.Path[m].Vehcles_Reserving != null)
                        {
                            Schedule(() => Reserve_Path(vehicle), Exponential.Sample(DefaultRS, TimeSpan.FromSeconds(5)));
                        }
                    }
                    for (int k = 0; k < vehicle.Path.Count; k++)
                    {
                        sys.Grids_Reserved.Add(vehicle.Path[k]);
                        
                        sys.RemovePoint(sys.Grids_Releasing, vehicle.Path[k]);
                        sys.RemovePoint(sys.Grids_Reserved, vehicle.Current_Position);
                    }
                    Schedule(() => Complete_Partial_Path(vehicle), Exponential.Sample(DefaultRS, TimeSpan.FromSeconds(5) * vehicle.Path.Count));
                    Console.WriteLine($"{ClockTime}\tReserved_completed");
                }
                else
                {
                    for(int m = 0; m < Reserve_Size; m++)
                    {
                        if (vehicle.Path[m].Vehcles_Reserving != null)
                        {
                            Schedule(() => Reserve_Path(vehicle), Exponential.Sample(DefaultRS, TimeSpan.FromSeconds(5)));
                        }
                    }
                    for (int l = 0; l < Reserve_Size; l++)
                    {
                        sys.Grids_Reserved.Add(vehicle.Path[l]);
                        
                        sys.RemovePoint(sys.Grids_Releasing, vehicle.Path[l]);
                        sys.RemovePoint(sys.Grids_Reserved, vehicle.Current_Position);

                    }
                    Schedule(() => Complete_Partial_Path(vehicle), Exponential.Sample(DefaultRS, TimeSpan.FromSeconds(5) * Reserve_Size));
                    Console.WriteLine($"{ClockTime}\tReserved_completed");
                }


            }
            else
            {
                sys.Vehicles_Pending.Add(vehicle);
                
                Schedule(() => Reserve_Path(vehicle), Exponential.Sample(DefaultRS, TimeSpan.FromSeconds(5)));
                Console.WriteLine($"{ClockTime}\tRedo_reserve_path");
            }
            
            
        }

        /*Complete partial path, and update the current position:
         */
        void Complete_Partial_Path(Vehicles vehicle)
        {
            if (vehicle.Path.Count <= Reserve_Size)
            {
                vehicle.Current_Position = vehicle.Path.Last();
                for(int m = 0; m < vehicle.Path.Count; m++) {

                    vehicle.Path[m].Vehcles_Reserving = null;
                }
                vehicle.Path.Clear();

            }
            else
            {
                vehicle.Current_Position = vehicle.Path[Reserve_Size - 1];
                for (int m = 0; m < Reserve_Size; m++)
                {
                    vehicle.Path[m].Vehcles_Reserving = null;
                }
                vehicle.Path.RemoveRange(0, Reserve_Size - 1);
            }
            
               
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

        /*Job loading
         */
        void Load (Vehicles vehicle)
        {
            vehicle.Status = 2;
            
            Schedule(() => Vehicle_Route(vehicle));
            Console.WriteLine($"{ClockTime}\tLoad_Completed");

        }

        /*Unload the job
         */
        void Unload(Vehicles vehicle)
        {
            vehicle.Status = 0;
            sys.Vehicles_Idle.Add(vehicle);
            vehicle.Parking_Position = vehicle.Assigned_Job.destination;
            job_finished++;
            vehicle.Assigned_Job.Finish_time = ClockTime;
            TimeSpan time_cost = vehicle.Assigned_Job.Finish_time - vehicle.Assigned_Job.Generate_time;
            float time_for_second = (float)Math.Round(time_cost.TotalSeconds);
            time_spent.Add(time_for_second);
            Schedule(Job_Allocation);
            Console.WriteLine($"{ClockTime}\tUnload_Completed");
        }
    }
}

// File name: Program.cs
/*Description:Models running*/
//Tables: Nothing
//Author: Li Yunmiao (Brandon)
//Modification Date: 2022/04/08
//
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
    class Program
    {
        static void Main(string[] args)
        {
            
            float sum_1 = 0;
            float sum_2 = 0;
            float sum_3 = 0;
            float sum_4 = 0;

            
            float avr_1 = 0;
            float avr_2 = 0;
            float avr_3 = 0;
            float avr_4 = 0;

            
            float finish_rate_1 = 0;
            float finish_rate_2 = 0;
            float finish_rate_3 = 0;
            float finish_rate_4 = 0;

            
            float efficiency_1 = 0;
            float efficiency_2 = 0;
            float efficiency_3 = 0;
            float efficiency_4 = 0;


            int Distance_No_Restriction = 600;
            int Reserve_Size_No_Restriction = 60;
            TimeSpan Time_Pending_No_Restriction = new TimeSpan(0,1,0);
            List<float> Reserve_Size_List = new List<float>();
            List<float> Allocation_List = new List<float>();
            List<float> Time_List = new List<float>();


            Dictionary<int, float> Reserve_Dic = new Dictionary<int, float>();
            Dictionary<int, float> Allocation_Dic = new Dictionary<int, float>();
            Dictionary<int, float> Time_Dic = new Dictionary<int, float>();
            List<int> index_0 = new List<int>();
            List<int> index_1 = new List<int>();
            List<int> index_2 = new List<int>();
            int index_a = 0;
            int index_b = 0;
            int index_c = 0;

            /*Original greedy algorithm search*/
            for (int i = 2; i < 61; i++)
            {
                Configuration config = new Configuration();
                Model model = new Model(config);
                model.Reserve_Size = i;
                model.Max_Allocation_Distance = Distance_No_Restriction;
                model.MaxPending = Time_Pending_No_Restriction;
                model.Run(TimeSpan.FromDays(1));
                if (model.time_spent.Count != 0)
                {
                    for (int m = 0; m < model.time_spent.Count; m++)
                    {
                        sum_1 += model.time_spent[m];
                    }
                    avr_1 = (sum_1 / model.time_spent.Count);
                    finish_rate_1 = (model.job_finished / model.job_generate);
                    efficiency_1 = (finish_rate_1 / avr_1);

                }
                Reserve_Size_List.Add(efficiency_1);
                Reserve_Dic.Add(i, efficiency_1);

                index_0.Add(i);

                
                sum_1 = 0;
                

                Console.WriteLine($"End");
            }

            


            for (int n = 0; n < Reserve_Size_List.Count; n++)
            {
                if (Reserve_Dic[index_0[n]] == Reserve_Size_List.Max())
                {
                    index_a = index_0[n];

                    break;
                }
            }


            
            Console.WriteLine($"Best option of size is {index_a}");

            for (int j = 10; j < 610; j+=10)
            {
                Configuration config = new Configuration();
                Model model = new Model(config);
                model.Reserve_Size = index_a;
                model.Max_Allocation_Distance = j;
                model.MaxPending = Time_Pending_No_Restriction;
                model.Run(TimeSpan.FromDays(1));
                if (model.time_spent.Count != 0)
                {
                    for (int m = 0; m < model.time_spent.Count; m++)
                    {
                        sum_2 += model.time_spent[m];
                    }
                    avr_2 = sum_2 / model.time_spent.Count;
                    finish_rate_2 = (model.job_finished / model.job_generate);
                    efficiency_2 = finish_rate_2 / avr_2;

                }
                Allocation_List.Add(efficiency_2);
                Allocation_Dic.Add(j, efficiency_2);


                efficiency_2 = 0;
                sum_2 = 0;
                avr_2 = 0;
                finish_rate_2 = 0;

                index_1.Add(j);

                Console.WriteLine($"End");

            }


            for (int q = 0; q < Allocation_List.Count; q++)
            {
                if (Allocation_Dic[index_1[q]] == Allocation_List.Max())
                {
                    index_b = index_1[q];

                    break;
                }
            }

            Console.WriteLine($"Best option of allocation is {index_b}");


            for (int k = 1; k < 61; k += 1)
            {
                Configuration config = new Configuration();
                Model model = new Model(config);
                model.Reserve_Size = index_a;
                model.Max_Allocation_Distance = index_b;
                model.MaxPending = new TimeSpan(0, k, 0);
                model.Run(TimeSpan.FromDays(1));
                if (model.time_spent.Count != 0)
                {
                    for (int m = 0; m < model.time_spent.Count; m++)
                    {
                        sum_3 += model.time_spent[m];
                    }
                    avr_3 = sum_3 / model.time_spent.Count;
                    finish_rate_3 = (model.job_finished / model.job_generate);
                    efficiency_3 = finish_rate_3 / avr_3;

                }
                Time_List.Add(efficiency_3);
                Time_Dic.Add(k, efficiency_3);

                index_2.Add(k);

                efficiency_3 = 0;
                finish_rate_3 = 0;
                avr_3 = 0;
                sum_3 = 0;

                Console.WriteLine($"End");

            }

            for (int q = 0; q < Time_List.Count; q++)
            {
                if (Time_Dic[index_2[q]] == Time_List.Max())
                {
                    index_c = index_2[q];

                    break;
                }
            }

            /*Search result and statistics of system performance*/
            Console.WriteLine($"Best option of time is {index_c}");

            Configuration config_1 = new Configuration();
            Model model_1 = new Model(config_1);

            model_1.Reserve_Size = index_a;
            model_1.Max_Allocation_Distance = index_b;
            model_1.MaxPending = new TimeSpan(0,0,index_c);
            model_1.Run(TimeSpan.FromDays(1));
            if (model_1.time_spent.Count != 0)
            {
                for (int m = 0; m < model_1.time_spent.Count; m++)
                {
                    sum_4 += model_1.time_spent[m];
                }
                avr_4 = sum_4 / (model_1.time_spent.Count*60);
                finish_rate_4 = (model_1.job_finished / model_1.job_generate);
                efficiency_4 = finish_rate_4 / avr_4;

            }
            Console.WriteLine($"Best combination is {index_a} {index_b} {index_c}");
            Console.WriteLine($"Best efficiency is {efficiency_4}");
            Console.WriteLine($"the cycling time is {avr_4}");
            Console.WriteLine($"the number of job generate is {model_1.job_generate}");
            Console.WriteLine($"the number of job finished is {model_1.job_finished}");
            Console.WriteLine($"the job finish rate is {finish_rate_4}");

        }
    }
}

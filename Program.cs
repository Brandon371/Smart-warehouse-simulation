using System;

namespace Smart_Warehouse_Simulation
{
    class Program
    {
        static void Main(string[] args)
        {
            int sum = 0;
            
            Configuration config = new Configuration();
            Model model = new Model(config);
            model.Run(TimeSpan.FromHours(1));
            for(int i =0; i < model.time_spent.Count; i++)
            {
                sum += model.time_spent[i];
            }
            int avr = sum / model.time_spent.Count;
            Console.WriteLine($"The average complete time in seconds is {avr}");
            Console.WriteLine($"Job completed is {model.job_finished}");
            Console.WriteLine($"Job generated is {model.job_generate}");
            
        }
    }
}

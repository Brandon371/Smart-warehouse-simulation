// File name: System.cs
/*Description:System related algorithm, method and statistics lists */
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

    class System
    {

        public List<Job> Job_Pending { get; set; } = new List<Job>();
        public List<Vehicles> Vehicles_Idle { get; set; } = new List<Vehicles>();

        public List<Grid> Grids_Releasing { get; set; } = new List<Grid>();

        public List<Grid> Grids_Reserved { get; set; } = new List<Grid>();

        public List<Vehicles> Vehicles_Pending { get; set; } = new List<Vehicles>();

        public List<Grid> Obstacle_List { get; set; } = new List<Grid>();

        public List<Job> Job_List { get; set; } = new List<Job>();


        public List<Vehicles> Pending_Route { get; set; } = new List<Vehicles>();

        /*Generate the job
         */
        public void Job_Generate(Random rs, int x, int y)
        {            
            int a = rs.Next(0, x);
            int b = rs.Next(0, y);
            int c = rs.Next(0, x);
            int d = rs.Next(0, y);
            
            Grid origin = new Grid(a, b);
            Grid destination = new Grid(c,d);

            Job job = new Job(origin, destination);

            if (a == c && b == d)
            {
                Job_Generate(rs,x,y);
            }
            else
            {
                if (!(ExistPoint(Obstacle_List, origin)) && !(ExistPoint(Obstacle_List, destination)))
                {
                    Job_List.Add(job);
                    Job_Pending.Add(job);
                    origin.Job_From.Add(job);
                    destination.Job_To.Add(job);
                    
                }
                else
                {
                    Job_Generate(rs,x,y);
                }

            }
        }

        /*A* algorithm
         */
        public bool IsvalidGrid(Grid grid,List<Grid> list_1,List<Grid> list_2)
        {
            if (grid.Row_Index > 29 || grid.Row_Index < 0 || grid.Column_Index > 29 || grid.Column_Index < 0)
            {
                return false;
            }
            if (ExistPoint(Obstacle_List, grid))
            {
                return false;
            }

            if (ExistPoint(list_2,grid))
            {
                return false;
            }
           
            return true;
            
        }



        public List<Grid> SurroundPoints(Grid grid,List<Grid>openList,List<Grid>closeList)
        {
            var surroundPoints = new List<Grid>(9);
            for (int x = grid.Row_Index - 1; x <= grid.Row_Index + 1; x++)
            {
                for (int y = grid.Column_Index - 1; y <= grid.Column_Index + 1; y++)
                {
                    Grid current_grid = new Grid(x, y);
                    if (IsvalidGrid(current_grid, openList,closeList))
                    {
                        surroundPoints.Add(current_grid);
                    }
                }
            }
            return surroundPoints;

        }

        public bool ExistPoint(List<Grid> grids, Grid grid)
        {
            for (int i = 0; i < grids.Count; ++i){

                if (grids[i].Row_Index == grid.Row_Index && grids[i].Column_Index == grid.Column_Index)
                {
                    return true;
                }
            }
            return false;
        }





        public static Grid MinPoint(List<Grid> list)
        {
            Grid tempGrid = list[0];
            foreach (var item in list)
            {
                if (item.F < tempGrid.F)
                    tempGrid = item;
            }
            return tempGrid;
        }


        public Grid GetPoint(List<Grid> points, Grid grid)
        {
            foreach (var item in points)
            {
                if (grid.Row_Index == item.Row_Index && grid.Column_Index == item.Column_Index)
                    return item;
            }
            return null;
        }

        public List<Grid> RemovePoint(List<Grid> points, Grid grid)
        {
            List<Grid> list = new List<Grid>();
            foreach(var item in points)
            {
                if (grid.Row_Index == item.Row_Index && grid.Column_Index == item.Column_Index)
                {
                    points.Remove(item);
                    list = points;
                    return list;
                }
            }
            return points;
        }


        public List<Grid> FindPath(Grid startPoint, Grid endPoint)
        {
            List<Grid> openList = new List<Grid>();
            List<Grid> closeList = new List<Grid>();
            List<Grid> result = new List<Grid>();
            openList.Add(startPoint);
            while (openList.Count > 0)
            {
                Grid point = MinPoint(openList);
                result.Add(point);
                if(point.Row_Index == endPoint.Row_Index && point.Column_Index == endPoint.Column_Index)
                {
                    
                    break;
                }
                openList.Remove(point);
                closeList.Add(point);
                List<Grid> neighbors = SurroundPoints(point,openList,closeList);
                foreach (var item in neighbors)
                {
                    if (!openList.Contains(item))
                    {
                        item.InitGrid(point, endPoint);
                        openList.Add(item);
                    }
                }

                
            }

            

            //result.Remove(startPoint);

            return result;
        }


    }
}

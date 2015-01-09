using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenCycleGame.AI
{
    public class ScheduledAStar : AStar
    {
        private class PathTask
        {
            public List<TileRecord> openList;
            public List<TileRecord> closedList;

            public Tile[,] tiles;
            public Tile startTile;
            public Tile endTile;

            public TileRecord currentRecord;
            public TileRecord currentBestRecord;

            public CostFunction costFunction;
            public Heuristic heuristicFunction;

            public PathTask(Tile[,] tiles, Tile begin, Tile end, CostFunction c, Heuristic h)
            {
                openList = new List<TileRecord>();
                closedList = new List<TileRecord>();

                this.tiles = tiles;
                this.startTile = begin;
                this.endTile = end;
                this.costFunction = c;
                this.heuristicFunction = h;
            }
        }

        private PathTask currentTask;
        private bool _has_task = false;
        public bool hasTask { get { return _has_task; } }
        
        /// <summary>
        /// Starts a new pathfinding task with the given tile array, start and end tiles,
        /// Cost Function, and Heuristic.  It will do one step of work and then return the
        /// best path so far (which should be just the start tile, probably).
        /// </summary>
        public List<Tile> findPath(Tile[,] tiles, Tile begin, Tile end, CostFunction c, Heuristic h)
        {
            foreach (Tile t in tiles)
            {
                if (t != null)
                    t.checkNeighbor();
            }

            currentTask = new PathTask(tiles, begin, end, c, h);

            TileRecord start = new TileRecord();
            start.node = begin;
            start.costSoFar = 0;
            start.estimatedTotalCost = h.Estimate(begin);
            start.parent = null;

            currentTask.openList.Add(start);
            currentTask.currentRecord = start;
            currentTask.currentBestRecord = currentTask.currentRecord;

            _has_task = true;

            stepAStar();

            return getCurrentBestPath();
        }

        /// <summary>
        /// Schedules A* to do maxSteps more steps of the algorithm.  It will stop if it reaches the end
        /// before maxSteps is reached.  It will return the current best path found.
        /// </summary>
        /// <param name="maxSteps"></param>
        /// <returns></returns>
        public List<Tile> schedule(int maxSteps)
        {
            for (int step = 0; step < maxSteps; ++step)
            {
                if (stepAStar())
                {
                    _has_task = false;
                    break;
                }
            }

            return getCurrentBestPath();
        }

        /// <summary>
        /// Runs one step of A*.  It will return true if the algorithm is complete, and
        /// false if the algorithm has more work to do.
        /// </summary>
        private bool stepAStar()
        {
            //Add startRecord to the open list, set current to startRecord
            if (currentTask.openList.Count > 0)
            {
                float endNodeCost;
                float endNodeHeuristic;

                currentTask.currentRecord = SmallestElement(currentTask.openList);
                if (currentTask.currentRecord.node == currentTask.endTile)
                {
                    currentTask.currentBestRecord = currentTask.currentRecord;
                    return true;
                }

                Tile[] connections = currentTask.currentRecord.node.getConnections();
                foreach (Tile endNode in connections)
                {
                    //if the node is null, skip it
                    if (endNode == null) continue;
                    //Uses influence maps AND tiles value to calculate connection cost (cost of traversing this tile)
                    endNodeCost = currentTask.currentRecord.costSoFar +
                        currentTask.costFunction.Calculate(currentTask.currentRecord.node, endNode);
                    if (Contains(currentTask.closedList, endNode))
                    {
                        endNodeRecord = GetElement(currentTask.closedList, endNode);
                        //if route isn't better, skip
                        if (endNodeRecord.costSoFar <= endNodeCost) continue;
                        currentTask.closedList.Remove(endNodeRecord);
                        endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                    }
                    else if (Contains(currentTask.openList, endNode))
                    {
                        endNodeRecord = GetElement(currentTask.openList, endNode);
                        //if route isn't better, skip
                        if (endNodeRecord.costSoFar <= endNodeCost) continue;
                        endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                    }
                    else
                    {
                        //otherwise make a new record
                        endNodeRecord = new TileRecord();
                        endNodeRecord.node = endNode;
                        endNodeHeuristic = currentTask.heuristicFunction.Estimate(endNode);
                    }
                    endNodeRecord.costSoFar = endNodeCost;
                    endNodeRecord.parent = currentTask.currentRecord;
                    endNodeRecord.estimatedTotalCost =
                        endNodeCost + endNodeHeuristic;

                    if (!Contains(currentTask.openList, endNode))
                    {
                        currentTask.openList.Add(endNodeRecord);
                    }
                }
                currentTask.openList.Remove(currentTask.currentRecord);
                currentTask.closedList.Add(currentTask.currentRecord);
                //if there's stil anything in the open list, set current to the smallest element
                if (currentTask.openList.Count > 0) currentTask.currentRecord = SmallestElement(currentTask.openList);

                if (currentTask.currentRecord.estimatedTotalCost <= currentTask.currentBestRecord.estimatedTotalCost)
                {
                    currentTask.currentBestRecord = currentTask.currentRecord;
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the current best path that A* has found and returns it.
        /// </summary>
        public List<Tile> getCurrentBestPath()
        {
            List<Tile> currentBestPath = new List<Tile>();
            TileRecord pathPointer = currentTask.currentBestRecord;

            while (pathPointer.node != currentTask.startTile)
            {
                currentBestPath.Add(pathPointer.node);
                pathPointer = pathPointer.parent;
            }
            currentBestPath.Reverse();

            return currentBestPath;
        }

        /// <summary>
        /// Gets the path that A* is currently considering and returns it.
        /// </summary>
        public List<Tile> getCurrentTestPath()
        {
            List<Tile> currentPath = new List<Tile>();
            TileRecord pathPointer = currentTask.currentRecord;

            while (pathPointer.node != currentTask.startTile)
            {
                currentPath.Add(pathPointer.node);
                pathPointer = pathPointer.parent;
            }
            currentPath.Reverse();

            return currentPath;
        }
    }
}

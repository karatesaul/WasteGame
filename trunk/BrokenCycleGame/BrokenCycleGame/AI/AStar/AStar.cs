using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenCycleGame.AI
{
    public class AStar
    {
        public class TileRecord
        {
            public Tile node;
            public float costSoFar;
            public float estimatedTotalCost;
            public TileRecord parent;

        }


        public TileRecord startRecord = new TileRecord();
        protected TileRecord endNodeRecord;
        public AStar() { }
        


        public List<Tile> findPath(Tile[,] tiles, Tile begin, Tile end, CostFunction c, Heuristic h)
        {
            foreach (Tile t in tiles)
            {
                if(t != null)
                t.checkNeighbor();
            }
            List<TileRecord> open = new List<TileRecord>();
            List<TileRecord> closed = new List<TileRecord>();
            List<Tile> pathway = new List<Tile>();

            startRecord.node = begin;
            startRecord.costSoFar = 0;
            startRecord.estimatedTotalCost = h.Estimate(begin);
            startRecord.parent = null;


            Tile[] connections;
            float endNodeHeuristic;
            float endNodeCost;

            //Add startRecord to the open list, set current to startRecord
            open.Add(startRecord);
            TileRecord current = startRecord;
            while (open.Count > 0)
            {
                current = SmallestElement(open);
                if (current.node == end) break;
                connections = current.node.getConnections();
                foreach (Tile endNode in connections)
                {
                    //if the node is null, skip it
                    if (endNode == null) continue;
                    //Uses influence maps AND tiles value to calculate connection cost (cost of traversing this tile)
                    endNodeCost = current.costSoFar + c.Calculate(current.node, endNode) + 101 - current.node.scorevalue;
                    if (Contains(closed, endNode))
                    {
                        endNodeRecord = GetElement(closed, endNode);
                        //if route isn't better, skip
                        if (endNodeRecord.costSoFar <= endNodeCost) continue;
                        closed.Remove(endNodeRecord);
                        endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                    }
                    else if (Contains(open, endNode))
                    {
                        endNodeRecord = GetElement(open, endNode);
                        //if route isn't better, skip
                        if (endNodeRecord.costSoFar <= endNodeCost) continue;
                        endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                    }
                    else
                    {
                        //otherwise make a new record
                        endNodeRecord = new TileRecord();
                        endNodeRecord.node = endNode;
                        endNodeHeuristic = h.Estimate(endNode);
                    }
                    endNodeRecord.costSoFar = endNodeCost;
                    endNodeRecord.parent = current;
                    endNodeRecord.estimatedTotalCost =
                        endNodeCost + endNodeHeuristic;

                    if (!Contains(open, endNode))
                    {
                        open.Add(endNodeRecord);
                    }
                }
                open.Remove(current);
                closed.Add(current);
                //if there's stil anything in the open list, set current to the smallest element
                if (open.Count > 0) current = SmallestElement(open);
            }
            if (!current.node.Equals(end))
            {
                return null;
            }
            else
            {
                while (current.node != begin)
                {
                    pathway.Add(current.node);
                    current = current.parent;
                }
                pathway.Reverse();
            }
            return pathway;
        }

        //Returns the smallest element of a list
        public TileRecord SmallestElement(List<TileRecord> paths)
        {
            TileRecord smallest = null;
            //Iterate through paths, find smallest cost TileRecord
            //if (paths.Count() > 0)
            //{
                foreach (TileRecord record in paths)
                {
                    if (smallest == null) smallest = record;
                    if (record.estimatedTotalCost < smallest.estimatedTotalCost) smallest = record;
                }
            //}else{
            return smallest;
        }

        //checks if a Tile is in the list
        public bool Contains(List<TileRecord> paths, Tile element)
        {
            //iterate through, compare TileRecord.node to Tile
            foreach (TileRecord tileRecord in paths)
            {
                if (tileRecord.node == element) return true;
            }
            return false;
        }

        //Takes a TileRecord from a list
        public TileRecord GetElement(List<TileRecord> paths, Tile element)
        {
            //Iterates through paths, returns TileRecord if TileRecord.node =tile
            foreach (TileRecord Record in paths)
            {
                if (Record.node == element) return Record;
            }
            return null;
        }
    }
}
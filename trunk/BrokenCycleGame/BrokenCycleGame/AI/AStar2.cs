using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenCycleGame.AI
{
    public class AStar2
    {
        public class TileRecord
        {
            public Tile node;
            public float costSoFar;
            public float cost;
            public float estimatedTotalCost;
            public TileRecord parent;

            /* public TileRecord(Tile node, float costSoFar, float estimatedTotalCost, TileRecord parent)
             {
                 this.node = node;
                 this.costSoFar = costSoFar + node.scorevalue;

                 //Cost is 100 - the nodes value
                 this.cost = 1;//100 - node.scorevalue;
                 this.estimatedTotalCost = estimatedTotalCost;
                 this.parent = parent;
             }*/
        }
        List<TileRecord> open = new List<TileRecord>();
        List<TileRecord> closed = new List<TileRecord>();
        List<Tile> pathway = new List<Tile>();

        public Tile[,] tiles;
        private Tile[] connections;
        Tile begin;
        Tile end;
        Heuristic h;
        public TileRecord startRecord;
        private TileRecord endNodeRecord;
        public AStar2(Tile[,] tiles, Tile begin, Tile end, Heuristic h)
        {
            this.tiles = tiles;
            this.begin = begin;
            this.end = end;
            this.h = h;
            startRecord = new TileRecord();
            startRecord.node = begin;
            startRecord.costSoFar = 0;
            startRecord.estimatedTotalCost = h.Estimate(begin);
            startRecord.parent = null;
            startRecord.cost = 1;
        }

        public List<Tile> findPath(Tile[,] tiles, Tile begin, Tile end, Heuristic h)
        {
            float endNodeHeuristic;
            float endNodeCost;

            open.Add(startRecord);
            TileRecord current = SmallestElement(open);
            while (open.Count > 0)
            {

                if (current.node == end) break;
                connections = current.node.getConnections();
                foreach (Tile endNode in connections)
                {
                    endNodeCost = current.costSoFar + 1;
                    if (Contains(closed, endNode))
                    {
                        endNodeRecord = GetElement(closed, endNode);
                        //if route isn't better, skip
                        if (endNodeRecord.costSoFar <= endNodeCost) continue;
                        closed.Remove(endNodeRecord);
                        endNodeHeuristic = endNodeRecord.cost - endNodeRecord.costSoFar;
                    }
                    else if (Contains(open, endNode))
                    {
                        endNodeRecord = GetElement(open, endNode);
                        //if route isn't better, skip
                        if (endNodeRecord.costSoFar <= endNodeCost) continue;
                        endNodeHeuristic = endNodeRecord.cost - endNodeRecord.costSoFar;
                    }
                    else
                    {
                        //otherwise make a new record
                        endNodeRecord = new TileRecord();
                        endNodeRecord.node = endNode;
                        endNodeHeuristic = h.Estimate(endNode);
                    }
                    endNodeRecord.cost = endNodeCost;
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
                current = SmallestElement(open);
            }
            if (current.node.Equals(end))
            {
                return null;
            }
            else
            {
                while (current != null)
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
            foreach (TileRecord record in paths)
            {
                if (smallest == null) smallest = record;
                if (record.cost < smallest.cost) smallest = record;
            }
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
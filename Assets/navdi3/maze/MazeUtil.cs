namespace navdi3.maze
{

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    using navdi3;
    using navdi3.xxi;

    public static class MazeUtil
    {
        public static List<twin> AStar(AStarMapData data, twin start, twin end)
        {
            return new AStarMapper(data, start, end).path;
        }

        static twin LowestFCost(List<twin> list, MazeBody body, twin star, twin end)
        {
            throw new System.NotImplementedException();
        }
        static float FCost(twin cell, MazeBody body, twin star, twin end)
        {
            throw new System.NotImplementedException();
        }
    }

    public class AStarMapData
    {
        BaseTilemapXXI txxi;
        MazeMaster master;
        MazeBody templateBody;
        twinrect bounds;
        public AStarMapData(BaseTilemapXXI txxi, MazeMaster master, MazeBody templateBody, twinrect bounds)
        {
            this.txxi = txxi;
            this.master = master;
            this.templateBody = templateBody;
            this.bounds = bounds;
        }
        public bool CanMoveFromTo(twin a, twin b)
        {
            if (!this.bounds.Contains(b)) return false; // out of bounds.
            return this.templateBody.CanMoveFromTo(a, b);
        }
    }
    class AStarMapper
    {
        public List<twin> path { get; private set; }
        Dictionary<twin, AStarNode> open, closed;
        public AStarMapper(AStarMapData data, twin start, twin end)
        {
            var start_node = new AStarNode(start);
            var end_node = new AStarNode(start);
            open = new Dictionary<twin, AStarNode>();
            closed = new Dictionary<twin, AStarNode>();

            open.Add(start_node.pos, start_node);

            while(open.Count > 0)
            {
                var here = GetLowestFNode();

                open.Remove(here.pos);
                closed.Add(here.pos, here);

                if (here.pos == end)
                {
                    this.path = new List<twin>();
                    this.path.Add(here.pos);
                    while(here.parent.HasValue)
                    {
                        here = closed[here.parent.Value];
                        this.path.Add(here.pos);
                    }
                    this.path.Reverse();
                    return; // done
                }

                var children = new HashSet<AStarNode>();

                twin.ShuffleCompass();
                foreach(var dir in twin.compass)
                {
                    var there = new AStarNode(here.pos + dir, here.pos);
                    if (data.CanMoveFromTo(here.pos, there.pos)) children.Add(there);
                }

                foreach(var child in children)
                {
                    if (closed.ContainsKey(child.pos)) continue;
                    int g = here.g + 1;
                    int h = (here.pos - child.pos).sqrLength;
                    int f = g + h;
                    if (open.TryGetValue(child.pos, out var existing_node))
                        if (g > existing_node.g)
                            continue;

                    open[child.pos] = new AStarNode(child.pos, child.parent, g, h, f);
                }
            }

        }
        public AStarNode GetLowestFNode()
        {
            float lowest_f_value = float.MaxValue;
            AStarNode lowest_f_node = default(AStarNode);
            foreach(var kvp in open)
            {
                var f = kvp.Value.f;
                if (lowest_f_value > f)
                {
                    lowest_f_value = f;
                    lowest_f_node = kvp.Value;
                }
            }
            return lowest_f_node;
        }
    }
    struct AStarNode
    {
        public twin pos { get; private set; }
        public twin? parent { get; private set; }
        public int g, h, f;
        public AStarNode(twin pos, twin? parent = null, int g = 0, int h = 0, int f = 0)
        {
            this.pos = pos;
            this.parent = parent;
            this.g = g;
            this.h = h;
            this.f = f;
        }
    }

}
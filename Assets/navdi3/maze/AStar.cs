namespace navdi3.maze
{

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	using navdi3;
	using navdi3.xxi;

    public interface IMazeTileGetter
	{
		int Gett(twin cell);
	}

	//   public class MazeTileGetterLambda : IMazeTileGetter
	//{

	//}

	public interface IMazeWalker
	{
		int GetMoveCost(twin a, twin b);
	}
	public class MazeWalkerLambda : IMazeWalker {
		System.Func<twin, twin, int> fn;
		public MazeWalkerLambda(System.Func<twin, twin, int> fnGetMoveCost)
		{
			this.fn = fnGetMoveCost;
		}
		public int GetMoveCost(twin a, twin b)
	    {
            return this.fn(a, b);
	    }
    }

    public class AStarPather
    {
		IMazeWalker[] walkers;
        twinrect bounds;
        public AStarPather(IMazeWalker walker, twinrect? bounds = null)
        {
			this.walkers = new IMazeWalker[bounds.HasValue ? 2 : 1];
			this.walkers[0] = walker;
			if (bounds.HasValue)
				this.walkers[1] = new MazeWalkerLambda((a, b) => { return bounds.Value.Contains(b)?0:int.MaxValue; });
        }
        public int GetMoveCost(twin a, twin b)
        {
            var cost = 0;

			foreach (var walker in walkers) cost = Mathf.Max(cost, walker.GetMoveCost(a, b));

			return cost;
        }
    }
    public class AStarPath
    {
        public List<twin> cells { get; private set; }
        public int cost { get; private set; }
        Dictionary<twin, AStarNode> open, closed;
        public AStarPath(AStarPather pather, twin start, twin end)
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
                    this.cost = here.g;

                    this.cells = new List<twin>();
                    this.cells.Add(here.pos);
                    while(here.parent.HasValue)
                    {
                        here = closed[here.parent.Value];
                        this.cells.Add(here.pos);
                    }
                    this.cells.Reverse();
                    return; // done
                }

                var children = new List<AStarNode>();
                var childrencosts = new List<int>();

                twin.ShuffleCompass();
                foreach(var dir in twin.compass)
                {
                    var there = new AStarNode(here.pos + dir, here.pos);
                    var stepcost = pather.GetMoveCost(here.pos, there.pos);
                    if (stepcost < int.MaxValue && here.g + stepcost >= 0)
                    {
                        children.Add(there);
                        childrencosts.Add(stepcost);
                    }
                }

                for(int i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    var stepcost = childrencosts[i];

                    if (closed.ContainsKey(child.pos)) continue;
                    int g = here.g + stepcost;
                    int h = (here.pos - child.pos).sqrLength;
                    int f = g + h;
                    if (open.TryGetValue(child.pos, out var existing_node))
                        if (g > existing_node.g)
                            continue;

                    open[child.pos] = new AStarNode(child.pos, child.parent, g, h, f);
                }
            }
        }
        AStarNode GetLowestFNode()
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
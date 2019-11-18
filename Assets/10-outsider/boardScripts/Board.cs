namespace ends.outsider
{

    using navdi3;
    using navdi3.maze;

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Board
    {
        public MazeMaster master { get; private set; }
        public outerboardxxi xxi { get; private set; }

        public HashSet<BoardAgent> agents = new HashSet<BoardAgent>();
        public Dictionary<BoardAgent, twin> agent2cellpos = new Dictionary<BoardAgent, twin>();
        public Dictionary<twin, HashSet<BoardAgent>> cellpos2agents = new Dictionary<twin, HashSet<BoardAgent>>();

        public void AddAgent(BoardAgent agent)
        {
            agents.Add(agent);
            UpdateAgentCellPos(agent, agent.my_cell_pos);
        }
        public void UpdateAgentCellPos(BoardAgent agent, twin cellpos)
        {
            if (agents.Contains(agent))
            {
                if (agent2cellpos.TryGetValue(agent, out var prevpos))
                {
                    if (cellpos2agents.ContainsKey(prevpos)) cellpos2agents[prevpos].Remove(agent);
                }

                agent2cellpos[agent] = cellpos;
                if (!cellpos2agents.ContainsKey(cellpos)) cellpos2agents[cellpos] = new HashSet<BoardAgent>();
                cellpos2agents[cellpos].Add(agent);
            } else
            {
                Dj.Warn("UpdateAgentCellPos called on nonadded agent");
            }
        }
        public void RemoveAgent(BoardAgent agent)
        {
            if (agent2cellpos.TryGetValue(agent, out var cellpos))
            {
                cellpos2agents[cellpos].Remove(agent);
                agent2cellpos.Remove(agent);
            }
            agents.Remove(agent);
        }
        public HashSet<BoardAgent> GetAgentsAt(params twin[] cells)
        {
            return GetAgentsAt((IEnumerable<twin>)cells);
        }
        public HashSet<BoardAgent> GetAgentsAt(IEnumerable<twin> cells)
        {
            var agents_at_cells = new HashSet<BoardAgent>();
            foreach (var cell in cells) {
                if (cellpos2agents.TryGetValue(cell, out var agents_at_cell))
                {
                    agents_at_cells.UnionWith(agents_at_cell);
                }
            }
            return agents_at_cells;
        }

        twinrect neibs = new twinrect(-1, -1, 1, 1);
        twinrect extrabounds =  new twinrect(0, 0, 20 - 1, 18 - 1);
        twinrect bounds =       new twinrect(1, 1, 20 - 2, 18 - 2);
        List<twin> open, closed;
        public twin firstCell;

        public Board(MazeMaster master, outerboardxxi xxi)
        {
            this.master = master;
            this.xxi = xxi;

            ResetGen();
        }

        public void ResetGen()
        {

            List<twin> cells = new List<twin>();
            extrabounds.DoEach(cell =>
            {
                xxi.Sett(cell, 1);
            });
            bounds.DoEach(cell => {
                cells.Add(cell);
            });
            Util.shufl(ref cells);

            this.open = new List<twin>();
            this.closed = new List<twin>();

            open.Add(this.firstCell = new twin(
                Random.Range(bounds.min.x, bounds.max.x + 1),
                Random.Range(bounds.min.y, bounds.max.y + 1)
            ));

            for (int i = 0; i < 5; i++) NextGenStep();
        }

        public bool NextGenStep()
        {
            if (open.Count > 0) {
                var open_index = Random.Range(0, open.Count);
                var cell = open[open_index];
                open.RemoveAt(open_index);
                twin.ShuffleCompass();

                foreach (var dir in twin.compass)
                {
                    var ok = true;

                    foreach(var dx in new int[] { -1, 1})
                    {
                        foreach(var dy in new int[] { -1, 1})
                        {
                            if (xxi.Gett(cell + new twin(dx,dy))==0)
                            {
                                // empty corner. only allowed if it connects a proper path:
                                var path = 0;
                                if (xxi.Gett(cell + new twin(dx, 0)) == 0) path++;
                                if (xxi.Gett(cell + new twin(0, dy)) == 0) path++;
                                if (path != 1) ok = false;
                            }
                        }
                    }

                    if (ok && open.Count > 5 && Random.value < .25f) ok = false; 

                    if (ok)
                    {
                        xxi.Sett(cell, 0);

                        var newopen = cell + dir;
                        if (!bounds.Contains(newopen)) continue;
                        if (open.Contains(newopen)) continue;
                        if (closed.Contains(newopen)) continue;
                        open.Add(newopen);

                        closed.Add(cell); // do i need this?
                    } else
                    {
                        return NextGenStep();
                    }
                }

                return true;
            } else
            {

                int deadEndsCleanedUp = 0;

                // clean up dead ends
                bounds.DoEach(cell =>
                {
                    if (xxi.Gett(cell) == 0) {
                        var pathcount = 0;
                        foreach (var dir in twin.compass)
                        {
                            if (xxi.Gett(cell + dir) == 0) pathcount++;
                        }
                        if (pathcount < 2)
                        {
                            xxi.Sett(cell, 1);
                            deadEndsCleanedUp++;
                        }
                    }
                });

                if (deadEndsCleanedUp > 10) return true;

                return false;
            }
        }
    }

}

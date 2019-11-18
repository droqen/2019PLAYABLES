namespace ends.outsider
{

    using navdi3;
    using navdi3.maze;

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BaRobotBoy : BoardAgent
    {
        static BaRobotBoyPather Brain;

        bool targetaction_recoverjuice = false;
        bool targetaction_buildtower = false;
        twin my_target;

        int tower_construction_progress = 0;

        public float juice = 100;
        public float Juice01 { get { return Mathf.Clamp01(juice*0.01f); } }

        public void Setup(Board board, twin cell)
        {
            base.BoardSetup(board, cell);
            if (Brain == null) Brain = new BaRobotBoyPather(xxi);
            SetTarget(Brain.NewBrainBoyTarget());
            targetaction_buildtower = true;
            Brain.plannedTowers.Add(this.my_target);
        }

        public void SetTarget(twin target)
        {
            if (this.targetaction_buildtower) Brain.plannedTowers.Remove(this.my_target);

            this.targetaction_recoverjuice = false;
            this.targetaction_buildtower = false;

            this.my_target = target;
        }

        // robot boy wants to build towers, but not too close to other towers!

        // need a pathfinding module.

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var agent = collision.gameObject.GetComponent<BoardAgent>();
            if (agent != null)
            {
                if (agent is BaRobotTower) { }
                else if (agent is BaRobotBoy) { }
                else this.juice -= 2f; // lose some juice!
                
            }
        }

        private void FixedUpdate()
        {
            this.juice -= 0.05f; // lose juice at a steady rate
            if (this.juice <= 25)
            {
                if (this.juice <= 0)
                    this.juice = 0;

                if (targetaction_buildtower && (tower_construction_progress == 0 || this.juice <= 0))
                {
                    ChooseNewRecoveryTarget();
                }
            }

            this.accelRate = 0.1f + 0.4f * Juice01;
            this.moveSpeed = 5 + 30 * Juice01;
            this.GetComponent<BitsyAni>().speed = 0.01f + 0.09f * Juice01;
            this.GetComponent<SpriteRenderer>().color = juice > 25 ? Color.white : Color.grey;

            if (IsWithinDistOfCentered(this.fullSpeedDist, offset: subtileOffset))
            {
                if (my_cell_pos == my_target)
                {

                    if (targetaction_buildtower)
                    {
                        // well, first, build a tower.
                        BaRobotTower existingTower = null;
                        foreach (var agent in board.GetAgentsAt(my_cell_pos))
                            if (agent is BaRobotTower) existingTower = (BaRobotTower)agent;
                        if (existingTower == null)
                        {
                            this.GetComponent<BitsyAni>().speed = 0.34f;
                            this.juice -= 0.1f; // lose juice faster while building.

                            tower_construction_progress++;
                            if (tower_construction_progress >= 100)
                            {
                                xxi.banks["robot tower"].Spawn<BaRobotTower>(xxi.GetEntLot("robot towers")).Setup(board, my_cell_pos);
                            }
                        }
                        else
                        {
                            tower_construction_progress = 0;
                            // already a tower here? *then* move on to the next one.
                            ChooseNewTowerTarget();

                        }
                    }

                    if (targetaction_recoverjuice)
                    {
                        // ok just sit here :)
                        bool recoveringjuice = false;
                        foreach (var agent in board.GetAgentsAt(my_cell_pos))
                            if (agent is BaRobotTower)
                            {
                                this.juice += 0.3f; // recover juice
                                recoveringjuice = true;
                                if (this.juice >= 100)
                                {
                                    this.juice = 100;
                                    ChooseNewTowerTarget();
                                }
                            }

                        if (!recoveringjuice)
                        {
                            // wtf >:(
                            ChooseNewRecoveryTarget();
                        }
                    }

                }
                else if (TryMove(Brain.StepTowardsTarget(my_cell_pos, my_target)))
                {
                    // ok!
                }
                else
                {
                    // move failed, just sit there and wiggle a bit
                    if (Random.value < .01f) RandomizeSubtileOffset();
                }
            } else
            {
                tower_construction_progress = 0;
                if (bodyStuckFrames > 50)
                {
                    bodyStuckFrames--;

                    ChooseNewTowerTarget();
                    
                    TryMove(-lastMove);
                    bodyStuckFrames = 0;
                    // stuuuck
                }
            }

            SetVelocityApproachTarget();
        }

        public void ChooseNewTowerTarget()
        {
            SnapMyCellPos(); RandomizeSubtileOffset();
            SetTarget(Brain.NewBrainBoyTarget());
            targetaction_buildtower = true;
            Brain.plannedTowers.Add(this.my_target);
        }

        public void ChooseNewRecoveryTarget()
        {
            var path_to_tower = Brain.GetPathToNearestTowerFrom(my_cell_pos);
            if (path_to_tower != null)
            {
                SetTarget(path_to_tower.cells[path_to_tower.cells.Count - 1]);
                targetaction_recoverjuice = true;
            } else
            {
                ChooseNewTowerTarget(); // no towers exist! must build one.
            }
        }

    }

    class BaRobotBoyPather : IMazeWalker {
        outerboardxxi xxi;
        AStarPather robotPather;
        public HashSet<twin> plannedTowers;

        public BaRobotBoyPather(outerboardxxi xxi)
        {
            this.xxi = xxi;
            this.robotPather = new AStarPather(this);
            this.plannedTowers = new HashSet<twin>();
        }

        public int GetMoveCost(twin a, twin b)
        {
            if (xxi.SolidTileIds.Contains(xxi.Gett(b)))
            {
                return int.MaxValue; // unwalkable
            } else
            {
                return 1;
            }
        }

        public twin NewBrainBoyTarget()
        {
            Util.shufl<twin>(ref xxi.openCells);

            // return whichever of 5 cells is the *furthest* from any other existing tower.
            return Util.findbest(xxi.openCells.GetRange(0, 5), cell =>
            {
                var pathToNearestTower = GetPathToNearestTowerFrom(cell, include_planned_towers:true);
                return pathToNearestTower!=null? pathToNearestTower.cost : int.MinValue;
            });
        }

        public AStarPath GetPathToNearestTowerFrom(twin start, bool get_furthest_instead = false, bool include_planned_towers = false)
        {
            var towerpaths = new HashSet<AStarPath>();

            foreach (Transform robotTransform in xxi.GetEntLot("robot towers").transform)
            {
                var tower = robotTransform.GetComponent<BaRobotTower>();
                towerpaths.Add(new AStarPath(robotPather, start, tower.my_cell_pos));
            }

            if (include_planned_towers) foreach(var plannedTowerPos in plannedTowers)
            {
                towerpaths.Add(new AStarPath(robotPather, start, plannedTowerPos));
            }

            if (towerpaths.Count == 0) return null;

            return Util.findbest(towerpaths, path =>
            {
                if (path.cost <= 0 || path.cost == int.MaxValue) return int.MinValue; // never return impossible paths
                return get_furthest_instead? path.cost: -path.cost;
            });
        }

        public twin StepTowardsTarget(twin mover, twin target)
        {
            var path = new AStarPath(this.robotPather, mover, target);
            if (path.cost > 0 && path.cells.Count > 1)
            {
                return path.cells[1] - mover;
            }

            // else: there is no path
            return twin.zero;
        }
    }

}
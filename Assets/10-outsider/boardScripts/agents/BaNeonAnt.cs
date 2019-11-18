namespace ends.outsider
{

    using navdi3;

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BaNeonAnt : BoardAgent
    {

        override public void OnMoved(twin prev_pos, twin target_pos)
        {
            base.OnMoved(prev_pos, target_pos); // important.

            if (this.isWandering && !isGoingHome)
            {
                var index = pathNestToFood.IndexOf(target_pos);
                if (index >= 0) pathNestToFood.RemoveRange(index, pathNestToFood.Count - index);
                else if (pathNestToFood.Count == 0 || (pathNestToFood[pathNestToFood.Count - 1] - target_pos).taxicabLength == 1) pathNestToFood.Add(target_pos);
                else this.isGoingHome = true;

                if (pathNestToFood.Count > maxPathLength) isGoingHome = true;
            } else
            {
                // otherwise the path is locked-in! don't change a thing!
            }
        }

        public BaNeonNest homeNest = null;
        public List<twin> pathNestToFood = new List<twin>();
        public bool isWandering = true;
        public bool isCarryingFood = false;
        public bool isGoingHome = false;
        public int maxPathLength = 5;
        public int frustration = 0;

        public void Setup(Board board, BaNeonNest nest)
        {
            base.BoardSetup(board, nest.my_cell_pos);
            this.homeNest = nest;
            this.pathNestToFood = new List<twin>();
            pathNestToFood.Add(homeNest.my_cell_pos);
        }

        private void FixedUpdate()
        {

            if (IsWithinDistOfCentered(fullSpeedDist, offset: subtileOffset))
            {
                var agents = 0;

                // what's in this cell?
                foreach(var agent in board.GetAgentsAt(my_cell_pos))
                {
                    agents++;
                    if (agent is BaFoodSource)
                    {
                        if (pathNestToFood.Contains(this.my_cell_pos))
                        {
                            // only take food if i know how to get back home with it.

                            ((BaFoodSource)agent).TakeFood();
                            isCarryingFood = true;
                            isWandering = false;
                            isGoingHome = true;
                            frustration = 0;
                        }

                        Dj.Tempf("Ant found food. Path has length {0}", this.pathNestToFood.Count);
                        SnapMyCellPos();
                    }
                    if (agent is BaNeonNest)
                    {
                        this.homeNest = (BaNeonNest)agent;
                        if (isCarryingFood)
                        {
                            this.homeNest.ReceiveFood();
                            isCarryingFood = false;
                        }

                        isGoingHome = false;

                        if (frustration > 100 + this.maxPathLength)
                        {
                            isWandering = true;
                        }

                        frustration = 0;

                        if (isWandering)
                        {
                            this.maxPathLength += Random.Range(1, 3 + 1); // try a longer path
                            this.pathNestToFood.Clear();
                            this.pathNestToFood.Add(my_cell_pos);
                        }

                        SnapMyCellPos();
                    }
                }

                bool do_wander = false;

                var on_path_index = pathNestToFood.IndexOf(my_cell_pos);
                if (isGoingHome && on_path_index > 0 && pathNestToFood.Count > 0 && TryMove( pathNestToFood[on_path_index-1] - my_cell_pos ))
                {
                    // ok, followed path!
                } else if (!isGoingHome && on_path_index >= 0 && on_path_index < pathNestToFood.Count - 1 && TryMove(pathNestToFood[on_path_index + 1] - my_cell_pos ))
                {
                    // ok, followed path!
                } else
                {
                    do_wander = true;
                }

                if (!isCarryingFood && pathNestToFood.Count > 0 && my_cell_pos == pathNestToFood[pathNestToFood.Count-1])
                {
                    frustration = 1000;
                }

                if (do_wander)
                {
                    ChoiceStack<twin> dirs = new ChoiceStack<twin>();
                    if (!isWandering)
                    {
                        // always prefer cells on the path!
                        foreach (var dir in twin.compass)
                            if (pathNestToFood.Contains(my_cell_pos + dir))
                                dirs.Add(dir);
                        dirs.Lock();
                    }
                    // move like a ghost.
                    List<twin> forward_compass = new List<twin>(twin.compass);
                    forward_compass.Remove(-lastMove);
                    dirs.AddManyThenLock(forward_compass);
                    lastMove = dirs.GetFirstTrue(TryMove);
                }
            } else
            {
                SetVelocityApproachTarget();
                GetComponent<SpriteRenderer>().flipX = body.velocity.x < 0;

                if (bodyStuckFrames > 20)
                {
                    frustration++;
                    if (!isCarryingFood && frustration > 20) isGoingHome = true;
                    bodyStuckFrames = 0;
                    SnapMyCellPos(); RandomizeSubtileOffset();
                    if (Random.value < (isGoingHome ?.75f:.25f)) TryMove(-lastMove);
                }
            }

            if (isCarryingFood)
            {
                GetComponent<BitsyAni>().spriteIds = new int[] { 54, 55, };
                GetComponent<BitsyAni>().speed = 0.087f;
            } else
            {
                GetComponent<BitsyAni>().spriteIds = new int[] { 52, 53, };
                GetComponent<BitsyAni>().speed = 0.050f;
            }
        }
    }

}
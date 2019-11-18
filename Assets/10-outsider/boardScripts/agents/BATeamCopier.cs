namespace ends.outsider
{

    using navdi3;

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BATeamCopier : BoardAgent
    {
        //int teamid = 0;
        //public void TeamCopierSetup(Board board, twin cell, int teamid)
        //{
        //    base.BoardSetup(board, cell);
        //    this.teamid = teamid;
        //    if (teamid == 1)
        //    {
        //        GetComponent<BitsyAni>().spriteIds = new int[] { 60, 61 };
        //    }
        //}


        //int waitingFrames = 0;
        //int peacefulPhases = 0;
        //int stuckFrames = 0;
        //int hunger = 0;
        //Vector2 lastVelocity = Vector2.zero;


        //private void FixedUpdate()
        //{

        //    if ((body.velocity - lastVelocity).sqrMagnitude > float.Epsilon)
        //    {
        //        stuckFrames++;
        //    }
        //    else
        //    {
        //        stuckFrames = 0;
        //    }

        //    var toTarget = (Vector2)ToCentered() + targetOffset;
        //    var targetVelocity = Vector2.zero;
        //    if (toTarget.sqrMagnitude > fullSpeedDist * fullSpeedDist)
        //        targetVelocity = toTarget.normalized * moveSpeed;
        //    else
        //        targetVelocity = toTarget / fullSpeedDist * moveSpeed;

        //    body.velocity = body.velocity * .75f + .25f * targetVelocity;
        //    lastVelocity = body.velocity;

        //    waitingFrames--;
        //    if (waitingFrames <= 0)
        //    {

        //        if (CountFriendsAt(my_cell_pos) > 2) if (hunger < 3) hunger++; else Object.Destroy(this.gameObject);
        //        if (CountFriendsAt(my_cell_pos) < 1) if (hunger > 0) hunger--;

        //        if (IsWithinDistOfCentered(fullSpeedDist, offset: targetOffset) && bumpedThisFrame < 2)
        //        {
        //            // then i'm happy
        //            peacefulPhases++;
        //            if (peacefulPhases > 4 + CountFriendsAt(my_cell_pos) * 4) // takes longer to make babies if it's too crowded.
        //            {

        //                board.xxi.banks[gameObject.name].Spawn<BATeamCopier>(board.xxi.GetEntLot("agents")).TeamCopierSetup(board, my_cell_pos, teamid);

        //                peacefulPhases = 0;
        //            }
        //        }
        //        else
        //        {
        //            if (TryMove(Util.findbest(twin.compass, dir =>
        //            {
        //                // count my friends in that direction:
        //                var friends = CountFriendsAt(my_cell_pos + dir);

        //                if (dir == -lastMove)
        //                {
        //                    if (Random.value < .5f) return -1000;
        //                    else friends--;
        //                }

        //                return friends;
        //            }))) targetOffset = new Vector2(Random.Range(-maxOffset, maxOffset), Random.Range(-maxOffset, maxOffset));
        //            // migrate
        //        }

        //        waitingFrames = Random.Range(50, 100);

        //    }

        //    bumpedThisFrame = 0;

        //    //    int party = 0;

        //    //    if (TryMove(Util.findbest(twin.compass, dir =>
        //    //    {

        //    //        // count my friends in that direction:
        //    //        var friends = Util.findall<BoardAgent, List<BoardAgent>>(board.agents, agent =>
        //    //        {
        //    //            return (agent.my_cell_pos == my_cell_pos + dir && agent != this && agent.name == this.name);
        //    //        }).Count;

        //    //        if (dir == -lastMove)
        //    //        {
        //    //            if (socialFrames > 100) return -1000; // i'm NOT ALLOWED to backtrack if my socialFrames are moderate
        //    //            friends--; // otherwise, just slightly disfavour retreating
        //    //        }

        //    //        if (friends > 0) party += friends;

        //    //        if (socialFrames >= 500) return -friends;

        //    //        return friends;
        //    //    })))

        //    //        targetOffset = new Vector2(Random.Range(-maxOffset, maxOffset), Random.Range(-maxOffset, maxOffset));

        //    //    socialFrames = Mathf.Clamp(socialFrames + (party > 3 ? 2 : -1), 0, 500);

        //    //}
        //}

        //int CountFriendsAt(twin cell)
        //{
        //    return Util.findall<BoardAgent, List<BoardAgent>>(board.agents, agent =>
        //    {
        //        return (agent.my_cell_pos == cell && agent != this && agent.name == this.name);
        //    }).Count;
        //}
    }

}
using navdi3;

public class CommuterAnt : BaseCommuter
{
    public bool hasBerry = false;

    twin target = new twin(1, 1);
    twin flip_target = new twin(18, 16);

    private void FixedUpdate()
    {
        if (IsWithinDistOfCentered(2f))
        {
            if (my_cell_pos == target)
            {
                if (hasBerry) LoseBerry(peacefully:true);
                else GainBerry();
            }

            if (TryTakeForcedMove()) { }
            else if (TryTakeNextMoveTo(target)) { }
            else TakeAnyMove();
        }
        else if (TryVelocityUpdate())
        {
            // ok!
            UpdateSpriteFlipX();
        }
        else
        {
            stuckFrames = 0;
            RecenterCellPos();
            lastMove = -lastMove;
            TakeAnyMove();

            //commuterxxi.Instance.AddStink(my_cell_pos, 1); // blockages become stinky.
        }
    }

    public void LoseBerry(bool peacefully=false)
    {
        hasBerry = false;
        target = new twin(1, 1);
        GetComponent<BitsyAni>().spriteIds = new int[] { 50, 51, };

        if (!peacefully) commuterxxi.Instance.AddStink(my_cell_pos, 100);
    }

    public void GainBerry()
    {
        hasBerry = true;
        target = new twin(18, 16);
        GetComponent<BitsyAni>().spriteIds = new int[] { 60, 61, };
    }
}

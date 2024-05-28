namespace HuangD.Sessions;

public class Date
{
    public int Year { get; private set; }

    public int Month { get; private set; }

    public int Day { get; private set; }

    public Date()
    {
        Year = 1;
        Month = 1;
        Day = 1;
    }

    public void DaysInc()
    {
        Day++;

        if (Day > 30)
        {
            Month += 1;
            Day = 1;
        }

        if (Month > 12)
        {
            Year += 1;
            Month = 1;
        }
    }

    public void MonthsInc()
    {
        Month++;

        if (Month > 12)
        {
            Year += 1;
            Month = 1;
        }
    }
}

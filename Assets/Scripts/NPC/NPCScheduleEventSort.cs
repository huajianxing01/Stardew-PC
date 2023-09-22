using System.Collections.Generic;
using Unity.VisualScripting;

public class NPCScheduleEventSort : IComparer<NPCScheduleEvent>
{
    public int Compare(NPCScheduleEvent event1, NPCScheduleEvent event2)
    {
        if (event1?.Time == event2?.Time)
        {
            if(event1?.priority < event2?.priority)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
        else if(event1?.Time > event2?.Time)
        {
            return 1;
        }
        else if(event1?.Time < event2?.Time)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}

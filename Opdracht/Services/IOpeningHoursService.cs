using Opdracht.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Opdracht.Services
{
    public interface IOpeningHoursService
    {
        bool IsOpenToday();
        OpeningHours GetClosestToOpen();
        Double TimeBetweenDates(OpeningHours storeOpen);

    }
}

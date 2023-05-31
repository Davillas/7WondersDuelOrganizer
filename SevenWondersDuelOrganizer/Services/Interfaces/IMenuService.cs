using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SevenWondersDuelOrganizer.Models;

namespace SevenWondersDuelOrganizer.Services.Interfaces
{
    public interface IMenuService
    {
        Task<List<NavPanelElement>> LoadMenuTabsAsync();
    }
}

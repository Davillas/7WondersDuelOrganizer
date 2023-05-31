using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SevenWondersDuelOrganizer.Models;
using SevenWondersDuelOrganizer.Services.Interfaces;

namespace SevenWondersDuelOrganizer.Services
{
    internal class MenuService : IMenuService
    {
        public Task<List<NavPanelElement>> LoadMenuTabsAsync() =>
        

                Task.FromResult(new List<NavPanelElement>(new[]
                {
                    new NavPanelElement("New Game", "Plus"),
                    new NavPanelElement("Players", "Minus"),
                    new NavPanelElement("Statistics", "About"),
                    new NavPanelElement("Settings", "Plus"),
                    new NavPanelElement("About", "About"),
                }));
        
    }
}

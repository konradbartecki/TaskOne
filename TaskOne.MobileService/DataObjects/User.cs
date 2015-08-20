using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service;

namespace TaskOne.MobileService.DataObjects
{
    public class User : EntityData
    {
        public string Email { get; set; }
    }
}

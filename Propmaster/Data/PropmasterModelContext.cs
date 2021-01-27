using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Propmaster.Models;

namespace Propmaster.Data
{
    public class PropmasterModelContext : DbContext
    {
        public PropmasterModelContext (DbContextOptions<PropmasterModelContext> options)
            : base(options)
        {
        }

        public DbSet<Propmaster.Models.Booking> Booking { get; set; }
    }
}

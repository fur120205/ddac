using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Propmaster.Areas.Identity.Data;
using Propmaster.Data;

[assembly: HostingStartup(typeof(Propmaster.Areas.Identity.IdentityHostingStartup))]
namespace Propmaster.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<PropmasterContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("PropmasterContextConnection")));

                services.AddDefaultIdentity<PropmasterUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<PropmasterContext>();
            });
        }
    }
}
using Application.Aircrafts;
using Application.Aircrafts.Dto;
using AutoMapper;
using Castle.Core.Logging;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var elasticClient = services.GetRequiredService<ElasticClient>();
                    var mapper = services.GetRequiredService<IMapper>();
                    var context = services.GetRequiredService<DataContext>();
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();

                    context.Database.Migrate();
                    Seed.SeedData(context, userManager).Wait();

                    var aircrafts = context.Aircraft.ToList();
                    var aircraftsDto = mapper.Map<List<AircraftDto>>(aircrafts);

                    var deleteDocsFromIndexRes = elasticClient.DeleteByQuery<AircraftDto>(d => d.MatchAll());
                    if (deleteDocsFromIndexRes.OriginalException != null) throw deleteDocsFromIndexRes.OriginalException;
                    var addDocsInIndexRes = elasticClient.IndexMany(aircraftsDto);

                    if (addDocsInIndexRes.OriginalException != null) throw addDocsInIndexRes.OriginalException;
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured during program launch");
                }
                host.Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

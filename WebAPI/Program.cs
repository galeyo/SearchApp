using Application.Aircrafts;
using Application.Aircrafts.Dto;
using AutoMapper;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Hosting;
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
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var client = services.GetRequiredService<ElasticClient>();
                    var mapper = services.GetRequiredService<IMapper>();
                    var context = services.GetRequiredService<DataContext>();

                    var aircrafts = context.Aircraft.ToList();
                    var aircraftsDto = mapper.Map<List<AircraftDto>>(aircrafts);

                    var deleteDocsFromIndexRes = client.DeleteByQuery<AircraftDto>(d => d.MatchAll());
                    if (deleteDocsFromIndexRes.OriginalException != null) throw deleteDocsFromIndexRes.OriginalException;
                    var addDocsInIndexRes = client.IndexMany(aircraftsDto);

                    if (addDocsInIndexRes.OriginalException != null) throw addDocsInIndexRes.OriginalException;
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured during adding docs in ES index");
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

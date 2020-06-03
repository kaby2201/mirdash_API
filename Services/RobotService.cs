using System;
using System.Linq;
using System.Text;
using backend.Data;
using backend.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace backend.Services
{
    public class RobotService : RobotServiceTask, IHostedService, IDisposable
    {
        private Timer _timer;
        
        
        public RobotService(ILogger<RobotService> logger, IHttpClientFactory clientFactory, IServiceProvider services)
            : base(logger, clientFactory, services)
        {
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(BackgroundWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private async void LoadData()
        {
            ApplicationDbContext db = null;
            try
            {
                using var serviceScope = Services.CreateScope();
                db = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                Hosts = await db.Robots.ToListAsync();
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Invalid operation at Services.RobotService.LoadData()");
            }

            if (db != null)
            {
                await db.DisposeAsync();
            }
        }

        private async void BackgroundWork(object state)
        {
            ApplicationDbContext db = null;
            try
            {
                LoadData();
                db = Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
                foreach (var host in Hosts)
                {
                    await LoadStatus(host, db);
                    await LoadMissions(host, db);
                    //await PostToQueue(host, db);
                    //await GetQueue(host, db);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception at Services.RobotService.BackgroundWork");
            }

            if (db != null)
            {
                await db.DisposeAsync();
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
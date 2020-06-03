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
    public abstract class RobotServiceTask
    {
        // JsonSerializer configurations
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = false,
            IgnoreNullValues = true,
            WriteIndented = false
        };

        public readonly ILogger<RobotService> _logger;
        internal IServiceProvider Services { get; set; }
        private readonly HttpClient _client;
        public List<Robot> Hosts { get; set; }

        public RobotServiceTask(ILogger<RobotService> logger, IHttpClientFactory clientFactory,
            IServiceProvider services)
        {
            _logger = logger;
            Services = services;
            _client = clientFactory.CreateClient();
        }

        protected async Task LoadMissions(Robot host, ApplicationDbContext db)
        {
            try
            {
                _client.DefaultRequestHeaders.Accept.Clear();
                var response = await _client.SendAsync(HttpRequestMessage(host, "/missions"));

                if (response.IsSuccessStatusCode)
                {
                    await using var responseStream = await response.Content.ReadAsStreamAsync();

                    var missions =
                        await JsonSerializer.DeserializeAsync<List<Mission>>(responseStream, _options);
                    foreach (var mission in missions)
                    {
                        mission.RobotId = host.Id;
                        if (!db.Missions.Any(m => m.Name.Contains(mission.Name)
                                                  && m.RobotId.Equals(mission.RobotId)))
                            db.Missions.Add(mission);
                    }

                    host.IsOnline = true;
                    db.Robots.Update(host);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                SetRobotOffline(host, db, e);
            }
        }

        protected async Task LoadStatus(Robot host, ApplicationDbContext db)
        {
            // No need to send request when robot is offline
            if (!host.IsOnline) return;

            try
            {
                _client.DefaultRequestHeaders.Accept.Clear();
                var response = await _client.SendAsync(HttpRequestMessage(host, "/status"));

                if (response.IsSuccessStatusCode)
                {
                    await using var responseStream = await response.Content.ReadAsStreamAsync();
                    var status = await JsonSerializer.DeserializeAsync<Status>(responseStream, _options);

                    status.RobotId = host.Id;
                    host.StateText = status.StateText;
                    host.Hostname = status.RobotName;
                    host.IsOnline = true;

                    try
                    {
                        var previousStatus = db.Statuses.First(s => s.SerialNumber.Equals(status.SerialNumber));
                        previousStatus.Errors = status.Errors;
                        previousStatus.Footprint = status.Footprint;
                        previousStatus.Moved = status.Moved;
                        previousStatus.Position = status.Position;
                        previousStatus.Uptime = status.Uptime;
                        previousStatus.Velocity = status.Velocity;
                        previousStatus.BatteryPercentage = status.BatteryPercentage;
                        previousStatus.BatteryTimeRemaining = status.BatteryTimeRemaining;
                        previousStatus.MapId = status.MapId;
                        previousStatus.MissionText = status.MissionText;
                        previousStatus.ModeId = status.ModeId;
                        previousStatus.ModeText = status.ModeText;
                        previousStatus.RobotModel = status.RobotModel;
                        previousStatus.RobotName = status.RobotName;
                        previousStatus.StateId = status.StateId;
                        previousStatus.SessionId = status.SessionId;
                        previousStatus.StateText = status.StateText;
                        previousStatus.UserPrompt = status.UserPrompt;
                        previousStatus.MissionQueueId = status.MissionQueueId;
                        previousStatus.MissionQueueUrl = status.MissionQueueUrl;
                        previousStatus.DistanceToNextTarget = status.DistanceToNextTarget;
                        db.Update(previousStatus);
                    }
                    catch (InvalidOperationException) // No status yet
                    {
                        db.Statuses.Add(status);
                    }

                    db.Robots.Update(host);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical("The Robot may be is offline: No status could be loaded");
            }
        }

        protected async Task PostToQueue(Robot host, ApplicationDbContext db)
        {
            if (!host.IsOnline) return;
            var IsAvailible = db.MissionQueueRequests.Any(s => s.RobotId.Equals(host.Id));
            if (!IsAvailible)
            {
                return;
            }

            var res = db.MissionQueueRequests.First(
                s => s.RobotId.Equals(host.Id));
            try
            {
                var json = JsonSerializer.Serialize(res, _options);
                var details = JsonConvert.DeserializeObject(json);
                // var client = new HttpClient();
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", host.Token);
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                await _client.PostAsJsonAsync(host.BasePath + "/mission_queue", details);

                var id = await db.MissionQueueRequests.FindAsync(res.Id);
                db.MissionQueueRequests.Remove(id);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
            }
        }

        protected async Task GetQueue(Robot host, ApplicationDbContext db)
        {
            if (!host.IsOnline) return;

            try
            {
                _client.DefaultRequestHeaders.Accept.Clear();
                var httpResponse = await _client.SendAsync(HttpRequestMessage(host, "/mission_queue"));

                if (httpResponse.IsSuccessStatusCode)
                {
                    await using var responseStream = await httpResponse.Content.ReadAsStreamAsync();

                    var response =
                        await JsonSerializer.DeserializeAsync<List<MissionQueuesResponse>>(responseStream, _options);

                    foreach (var queuesResponse in response)
                    {
                        queuesResponse.RobotId = host.Id;
                        var isAvailable = db.MissionQueuesResponse
                            .Where(s => s.Id.Equals(queuesResponse.Id))
                            .Any(s => s.RobotId.Equals(host.Id));

                        if (isAvailable)
                            db.MissionQueuesResponse.Update(new MissionQueuesResponse
                            {
                                Id = queuesResponse.Id,
                                Robot = host,
                                RobotId = host.Id,
                                State = queuesResponse.State,
                                Url = queuesResponse.Url
                            });
                        else
                        {
                            var missq = new MissionQueuesResponse
                            {
                                Id = queuesResponse.Id,
                                Robot = host,
                                RobotId = host.Id,
                                State = queuesResponse.State,
                                Url = queuesResponse.Url
                            };

                            await db.MissionQueuesResponse.AddAsync(missq);
                        }
                    }

                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical("The Robot may be is offline");
            }
        }

        private void SetRobotOffline(Robot host, ApplicationDbContext db, Exception e)
        {
            var robot = db.Robots.Find(host.Id);
            robot.IsOnline = false;
            robot.StateText = "Offline";
            db.Update(robot);
            db.SaveChanges();
        }

        private static HttpRequestMessage HttpRequestMessage(Robot host, string path)
        {
            // This method will be injected by the caller with appropriated data ex: requestUrl and authorization
            var request = new HttpRequestMessage(HttpMethod.Get, host.BasePath + path);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization",
                $"Basic {host.Token}");
            return request;
        }
    }
}
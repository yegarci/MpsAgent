﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace LocalMultiplayerAgent.Controllers
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Gaming.AgentInterfaces;
    using Microsoft.Azure.Gaming.LocalMultiplayerAgent;
    using Microsoft.Azure.Gaming.VmAgent.Model;
    using Newtonsoft.Json;

    public class SessionHostController : Controller
    {
        private const int DefaultHeartbeatIntervalMs = 1000;
        private static readonly ConcurrentDictionary<string, int> HeartBeatsCount = new ConcurrentDictionary<string, int>();

        [HttpPost]

        [Route("v1/titles/{titleId}/clusters/{sessionHost}/instances/{instanceId}/heartbeat")]
        [Route("v1/titles/{titleId}/sessionHost/{sessionHost}/instances/{instanceId}/heartbeat")]
        public async Task<IActionResult> ProcessHeartbeatLegacy(string titleId, string sessionHost, string instanceId,
            [FromBody] LegacyGameInfo heartbeatRequest)
        {

            // Do all the work with the newer GameInfo object, and just convert back to a LegacyGameInfo before responding
            IActionResult result = await ProcessHeartbeat(instanceId, heartbeatRequest.ToSessionHostHeartbeatInfo());

            if (result is OkObjectResult response)
            {
                LegacyGameInfo legacyResponse = LegacyGameInfo.FromSessionHostHeartbeatInfo(response.Value as SessionHostHeartbeatInfo, titleId);
                response.Value = legacyResponse;
            }

            return result;
        }

        [HttpPost]

        [Route("v1/sessionHosts/{sessionHostId}/heartbeats")]
        public async Task<IActionResult> ProcessHeartbeat(string sessionHostId,
            [FromBody] SessionHostHeartbeatInfo heartbeatRequest)
        {
            await Task.Delay(1);
            SessionHostStatus currentGameState = heartbeatRequest.CurrentGameState;
            Operation op = Operation.Continue;
            SessionConfig config = null;
            Console.WriteLine($"CurrentGameState: {heartbeatRequest.CurrentGameState}");
            if (currentGameState == SessionHostStatus.Terminated || currentGameState == SessionHostStatus.Terminating)
            {
                HeartBeatsCount.TryRemove(sessionHostId, out _);
            }
            else if (HeartBeatsCount.TryGetValue(sessionHostId, out int numHeartBeats))
            {
                if (numHeartBeats >= Globals.Settings.NumHeartBeatsForTerminateResponse)
                {
                    op = Operation.Terminate;
                }
                else if (numHeartBeats >= Globals.Settings.NumHeartBeatsForActivateResponse && currentGameState == SessionHostStatus.StandingBy)
                {
                    op = Operation.Active;
                    config = Globals.SessionConfig;
                }

                HeartBeatsCount[sessionHostId]++;
            }
            else
            {
                HeartBeatsCount.TryAdd(sessionHostId, 1);
            }

            return Ok(new SessionHostHeartbeatInfo
            {
                CurrentGameState = currentGameState,
                NextHeartbeatIntervalMs = DefaultHeartbeatIntervalMs,
                Operation = op,
                SessionConfig = config
            });
        }

        [HttpPatch]

        [Route("v1/sessionHosts/{sessionHostId}")]
        public Task<IActionResult> ProcessHeartbeatV1(
            string sessionHostId,
            [FromBody] SessionHostHeartbeatInfo heartbeatRequest)
        {
            // To be removed once we update the new GSDK
            return ProcessHeartbeat(sessionHostId, heartbeatRequest);
        }
    }
}
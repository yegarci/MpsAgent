﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Azure.Gaming.AgentInterfaces
{
    using System;

    /// <summary>
    /// The host of a game session
    /// </summary>
    public class SessionHost
    {
        /// <summary>
        /// Identifier (GUID) of the session (provided by client).
        /// </summary>
        public Guid? SessionId { get; set; }

        /// <summary>
        /// Identity of the session host (container), generated by VmAgent.
        /// </summary>
        public string SessionHostId { get; set; }

        /// <summary>
        /// Identity of the virtual machine running the session host.
        /// </summary>
        public string VmId { get; set; }

        /// <summary>
        /// IP Address (v4)
        /// </summary>
        public string IPv4Address { get; set; }

        /// <summary>
        /// The fully qualified domain name for the session host.
        /// </summary>
        public string FQDN { get; set; }

        /// <summary>
        /// IP Address (v6)
        /// </summary>
        public string IPv6Address { get; set; }

        /// <summary>
        /// Named public ports
        /// </summary>
        public Port[] Ports { get; set; }

        /// <summary>
        /// Region
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// SecureContext
        /// </summary>
        public string SecureContext { get; set; }

        /// <summary>
        /// Current state
        /// </summary>
        public SessionHostStatus State { get; set; }

        /// <summary>
        /// Identity of all currently connected players
        /// </summary>
        public ConnectedPlayer[] ConnectedPlayers { get; set; }

        /// <summary>
        /// The time at which the <see cref="State"/> had last changed.
        /// Nullable since the type was introduced later and existing VMs will not emit it.
        /// </summary>
        public DateTime? LastStateTransitionTimeUtc { get; set; }

        /// <summary>
        /// The build id of the session.
        /// In case of regular allocation this will be the build id specified.
        /// In case of allocation using aliases this will specify what build id from the alias got the allocation.
        /// </summary>
        public Guid? BuildId { get; set; }

        /// <summary>
        /// Used by some legacy games such as Forza 5 for security handshake with the game client.
        /// </summary>
        public string SecureDeviceAddress { get; set; }
    }
}

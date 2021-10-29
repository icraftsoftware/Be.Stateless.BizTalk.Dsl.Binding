#region Copyright & License

// Copyright © 2012 - 2021 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.BizTalk.Adapter.Sftp;
using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class SftpAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// Use the SFTP adapter to send and receive messages from a secure FTP server using the SSH file transfer protocol.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/sftp-adapter">SFTP Adapter</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/sftp-adapter#configure-the-receive-location">Configure the receive location</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/sftp-adapter#frequently-asked-questions">Frequently asked questions</seealso>
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public class Inbound : SftpAdapter<SftpRLConfig>, IInboundAdapter
		{
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public Inbound()
			{
				// Polling Settings
				PollingInterval = TimeSpan.FromSeconds(5);

				// Proxy Settings
				ProxyPort = 1080;
				ProxyType = ProxyType.None;

				// Security Settings
				AcceptAnySshServerHostKey = false;
				ClientAuthenticationMode = ClientAuthenticationMode.Password;
				EncryptionCipher = EncryptionCipher.Auto;

				// SSH Server Settings
				Port = 22;
			}

			public Inbound(Action<Inbound> adapterConfigurator) : this()
			{
				if (adapterConfigurator == null) throw new ArgumentNullException(nameof(adapterConfigurator));
				adapterConfigurator(this);
			}

			#region Base Class Member Overrides

			protected override string GetAddress()
			{
				return $"sftp://{ServerAddress}:{Port}/{FolderPath}/{FileMask}";
			}

			protected override void Validate()
			{
				// see Microsoft.BizTalk.Adapter.Sftp.SftpRLConfig.Validate()
				// see Microsoft.BizTalk.Adapter.Wcf.Config.RLConfig.ValidateBinding()
				var binding = AdapterConfig.CreateBinding(new SftpRHConfig());
				binding.CreateBindingElements();
				// see Microsoft.BizTalk.Adapter.Wcf.Config.RLConfig.ValidateAddress()
				AdapterConfigValidationHelper.ValidateAddress(GetAddress(), UriKind.Absolute, binding.Scheme, null);
			}

			#endregion

			#region Polling Settings

			/// <summary>
			/// If <see cref="RetainAfterDownload"/> is set to <c>True</c>, this property determines whether a change in file
			/// timestamp will trigger a redownload of the file.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
			/// </remarks>
			public bool EnableTimeComparison
			{
				get => AdapterConfig.EnableTimeComparison;
				set => AdapterConfig.EnableTimeComparison = value;
			}

			/// <summary>
			/// Specify the intervals at which the adapter will poll the server.
			/// </summary>
			/// <remarks>
			/// <para>
			/// To poll continuously, set this value to <see cref="TimeSpan.Zero"/>.
			/// </para>
			/// <para>
			/// It defaults to <c>5</c> seconds.
			/// </para>
			/// </remarks>
			public TimeSpan PollingInterval
			{
				get => BuildTimeSpan(AdapterConfig.PollingInterval, AdapterConfig.PollingIntervalUnit);
				set
				{
					UnbuildTimeSpan(
						value,
						(i, u) => {
							AdapterConfig.PollingInterval = i;
							AdapterConfig.PollingIntervalUnit = u;
						});
				}
			}

			/// <summary>
			/// The interval after which the file will be downloaded again. Applicable if <see cref="RetainAfterDownload"/> is
			/// <c>True</c>, and <see cref="EnableTimeComparison"/> is <c>False</c>. If set to -1, the file will not be downloaded
			/// again.
			/// </summary>
			/// <remarks>
			/// <para>
			/// It defaults to <c>0</c>.
			/// </para>
			/// <para>
			/// <list type="bullet">
			/// <item>
			/// <c>-1</c> indicates that the adapter will not download files again.
			/// </item>
			/// <item>
			/// <c>0</c> indicates that the adapter will download the file in each polling cycle.
			/// </item>
			/// </list>
			/// </para>
			/// </remarks>
			public int RedownloadInterval
			{
				get => AdapterConfig.RedownloadInterval;
				set => AdapterConfig.RedownloadInterval = value;
			}

			/// <summary>
			/// Specifies whether the adapter will retain a file from the SFTP server after downloading it.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
			/// </remarks>
			public bool RetainAfterDownload
			{
				get => AdapterConfig.RetainAfterDownload;
				set => AdapterConfig.RetainAfterDownload = value;
			}

			#endregion

			#region Other Settings

			/// <summary>
			/// Specify the maximum number of concurrent connections that can be opened to the server.
			/// </summary>
			/// <remarks>
			/// This setting is per server and per receive location. Consider the following scenarios:
			/// <list type="bullet">
			/// <item>
			/// There are two receive locations that have the same configuration property values, including the ConnectionLimit
			/// property set to the same value. For example, the property is set to 6. In this situation, there is one connection
			/// pool (with 6 available connections) that is used by both receive locations.
			/// </item>
			/// <item>
			/// There are two receive locations configured with same configuration values, and have the ConnectionLimit property
			/// set to different values. For example, ReceiveLocation1 property is set to 6 and ReceiveLocation2 property is set
			/// to 5. In this situation, each receive location has its own connection pool with its own available connections.
			/// ReceiveLocation1 connection pool has 6 available connections. ReceiveLocation2 connection pool has 5 available
			/// connections.
			/// </item>
			/// </list>
			/// </remarks>
			public int ConnectionLimit
			{
				get => AdapterConfig.ConnectionLimit;
				set => AdapterConfig.ConnectionLimit = value;
			}

			/// <summary>
			/// The full path to create a client-side log file. Use this log file to troubleshoot any errors.
			/// </summary>
			public string Log
			{
				get => AdapterConfig.Log;
				set => AdapterConfig.Log = value;
			}

			/// <summary>
			/// The maximum connection reuse time allows connections to be gracefully closed and removed from the pool after a
			/// connection has been in use for a specific amount of time.
			/// </summary>
			/// <remarks>
			/// <para>
			/// A value of <c>0</c> or less indicates that this behaviour is disabled.
			/// </para>
			/// <para>
			/// It defaults to <c>0</c>.
			/// </para>
			/// </remarks>
			public TimeSpan MaxConnectionReuseTime
			{
				get => TimeSpan.FromSeconds(AdapterConfig.MaxConnectionReuseTimeInSeconds);
				set => AdapterConfig.MaxConnectionReuseTimeInSeconds = (int) value.TotalSeconds;
			}

			/// <summary>
			/// Specify the transfer mode.
			/// </summary>
			/// <remarks>
			/// When <see cref="TransferMode"/> is set to <see cref="Microsoft.BizTalk.Adapter.Sftp.TransferMode.ASCII"/>, it
			/// allows to not only transfer but also to convert text files to the format used by the target platform.
			/// </remarks>
			public TransferMode TransferMode
			{
				get => AdapterConfig.TransferMode;
				set => AdapterConfig.TransferMode = value;
			}

			#endregion

			#region Proxy Settings

			/// <summary>
			/// Specifies either the DNS name or the IP address of the proxy server.
			/// </summary>
			public string ProxyAddress
			{
				get => AdapterConfig.ProxyAddress;
				set => AdapterConfig.ProxyAddress = value;
			}

			/// <summary>
			/// Specifies the port for the proxy server.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>1080</c>.
			/// </remarks>
			public int ProxyPort
			{
				get => AdapterConfig.ProxyPort;
				set => AdapterConfig.ProxyPort = value;
			}

			/// <summary>
			/// Specifies the protocol used by the proxy server.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="Microsoft.BizTalk.Adapter.Sftp.ProxyType.None"/>.
			/// </remarks>
			public ProxyType ProxyType
			{
				get => AdapterConfig.ProxyType;
				set => AdapterConfig.ProxyType = value;
			}

			/// <summary>
			/// Specifies the username for the proxy server.
			/// </summary>
			public string ProxyUserName
			{
				get => AdapterConfig.ProxyUserName;
				set => AdapterConfig.ProxyUserName = value;
			}

			/// <summary>
			/// Specifies the password for the proxy server.
			/// </summary>
			public string ProxyPassword
			{
				get => AdapterConfig.ProxyPassword;
				set => AdapterConfig.ProxyPassword = value;
			}

			#endregion

			#region Security Settings

			/// <summary>
			/// If set to <c>True</c>, the receive location accepts any SSH public host key from the server. If set to
			/// <c>False</c>, the receive location uses the fingerprint of the server for authentication. You specify the
			/// fingerprint in the <see cref="SshServerHostKeyFingerPrint"/> property.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
			/// </remarks>
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public bool AcceptAnySshServerHostKey
			{
				get => AdapterConfig.AccessAnySSHServerHostKey;
				set => AdapterConfig.AccessAnySSHServerHostKey = value;
			}

			/// <summary>
			/// Specifies the authentication method that the receive location uses for authenticating the client to the SSH
			/// Server.
			/// </summary>
			/// <remarks>
			/// <para>
			/// If set to Password, you must specify the value in the <see
			/// cref="Microsoft.BizTalk.Adapter.Sftp.ClientAuthenticationMode.Password"/> property. If set to <see
			/// cref="Microsoft.BizTalk.Adapter.Sftp.ClientAuthenticationMode.PublicKeyAuthentication"/>, you must specify the
			/// private key of the user in the <see cref="PrivateKeyFile"/> property. If set to <see
			/// cref="Microsoft.BizTalk.Adapter.Sftp.ClientAuthenticationMode.MultiFactorAuthentication"/> you must provide <see
			/// cref="UserName"/> with its <see cref="Password"/> and <see cref="PrivateKeyFile"/>. Additionally, if the private
			/// key is protected by a password, specify the password as well for the <see cref="PrivateKeyPassword"/> property.
			/// </para>
			/// <para>
			/// It defaults to <see cref="Microsoft.BizTalk.Adapter.Sftp.ClientAuthenticationMode.Password"/>.
			/// </para>
			/// </remarks>
			public ClientAuthenticationMode ClientAuthenticationMode
			{
				get => AdapterConfig.ClientAuthenticationMode;
				set => AdapterConfig.ClientAuthenticationMode = value;
			}

			/// <summary>
			/// Specify the kind of encryption cipher.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="Microsoft.BizTalk.Adapter.Sftp.EncryptionCipher.Auto"/>.
			/// </remarks>
			public EncryptionCipher EncryptionCipher
			{
				get => AdapterConfig.EncryptionCipher;
				set => AdapterConfig.EncryptionCipher = value;
			}

			/// <summary>
			/// Comma-separated list of KEX preference order. Token WARN is used to delimit substandard KEXes. Example:
			/// ecdh,dh-gex-sha1,dh-group14-sha1,rsa,WARN,dh-group1-sha1. Visit WinSCP website for latest information.
			/// </summary>
			public string KeyExchangeAlgorithmSelectionPolicy
			{
				get => AdapterConfig.KexPolicy;
				set => AdapterConfig.KexPolicy = value;
			}

			/// <summary>
			/// Specify the private key for the SFTP user if you set the <see cref="ClientAuthenticationMode"/> to <see
			/// cref="Microsoft.BizTalk.Adapter.Sftp.ClientAuthenticationMode.PublicKeyAuthentication"/>.
			/// </summary>
			public string PrivateKeyFile
			{
				get => AdapterConfig.PrivateKeyFile;
				set => AdapterConfig.PrivateKeyFile = value;
			}

			/// <summary>
			/// Specify a private key password, if required for the key specified in the <see cref="PrivateKeyFile"/> property.
			/// </summary>
			public string PrivateKeyPassword
			{
				get => AdapterConfig.PrivateKeyPassword;
				set => AdapterConfig.PrivateKeyPassword = value;
			}

			/// <summary>
			/// Specifies the fingerprint of the server used by the adapter to authenticate the server if the <see
			/// cref="AcceptAnySshServerHostKey"/> property is set to <c>False</c>. If the fingerprints do not match, the
			/// connection fails.
			/// </summary>
			public string SshServerHostKeyFingerPrint
			{
				get => AdapterConfig.SSHServerHostKey;
				set => AdapterConfig.SSHServerHostKey = value;
			}

			/// <summary>
			/// The Single Sign On (SSO) Affiliate Application.
			/// </summary>
			public string AffiliateApplicationName
			{
				get => AdapterConfig.AffiliateApplicationName;
				set => AdapterConfig.AffiliateApplicationName = value;
			}

			/// <summary>
			/// Specifies a username to log on to the SFTP server.
			/// </summary>
			public string UserName
			{
				get => AdapterConfig.UserName;
				set => AdapterConfig.UserName = value;
			}

			/// <summary>
			/// Specify the SFTP user password if you set the <see cref="ClientAuthenticationMode"/> to <see
			/// cref="Microsoft.BizTalk.Adapter.Sftp.ClientAuthenticationMode.Password"/>.
			/// </summary>
			public string Password
			{
				get => AdapterConfig.Password;
				set => AdapterConfig.Password = value;
			}

			#endregion

			#region SSH Server Settings

			/// <summary>
			/// Specifies the server name or IP address of the secure FTP server.
			/// </summary>
			public string ServerAddress
			{
				get => AdapterConfig.ServerAddress;
				set => AdapterConfig.ServerAddress = value;
			}

			/// <summary>
			/// Specifies the port address for the secure FTP server on which the file transfer takes place.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>22</c>.
			/// </remarks>
			public int Port
			{
				get => AdapterConfig.Port;
				set => AdapterConfig.Port = value;
			}

			/// <summary>
			/// Specifies the folder path on the secure FTP server from where the receive location can retrieve files.
			/// </summary>
			public string FolderPath
			{
				get => AdapterConfig.FolderPath;
				set => AdapterConfig.FolderPath = value;
			}

			/// <summary>
			/// Specifies the file mask to use when retrieving files from a secure FTP server.
			/// </summary>
			public string FileMask
			{
				get => AdapterConfig.FileMask;
				set => AdapterConfig.FileMask = value;
			}

			#endregion
		}

		#endregion
	}
}

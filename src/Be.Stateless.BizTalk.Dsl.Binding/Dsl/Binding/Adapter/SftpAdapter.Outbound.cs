#region Copyright & License

// Copyright © 2012 - 2020 François Chabot
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
using System.Net;
using Microsoft.BizTalk.Adapter.Sftp;
using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class SftpAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// Use the SFTP adapter to send and receive messages from a secure FTP server using the SSH file transfer protocol.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/sftp-adapter">SFTP Adapter</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/sftp-adapter#configure-the-send-port">Configure the send port</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/sftp-adapter#frequently-asked-questions">Frequently asked questions</seealso>
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public class Outbound : SftpAdapter<SftpTLConfig>, IOutboundAdapter
		{
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public Outbound()
			{
				// Proxy Settings
				ProxyPort = 1080;
				ProxyType = ProxyType.None;

				// Security Settings
				AcceptAnySshServerHostKey = false;
				ClientAuthenticationMode = ClientAuthenticationMode.Password;
				EncryptionCipher = EncryptionCipher.Auto;

				// SSH Server Settings
				Port = 22;
				TargetFileName = "%MessageID%.xml";
			}

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				if (adapterConfigurator == null) throw new ArgumentNullException(nameof(adapterConfigurator));
				adapterConfigurator(this);
			}

			#region Base Class Member Overrides

			protected override string GetAddress()
			{
				return $"sftp://{ServerAddress}:{Port}/{FolderPath}/{TargetFileName}";
			}

			protected override void Validate()
			{
				// see Microsoft.BizTalk.Adapter.Sftp.SftpTLConfig.Validate()
				// see Microsoft.BizTalk.Adapter.Wcf.Config.TLConfig.ValidateBinding()
				var binding = _adapterConfig.CreateBinding(new SftpTHConfig());
				binding.CreateBindingElements();
				// see Microsoft.BizTalk.Adapter.Wcf.Config.TLConfig.ValidateAddress()
				AdapterConfigValidationHelper.ValidateAddress(GetAddress().Replace("%", WebUtility.UrlEncode("%")), UriKind.Absolute, binding.Scheme, null);
			}

			#endregion

			#region Other Settings

			/// <summary>
			/// Specify the maximum number of concurrent connections that can be opened to the server.
			/// </summary>
			public int ConnectionLimit
			{
				get => _adapterConfig.ConnectionLimit;
				set => _adapterConfig.ConnectionLimit = value;
			}

			/// <summary>
			/// The full path to create a client-side log file. Use this log file to troubleshoot any errors.
			/// </summary>
			public string Log
			{
				get => _adapterConfig.Log;
				set => _adapterConfig.Log = value;
			}

			/// <summary>
			/// A temporary folder on the SFTP server to upload large files to, before they can be atomically moved to the
			/// required location on the same server.
			/// </summary>
			public string TemporaryFolder
			{
				get => _adapterConfig.TemporaryFolder;
				set => _adapterConfig.TemporaryFolder = value;
			}

			#endregion

			#region Proxy Settings

			/// <summary>
			/// Specifies either the DNS name or the IP address of the proxy server.
			/// </summary>
			public string ProxyAddress
			{
				get => _adapterConfig.ProxyAddress;
				set => _adapterConfig.ProxyAddress = value;
			}

			/// <summary>
			/// Specifies the port for the proxy server.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>1080</c>.
			/// </remarks>
			public int ProxyPort
			{
				get => _adapterConfig.ProxyPort;
				set => _adapterConfig.ProxyPort = value;
			}

			/// <summary>
			/// Specifies the username for the proxy server.
			/// </summary>
			public string ProxyUserName
			{
				get => _adapterConfig.ProxyUserName;
				set => _adapterConfig.ProxyUserName = value;
			}

			/// <summary>
			/// Specifies the password for the proxy server.
			/// </summary>
			public string ProxyPassword
			{
				get => _adapterConfig.ProxyPassword;
				set => _adapterConfig.ProxyPassword = value;
			}

			/// <summary>
			/// Specifies the protocol used by the proxy server.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="Microsoft.BizTalk.Adapter.Sftp.ProxyType.None"/>.
			/// </remarks>
			public ProxyType ProxyType
			{
				get => _adapterConfig.ProxyType;
				set => _adapterConfig.ProxyType = value;
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
				get => _adapterConfig.AccessAnySSHServerHostKey;
				set => _adapterConfig.AccessAnySSHServerHostKey = value;
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
				get => _adapterConfig.ClientAuthenticationMode;
				set => _adapterConfig.ClientAuthenticationMode = value;
			}

			/// <summary>
			/// Specify the kind of encryption cipher.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="Microsoft.BizTalk.Adapter.Sftp.EncryptionCipher.Auto"/>.
			/// </remarks>
			public EncryptionCipher EncryptionCipher
			{
				get => _adapterConfig.EncryptionCipher;
				set => _adapterConfig.EncryptionCipher = value;
			}

			/// <summary>
			/// Comma-separated list of KEX preference order. Token WARN is used to delimit substandard KEXes. Example:
			/// ecdh,dh-gex-sha1,dh-group14-sha1,rsa,WARN,dh-group1-sha1. Visit WinSCP website for latest information.
			/// </summary>
			public string KeyExchangeAlgorithmSelectionPolicy
			{
				get => _adapterConfig.KexPolicy;
				set => _adapterConfig.KexPolicy = value;
			}

			/// <summary>
			/// Specify the private key for the SFTP user if you set the <see cref="ClientAuthenticationMode"/> to <see
			/// cref="Microsoft.BizTalk.Adapter.Sftp.ClientAuthenticationMode.PublicKeyAuthentication"/>.
			/// </summary>
			public string PrivateKeyFile
			{
				get => _adapterConfig.PrivateKeyFile;
				set => _adapterConfig.PrivateKeyFile = value;
			}

			/// <summary>
			/// Specify a private key password, if required for the key specified in the <see cref="PrivateKeyFile"/> property.
			/// </summary>
			public string PrivateKeyPassword
			{
				get => _adapterConfig.PrivateKeyPassword;
				set => _adapterConfig.PrivateKeyPassword = value;
			}

			/// <summary>
			/// Specifies the fingerprint of the server used by the adapter to authenticate the server if the <see
			/// cref="AcceptAnySshServerHostKey"/> property is set to <c>False</c>. If the fingerprints do not match, the
			/// connection fails.
			/// </summary>
			public string SshServerHostKeyFingerPrint
			{
				get => _adapterConfig.SSHServerHostKeyFingerPrint;
				set => _adapterConfig.SSHServerHostKeyFingerPrint = value;
			}

			/// <summary>
			/// The Single Sign On (SSO) Affiliate Application.
			/// </summary>
			public string AffiliateApplicationName
			{
				get => _adapterConfig.AffiliateApplicationName;
				set => _adapterConfig.AffiliateApplicationName = value;
			}

			/// <summary>
			/// Specifies a username to log on to the SFTP server.
			/// </summary>
			public string UserName
			{
				get => _adapterConfig.UserName;
				set => _adapterConfig.UserName = value;
			}

			/// <summary>
			/// Specify the SFTP user password if you set the <see cref="ClientAuthenticationMode"/> to <see
			/// cref="Microsoft.BizTalk.Adapter.Sftp.ClientAuthenticationMode.Password"/>.
			/// </summary>
			public string Password
			{
				get => _adapterConfig.Password;
				set => _adapterConfig.Password = value;
			}

			#endregion

			#region SSH Server Settings

			/// <summary>
			/// Specifies the server name or IP address of the secure FTP server.
			/// </summary>
			public string ServerAddress
			{
				get => _adapterConfig.ServerAddress;
				set => _adapterConfig.ServerAddress = value;
			}

			/// <summary>
			/// Specifies the port address for the secure FTP server on which the file transfer takes place.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>22</c>.
			/// </remarks>
			public int Port
			{
				get => _adapterConfig.Port;
				set => _adapterConfig.Port = value;
			}

			/// <summary>
			/// Specifies the folder path on the secure FTP server where the file is copied.
			/// </summary>
			public string FolderPath
			{
				get => _adapterConfig.FolderPath;
				set => _adapterConfig.FolderPath = value;
			}

			/// <summary>
			/// Specifies the name with which the file is transferred to the secure FTP server. You can also use macros for the
			/// target file name.
			/// </summary>
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public string TargetFileName
			{
				get => _adapterConfig.TargetFileName;
				set => _adapterConfig.TargetFileName = value;
			}

			/// <summary>
			/// If the file being transferred to the secure FTP server already exists at the destination, this property specifies
			/// whether the data from the file being transferred should be appended to the existing file. If set to <c>True</c>,
			/// the data is appended. If set to <c>False</c>, the file at the destination server is overwritten.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
			/// </remarks>
			public bool AppendIfExists
			{
				get => _adapterConfig.AppendIfExists;
				set => _adapterConfig.AppendIfExists = value;
			}

			#endregion
		}

		#endregion
	}
}

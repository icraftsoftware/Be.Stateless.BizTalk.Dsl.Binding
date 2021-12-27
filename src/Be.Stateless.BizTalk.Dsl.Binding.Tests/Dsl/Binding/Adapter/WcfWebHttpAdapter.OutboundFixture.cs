﻿#region Copyright & License

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
using System.ServiceModel;
using Be.Stateless.BizTalk.Adapter.Metadata;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;
using WebHttpSecurityMode = Microsoft.BizTalk.Adapter.Wcf.Config.WebHttpSecurityMode;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfWebHttpAdapterOutboundFixture
	{
		[SkippableFact]
		[SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
		public void SerializeToXml()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var wha = new WcfWebHttpAdapter.Outbound(
				a => {
					a.Address = new("https://localhost/dummy.svc");

					a.Identity = EndpointIdentityFactory.CreateSpnIdentity("spn_name");
					a.SecurityMode = WebHttpSecurityMode.Transport;
					a.ServiceCertificate = "thumbprint";
					a.TransportClientCredentialType = HttpClientCredentialType.Basic;
					a.UserName = "username";
					a.Password = "p@ssw0rd";

					a.UseAcsAuthentication = true;
					a.StsUri = new("https://localhost/swt_token_issuer");
					a.IssuerName = "issuer_name";
					a.IssuerSecret = "secret";

					a.SuppressMessageBodyForHttpVerbs = "GET";
					a.HttpHeaders = "Content-Type: application/json";
					a.HttpUrlMapping = new HttpUrlMapping {
						new("AddCustomer", "POST", "/Customer/{id}"),
						new("DeleteCustomer", "DELETE", "/Customer/{id}")
					};
					a.VariableMapping = new VariableMapping {
						new("id", BizTalkFactoryProperties.MapTypeName)
					};

					a.SendTimeout = TimeSpan.FromMinutes(2);
					a.MaxReceivedMessageSize = 2048;
				});
			var xml = wha.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<MaxReceivedMessageSize vt=\"3\">2048</MaxReceivedMessageSize>" +
				"<SecurityMode vt=\"8\">Transport</SecurityMode>" +
				"<TransportClientCredentialType vt=\"8\">Basic</TransportClientCredentialType>" +
				"<ServiceCertificate vt=\"8\">thumbprint</ServiceCertificate>" +
				"<UseSSO vt=\"11\">0</UseSSO>" +
				"<UserName vt=\"8\">username</UserName>" +
				"<Password vt=\"8\">p@ssw0rd</Password>" +
				"<ProxyToUse vt=\"8\">None</ProxyToUse>" +
				"<SuppressMessageBodyForHttpVerbs vt=\"8\">GET</SuppressMessageBodyForHttpVerbs>" +
				"<HttpHeaders vt=\"8\">Content-Type: application/json</HttpHeaders>" +
				"<HttpMethodAndUrl vt=\"8\">" + (
					"&lt;BtsHttpUrlMapping&gt;" +
					"&lt;Operation Name=\"AddCustomer\" Method=\"POST\" Url=\"/Customer/{id}\" /&gt;" +
					"&lt;Operation Name=\"DeleteCustomer\" Method=\"DELETE\" Url=\"/Customer/{id}\" /&gt;" +
					"&lt;/BtsHttpUrlMapping&gt;") +
				"</HttpMethodAndUrl>" +
				"<VariablePropertyMapping vt=\"8\">" + (
					"&lt;BtsVariablePropertyMapping&gt;" +
					$"&lt;Variable Name=\"id\" PropertyName=\"{BizTalkFactoryProperties.MapTypeName.Name}\" PropertyNamespace=\"{BizTalkFactoryProperties.MapTypeName.Namespace}\" /&gt;"
					+ "&lt;/BtsVariablePropertyMapping&gt;") +
				"</VariablePropertyMapping>" +
				"<EndpointBehaviorConfiguration vt=\"8\">" + (
					"&lt;behavior name=\"EndpointBehavior\" /&gt;") +
				"</EndpointBehaviorConfiguration>" +
				"<StsUri vt=\"8\">https://localhost/swt_token_issuer</StsUri>" +
				"<IssuerName vt=\"8\">issuer_name</IssuerName>" +
				"<IssuerSecret vt=\"8\">secret</IssuerSecret>" +
				"<UseAcsAuthentication vt=\"11\">-1</UseAcsAuthentication>" +
				"<UseSasAuthentication vt=\"11\">0</UseSasAuthentication>" +
				"<OpenTimeout vt=\"8\">00:01:00</OpenTimeout>" +
				"<SendTimeout vt=\"8\">00:02:00</SendTimeout>" +
				"<CloseTimeout vt=\"8\">00:01:00</CloseTimeout>" +
				"<Identity vt=\"8\">" + (
					"&lt;identity&gt;\r\n" +
					"  &lt;servicePrincipalName value=\"spn_name\" /&gt;\r\n" +
					"&lt;/identity&gt;") +
				"</Identity>" +
				"</CustomProps>");
		}

		[Fact(Skip = "TODO")]
		public void Validate()
		{
			// TODO Validate()
		}

		[SkippableFact]
		public void ValidateDoesNotThrow()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var wha = new WcfWebHttpAdapter.Outbound(
				a => {
					a.Address = new("https://localhost/dummy.svc");

					a.Identity = EndpointIdentityFactory.CreateSpnIdentity("spn_name");
					a.SecurityMode = WebHttpSecurityMode.Transport;
					a.ServiceCertificate = "thumbprint";
					a.TransportClientCredentialType = HttpClientCredentialType.Basic;
					a.UserName = "username";
					a.Password = "p@ssw0rd";

					a.UseAcsAuthentication = true;
					a.StsUri = new("https://localhost/swt_token_issuer");
					a.IssuerName = "issuer_name";
					a.IssuerSecret = "secret";

					a.SuppressMessageBodyForHttpVerbs = "GET";
					a.HttpHeaders = "Content-Type: application/json";
					a.HttpUrlMapping = new HttpUrlMapping {
						new("AddCustomer", "POST", "/Customer/{id}"),
						new("DeleteCustomer", "DELETE", "/Customer/{id}")
					};
					a.VariableMapping = new VariableMapping {
						new("id", BizTalkFactoryProperties.MapTypeName)
					};

					a.MaxReceivedMessageSize = 2048;
				});

			Invoking(() => ((ISupportValidation) wha).Validate()).Should().NotThrow();
		}
	}
}

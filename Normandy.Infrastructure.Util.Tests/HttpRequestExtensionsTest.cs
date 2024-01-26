using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;
using Microsoft.AspNetCore.Http;
using Normandy.Identity.Server.Application.Contracts.Dtos;
using Normandy.Infrastructure.Util.Common;
using Normandy.Infrastructure.Util.HttpUtil;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Normandy.Infrastructure.Util.Tests
{
    [TestFixture]
    internal class HttpRequestExtensionsTest
    {
        [Test]
        public void ParseHeaderTest()
        {
            var headerDic = new HeaderDictionary();
            headerDic.Add("h-appid", "appid");
            headerDic.Add("h-appkey", "appkey");

            var dic = headerDic.Keys.Aggregate(new Dictionary<string, string>(), (des, key) =>
            {
                headerDic.TryGetValue(key, out var values);
                var val = values.FirstOrDefault();
                des.Add(key, val);
                return des;
            });

            var header = dic.ConvertToModel<IdentityHeader>();
            Assert.That(header, Is.Not.Null);
            Assert.That(header.AppId, Is.EqualTo("appid"));
            Assert.That(header.AppKey, Is.EqualTo("appkey"));
        }

        [TestCase("127.0.0.1")]
        [TestCase("www.baidu.com")]
        public async Task DNSTest(string host)
        {
            var ipHost = await Dns.GetHostEntryAsync(host);
            var ip = ipHost.AddressList;
        }

        [Test]
        public async Task DNSResolveTest()
        {
            var ip = await DNSResolve();
            Assert.That(ip, Is.Not.EqualTo(string.Empty));
        }

        public async Task<string> DNSResolve()
        {
            var ipHost = await Dns.GetHostEntryAsync("slavedns.123ths.com");
            var ipList = ipHost.AddressList;
            if(ipList == null || ipList.Length <=0)
            {
                return string.Empty;
            }

            var dnsClient = new DnsClient(ipList[0], 1000);
            var dnsMessage = await dnsClient.ResolveAsync(DomainName.Parse("www.10jqka.com.cn"), RecordType.A);
            if (dnsMessage == null || (dnsMessage.ReturnCode != ReturnCode.NoError && dnsMessage.ReturnCode != ReturnCode.NxDomain))
            {
                return string.Empty;
            }

            return dnsMessage.AnswerRecords.Select(t => (t as ARecord)?.Address.ToString()).Where(a => !string.IsNullOrEmpty(a)).FirstOrDefault();

        }
    }
}

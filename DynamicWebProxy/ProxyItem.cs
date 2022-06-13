using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebProxy
{
    public class ProxyItem
    {
        private Uri? _uri;

        public Uri Uri
        {
            get => _uri;
            set
            {
                if (value == null) throw new InvalidOperationException("uri不能为空");

                _uri = value;
            }
        }

        public int HitCount { get; set; }

        public double Delay { get; set; }

        public string? ProxyType { get; set; }

        public ProxyStatus ProxyStatus { get; set; } = ProxyStatus.None;

        public string Location { get; set; }

        public string Source { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int InValidTimes { get; set; }

        public ProxyItem Clone()
        {
            return new ProxyItem
            {
                ProxyType = ProxyType,
                ProxyStatus = ProxyStatus.None,
                Location = Location,
                Source = Source,
                UserName = UserName,
                Password = Password,
                Uri = Uri,
                Delay = Delay,
                HitCount = HitCount,
                InValidTimes = InValidTimes
            };
        }
    }
}
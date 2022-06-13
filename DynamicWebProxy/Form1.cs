using System.Net;

namespace DynamicWebProxy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnGeneral_Click(object sender, System.EventArgs e)
        {
            var proxy = await ProxyHelper.GeneralProxyAsync();
            txtProxy.Text = (proxy as WebProxy)?.Address?.ToString();
        }
    }
}
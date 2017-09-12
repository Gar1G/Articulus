using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Websites
{
    public class WebsiteElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int WebsiteId { get; set; }
        public string WebsiteName { get; set; }
        public string OriginalImage { get; set; }
        public string BlurredImage;
        public int Checked = 0;

        private string WebsiteImage;
        public string setWebsiteImage
        {
            get
            {
                return this.WebsiteImage;
            }
            set
            {
                if (value != this.WebsiteImage)
                {
                    this.WebsiteImage = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _opacity;
        public double Opacity
        {
            get { return this._opacity; }
            set { this._opacity = value; NotifyPropertyChanged(); }
        }

    }
}

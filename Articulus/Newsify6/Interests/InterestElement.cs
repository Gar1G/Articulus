using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Newsify6.Interests
{
    public class InterestElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int InterestId { get; set; }
        public string InterestType { get; set; }
        public string OriginalImage { get; set; }
        public string BlurredImage;
        public int Checked = 0;

        private string InterestImage;
        public string setInterestImage
        {
            get
            {
                return this.InterestImage;
            }

            set
            {
                if (value != this.InterestImage)
                {
                    this.InterestImage = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}

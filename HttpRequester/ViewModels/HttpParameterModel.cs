using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequester.ViewModels
{
    public class HttpParameterModel : BaseModel
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set 
            { 
                _name = value;
                NotifyPropertyChanged();
            }
        }
        

        private string _value;

        public string Value
        {
            get { return _value; }
            set 
            { 
                _value = value; 
                NotifyPropertyChanged(); 
            }
        }
        
    }
}

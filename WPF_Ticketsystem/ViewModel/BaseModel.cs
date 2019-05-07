using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace WPF_Ticketsystem
{
  public class BaseModel : INotifyPropertyChanged
  {
    // Declare the PropertyChanged event
    public event PropertyChangedEventHandler PropertyChanged;

    // OnPropertyChanged will raise the PropertyChanged event passing the
    // source property that is being updated.
    protected void onPropertyChanged([CallerMemberName]string propertyName = "")
    {
      if (this.PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}

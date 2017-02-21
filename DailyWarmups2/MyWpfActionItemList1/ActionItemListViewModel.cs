using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWpfActionItemList1
{
    public class ActionItemListViewModel : ViewModelBase
    {
        private readonly ObservableCollection<ActionItemViewModel> _actionItems;

        public ObservableCollection<ActionItemViewModel> ActionItems
        {
            get { return _actionItems; }
        //    set { _actionItems = value; }
        }

        
        public ActionItemListViewModel()
        {
            _actionItems = new ObservableCollection<ActionItemViewModel>();

            // Create dummy data
            _actionItems.Add(new ActionItemViewModel { Description = "Eat cheese" });
            _actionItems.Add(new ActionItemViewModel { Description = "Practice parking" });
        }
    }
}

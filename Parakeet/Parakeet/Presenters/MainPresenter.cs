using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parakeet.DataModel;
using Parakeet.Infrastructure;

namespace Parakeet.Presenters
{
    public class MainPresenter : Presenter
    {
        public ThoughtEntryPresenter Entry { get; set; }
        public Presenter ActiveDisplay { get; private set; }
        public void SetActiveDisplay(Presenter presenter)
        {
            ActiveDisplay = presenter;
            RaisePropertyChanged("ActiveDisplay");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Parakeet.Sessions;
using Parakeet.Infrastructure;

namespace Parakeet.Presenters
{
    public class ThoughtReviewPresenter : Presenter
    {
        public ThoughtReviewPresenter(MessageReviewSession review)
        {
            Caption = review.Message.Caption;
            Repost = new DelegateCommand(review.RepostCurrent);
            Dismiss = new DelegateCommand(review.DismissCurrent);
        }
        public string Caption { get; private set; }
        public ICommand Repost { get; private set; }
        public ICommand Dismiss { get; private set; }
    }
}

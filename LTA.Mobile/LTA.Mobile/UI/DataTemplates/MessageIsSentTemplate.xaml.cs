﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.UI.DataTemplates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageIsSentTemplate : BaseTemplate
    {
        public MessageIsSentTemplate()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            this.ScaleTo(1, 300);
            base.OnBindingContextChanged();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Avalanche
{
    public interface IPageAttribute
    {
        void Modify(ContentPage contentPage, string value);
    }
}
